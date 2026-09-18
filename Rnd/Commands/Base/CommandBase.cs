using Rnd.Services;
using Serilog;

namespace Rnd.Commands.Base
{
    public abstract class CommandBase : ICommand
    {
        protected readonly PersonService _personService;
        protected readonly OrderService _orderService;
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual string[] Aliases => Array.Empty<string>();
        public CommandBase(PersonService personService, OrderService orderService)
        {
            _personService = personService;
            _orderService = orderService;
        }

        public abstract Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default);
        protected bool ValidateArgumentCount(string[] args, int minCount, out CommandResult error)
        {
            if (args.Length >= minCount)
            {
                error = CommandResult.Ok(string.Empty);
                return true;
            }

            error = CommandResult.InvalidArgument(GetUsage());
            return false;
        }
        protected bool TryParseId(string arg, out int id, out CommandResult error)
        {
            if (int.TryParse(arg, out id) && id > 0)
            {
                error = CommandResult.Ok(string.Empty);
                return true;
            }

            error = CommandResult.Error($"Invlid Id: '{arg}'. Id Must be a positive integer");
            return false;
        }
        protected abstract string GetUsage();
        protected void LogInfo(string message) => Log.Information(message);
        protected void LogWarning(string message) => Log.Warning(message);
        protected void LogError(string message) => Log.Error(message);

    }
}
