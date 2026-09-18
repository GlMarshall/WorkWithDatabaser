using Rnd.Commands.Base;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    internal class DeleteCommand : CommandBase
    {
        public override string Name => "delete";

        public override string Description => "Delete Person. Using: delte <Id>";
        public override string[] Aliases => new[] { "remove", "del" };
        public DeleteCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 2, out var error))
                return error;
            if (!TryParseId(args[1], out var id, out error))
                return error;

            var deleted = await _service.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return CommandResult.NotFound("Person", id);

            return CommandResult.Ok($"Person With ID: {id} was deleted");
        }

        protected override string GetUsage() => "delete <Id>";
    }
}
