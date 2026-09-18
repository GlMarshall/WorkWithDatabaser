using Rnd.Commands.Base;
using Rnd.Models.DTOs;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    public class AddCommand : CommandBase
    {
        public override string Name => "add";

        public override string Description => "Adding New Person";
        public override string[] Aliases => new[] { "create", "new" };

        public AddCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 3, out var error))
                return error;

            //var firstName = args[1];
            //var lastName = args[2];
            //var email = args.Length > 3 ? args[3] : null;
            var dto = new CreatePersonDto
            {
                FirstName = args[1],
                LastName = args[2],
                Email = args.Length > 3 ? args[3] : null
            };

            try
            {
                var person = await _service.AddAsync(dto);
                LogInfo($"Added Person: {person.FullName} (ID: {person.Id})");
                return CommandResult.Ok($"Added: {person.FullName} (ID: {person.Id})", person);
            }
            catch (Exception ex)
            {
                LogError($"Failed to add person: {ex.Message}");
                return CommandResult.Error($"Failed to add Person: {ex.Message}");
            }
        }
        protected override string GetUsage() => "add <FirstName> <LastName> [Email]";
    }
}
