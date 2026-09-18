using Rnd.Commands.Base;
using Rnd.Services;

namespace Rnd.Commands.Handlers
{
    public class PageCommand : CommandBase
    {
        public override string Name => "page";

        public override string Description => "Show persons page by page: page <PageNumber> [PageSize]";
        public override string[] Aliases => new[] { "paged", "pg" };
        public PageCommand(PersonService personService) : base(personService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 2, out var error))
                return error;

            if (!TryParseId(args[1], out var pageNumber, out error))
                return error;

            var pageSize = args.Length > 2 && int.TryParse(args[2], out var size) ? size : 5;
            var result = await _service.GetPagedAsync(pageNumber, pageSize, cancellationToken);

            if (!result.Items.Any())
                return CommandResult.Ok($"No persons found on page {pageNumber}.");

            var message = $"\nPage {result.PageNumber} of {result.TotalPages} " +
                      $"(Total: {result.TotalCount} persons)\n" +
                      new string('─', 50) + "\n" +
                      string.Join("\n", result.Items.Select(p =>
                          $"  {p.Id} - {p.FullName} ({p.Email ?? "no email"})")) +
                      $"\n\n" +
                      $"{(result.HasPrevious ? "Previous" : "")} " +
                      $"{(result.HasNext ? "Next" : "")}";
            return CommandResult.Ok(message, result);
        }

        protected override string GetUsage() => "page <PageNumber> [PageSize]";
    }
}
