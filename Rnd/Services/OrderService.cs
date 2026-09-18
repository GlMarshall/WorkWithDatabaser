using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Rnd.Data.Repositories;
using Rnd.Models;
using Rnd.Models.DTOs;
using Serilog;

namespace Rnd.Services
{
    public class OrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IRepository<Order> orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task<OrderDto> AddAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
        {
            Log.Information($"Adding New Order for Person {dto.PersonId}");

            var order = _mapper.Map<Order>(dto);
            order.OrderDate = DateTime.Now;
            order.IsDeleted = false;

            Log.Information($"Added Order With Id: {order.Id}");

            var created = await _orderRepository.GetQueryable()
                .Include(o => o.Person)
                .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

            return _mapper.Map<OrderDto>(created);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            Log.Information("Getting All Orders");

            return await _orderRepository.GetQueryable()
                .Include(o => o.Person)
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.OrderDate)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<OrderDto>> GetByPersonIdAsync(int personId,
            CancellationToken cancellationToken = default)
        {
            Log.Information($"Getting Orders for Person {personId}");

            return await _orderRepository.GetQueryable()
                .Include(o => o.Person)
                .Where(o => !o.IsDeleted && o.PersonId == personId)
                .OrderByDescending(o => o.OrderDate)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(UpdateOrderDto dto, CancellationToken cancellationToken = default)
        {
            Log.Information($"Updateing Order With Id: {dto.Id}");

            var order = await _orderRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (order is null || order.IsDeleted)
                return false;

            _mapper.Map(dto, order);
            _orderRepository.Update(order);
            await _orderRepository.SaveChangeAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Log.Information($"Deleting Order With Id; {id}");

            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
            if (order is null || order.IsDeleted)
                return false;

            order.IsDeleted = true;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangeAsync(cancellationToken);
            return true;
        }
    }
}
