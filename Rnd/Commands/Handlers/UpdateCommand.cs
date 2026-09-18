using Rnd.Commands.Base;
using Rnd.Models.DTOs;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    public class UpdateCommand : CommandBase
    {
        public override string Name => "update";

        public override string Description => "Update Data. Using: update <id> <FirstName>" +
            " <LastName> [email]";
        public override string[] Aliases => new[] { "edit", "modify" };
        public UpdateCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 4, out var error))
                return error;

            if (!TryParseId(args[1], out var id, out error))
                return error;

            //var firstName = args[2];
            //var lastName = args[3];
            //var emil = args.Length > 4 ? args[4] : null;

            var dto = new UpdatePersonDto
            {
                Id = id,
                FirstName = args[1],
                LastName = args[2],
                Email = args.Length > 4 ? args[3] : string.Empty
            };

            var updated = await _service.UpdateAsync(dto, cancellationToken);
            if (!updated)
                return CommandResult.NotFound("Person", id);

            return CommandResult.Ok($"Person With ID {id} updated Seccessfully");
        }

        protected override string GetUsage() => "update <id> <firstName> <lastName> [email]";
    }
}
