using Rnd.Commands.Base;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    internal class SearchCommand : CommandBase
    {
        public override string Name => "search";

        public override string Description => "Search persons by name: search<Term>";
        public override string[] Aliases => new[] { "find", "lookup" };
        public SearchCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 2, out var error))
                return error;

            var term = args[1];
            var persons = await _service.SearchByNameAsync(term, cancellationToken);
            var list = persons.ToList();

            if (!list.Any())
                return CommandResult.Ok($"No Persons found '{term}'");

            var message = $"\n🔍 Found {list.Count} persons matching '{term}':\n" +
                      string.Join("\n", list.Select(p =>
                          $"  {p.Id} - {p.FullName} ({p.Email ?? "no email"})"));

            return CommandResult.Ok(message, list);
        }

        protected override string GetUsage()
        {
            throw new NotImplementedException();
        }
    }
}
