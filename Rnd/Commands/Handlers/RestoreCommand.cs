using Rnd.Commands.Base;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    internal class RestoreCommand : CommandBase
    {
        public override string Name => "restore";

        public override string Description => "Resote Person";
        public override string[] Aliases => new[] { "recover", "undo" };

        public RestoreCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 2, out var error))
                return error;
            if (!TryParseId(args[1], out var id, out error))
                return error;

            var restored = await _service.RestoreAsync(id, cancellationToken);
            if (!restored)
                return CommandResult.NotFound("Person", id);

            return CommandResult.Ok($"Person With ID {id} restored");
        }

        protected override string GetUsage() => "restore <id>";
    }
}
