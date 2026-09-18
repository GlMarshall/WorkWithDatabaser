using Rnd.Commands.Base;

namespace Rnd.Commands.Handlers
{
    public class ExitCommand : ICommand
    {
        public string Name => "exit";

        public string Description => "Exit from Programm";

        public string[] Aliases => new[] { "quit", "q" };


        Task<CommandResult> ICommand.ExecuteAsync(string[] args, CancellationToken cancellationToken)
        {
            Console.WriteLine("Goodbye!");
            Environment.Exit(0);
            return Task.FromResult(CommandResult.Ok("Goodbye!"));
        }
    }
}
