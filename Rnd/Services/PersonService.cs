using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Rnd.Data.Repositories;
using Rnd.Models;
using Rnd.Models.DTOs;
using Serilog;

namespace Rnd.Services
{
    public class PersonService
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IMapper _mapper;
        public PersonService(IRepository<Person> personRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _mapper = mapper;
        }
        public async Task<PersonDto> AddAsync(CreatePersonDto dto, CancellationToken cancellationToken = default)
        {
            Log.Information($"Adding New Person: {dto.FirstName} {dto.LastName}");

            var person = _mapper.Map<Person>(dto);
            person.CreatedAt = DateTime.Now;
            person.IsDeleted = false;

            await _personRepository.AddAsync(person, cancellationToken);
            await _personRepository.SaveChangeAsync(cancellationToken);
            Log.Information($"Added New Person With Id: {person.Id}");
            return _mapper.Map<PersonDto>(person);
        }

        public async Task<IEnumerable<PersonDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            Log.Information("Getting Active Persons");
            //var persons = await _personRepository.GetAllAsync(
            //        filter: p => !p.IsDeleted,
            //        orderBy: q => q.OrderBy(p => p.LastName).ThenBy(p => p.FirstName),
            //        cancellationToken: cancellationToken
            //    );
            //return persons.Select(MapToDto); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //return _mapper.Map<IEnumerable<PersonDto>>(persons);
            return await _personRepository.GetQueryable()
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ProjectTo<PersonDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<PersonDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Log.Information($"Getting Person By Id: {id}");

            var person = await _personRepository.GetByIdAsync(id, cancellationToken);
            if (person is null || person.IsDeleted)
                return null;
            return _mapper.Map<PersonDto>(person);
        }
        public async Task<bool> UpdateAsync(
                UpdatePersonDto dto,
                CancellationToken cancellationToken = default
            )
        {
            Log.Information($"Updating Person With Id: {dto.Id}");
            //var person = await GetByIdAsync(dto.Id, cancellationToken);
            var person = await _personRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (person == null)
            {
                Log.Warning($"Person With Id: {dto.Id} Not Found");
                return false;
            }

            //person.FirstName = dto.FirstName;
            //person.LastName = dto.LastName;
            //person.Email = dto.Email ?? string.Empty;

            _mapper.Map(dto, person);

            _personRepository.Update(person);
            await _personRepository.SaveChangeAsync(cancellationToken);
            Log.Information($"Updated Person With Id: {dto.Id}");
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Log.Information($"Deleting Person With Id: {id}");
            //var person = await GetByIdAsync(id, cancellationToken);
            var person = await _personRepository.GetByIdAsync(id, cancellationToken);
            if (person == null)
            {
                Log.Warning($"Person With Id: {id} Not Found");
                return false;
            }
            person.IsDeleted = true;
            _personRepository.Update(person);
            await _personRepository.SaveChangeAsync(cancellationToken);
            Log.Information($"Deleted Person With Id: {id}");
            return true;
        }

        public async Task<IEnumerable<PersonDto>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            Log.Information($"Searching Personsos By Name: {searchTerm}");
            var persons = await _personRepository.GetAllAsync(
                    filter: p => !p.IsDeleted && (p.FirstName.Contains(searchTerm) || p.LastName.Contains(searchTerm)),
                    orderBy: q => q.OrderBy(p => p.LastName).ThenBy(p => p.FirstName),
                    cancellationToken: cancellationToken
                );
            return _mapper.Map<IEnumerable<PersonDto>>(persons);
        }
        public async Task<bool> RestoreAsync(int id, CancellationToken cancellationToken = default)
        {
            Log.Information($"Restoring Deleted Person with Id: {id}");
            var person = await _personRepository.GetFirstOrDefaultAsync(
                    p => p.Id == id && p.IsDeleted, cancellationToken
                );

            if (person is null)
            {
                Log.Warning($"Deleted Person with Id: {id} Was Not Found");
                return false;
            }

            person.IsDeleted = false;
            _personRepository.Update(person);
            await _personRepository.SaveChangeAsync(cancellationToken);
            Log.Information($"Person With Id: {id} Was Restored");
            return true;
        }
        public async Task<PageResult<PersonDto>> GetPagedAsync(
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default
            )
        {
            Log.Information($"Getting page {pageNumber} with size {pageSize}");
            var totalCount = await _personRepository.GetQueryable()
                .Where(p => !p.IsDeleted)
                .CountAsync(cancellationToken);

            var items = await _personRepository.GetQueryable()
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<PersonDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new PageResult<PersonDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
            };
        }
    }
}
