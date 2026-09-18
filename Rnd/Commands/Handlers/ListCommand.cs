using Rnd.Commands.Base;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    internal class ListCommand : CommandBase
    {
        public override string Name => "list";

        public override string Description => "Getting List of Persons";
        public override string[] Aliases => new[] { "ls", "all" };
        public ListCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            var persons = await _service.GetAllAsync();
            var list = persons.ToList();

            if (!list.Any())
                return CommandResult.Ok("No Perosn Founded");

            var message = $"Total {list.Count} persons\n" +
                string.Join("\n", list.Select(p =>
                $"  {p.Id} - {p.FullName} ({p.Email ?? "no email"})"));

            return CommandResult.Ok(message, list);
        }

        protected override string GetUsage() => "list";
    }
}
