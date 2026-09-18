using Rnd.Commands.Base;

namespace Rnd.Commands.Factory
{
    public class CommandFactory
    {
        private readonly Dictionary<string, ICommand> _commands = new(StringComparer.CurrentCultureIgnoreCase);
        public CommandFactory(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
            {
                _commands[command.Name] = command;
                foreach (var alias in command.Aliases)
                {
                    _commands[alias] = command;
                }
            }
        }
        public ICommand? GetCommand(string name) =>
            _commands.TryGetValue(name, out var command) ? command : null;
        public IEnumerable<ICommand> GetAllCommands() =>
            _commands.Values.Distinct().OrderBy(c => c.Name);
    }
}
