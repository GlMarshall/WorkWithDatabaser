using Rnd.Commands.Base;
using Rnd.Commands.Factory;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    public class HelpCommand : CommandBase
    {
        private readonly CommandFactory _factory;
        public override string Name => "help";

        public override string Description => "Show Aviable Commands";
        public override string[] Aliases => new[] { "?", "h" };
        public HelpCommand(PersonService personService, CommandFactory factory) : base(personService)
        {
            _factory = factory;
        }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            var commands = _factory.GetAllCommands();

            var message = "\nAvailable commands:\n" +
                          "═══════════════════════════════════════════════\n" +
                          string.Join("\n", commands.Select(c =>
                              $"  {c.Name,-12} - {c.Description}")) +
                          "\n\nExamples:\n" +
                          "  add John Doe john@example.com\n" +
                          "  list\n" +
                          "  get 1\n" +
                          "  update 1 Johnathan Doe\n" +
                          "  delete 2\n" +
                          "  restore 2\n" +
                          "  search Alex\n" +
                          "  exit";

            return CommandResult.Ok(message);
        }

        protected override string GetUsage() => "help";
    }
}
