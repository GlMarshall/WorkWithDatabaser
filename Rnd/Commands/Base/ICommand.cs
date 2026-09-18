namespace Rnd.Commands.Base
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string[] Aliases { get; }
        Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default);
    }
}
