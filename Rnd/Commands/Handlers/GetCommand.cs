using Rnd.Commands.Base;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    public class GetCommand : CommandBase
    {
        public override string Name => "get";

        public override string Description => "Get Person By Id";
        public override string[] Aliases => new[] { "show", "view" };
        public GetCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 2, out var error))
                return error;

            if (!TryParseId(args[1], out var id, out error))
                return error;

            var person = await _service.GetByIdAsync(id, cancellationToken);
            if (person is null)
                return CommandResult.NotFound("Person", id);

            var message = $"\nID: {person.Id}\n" +
                      $"Name: {person.FullName}\n" +
                      $"Email: {person.Email ?? "not specified"}\n";
            //$"Created: {person.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
            //$"Deleted: {(person.IsDeleted ? "Yes" : "No")}";

            return CommandResult.Ok(message, person);
        }

        protected override string GetUsage() => "get <id>";
    }
}
