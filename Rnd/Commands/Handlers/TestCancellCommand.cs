using Rnd.Commands.Base;
using Rnd.Services;
using Serilog;

namespace Rnd.Commands.Handlers
{
    public class TestCancellCommand : CommandBase
    {
        public override string Name => "testcancel";

        public override string Description => "Test CancellationToken (5 sec delay, press Ctrl+C to cancel)";
        public TestCancellCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            try
            {
                Log.Information("Starting Test");
                var persons = await _service.GetAllAsync(cancellationToken);
                return CommandResult.Ok($"Success! Found {persons.Count()} persons");
            }
            catch (OperationCanceledException)
            {
                Log.Warning("Operation was canceled by user!");
                return CommandResult.Error("Operation Canceled");
            }
        }

        protected override string GetUsage()
        {
            throw new NotImplementedException();
        }
    }
}
