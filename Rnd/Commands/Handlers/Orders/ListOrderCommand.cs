using Rnd.Commands.Base;
using Rnd.Services;

namespace Rnd.Commands.Handlers.Orders
{
    public class ListOrderCommand : CommandBase
    {

        public override string Name => "orders";

        public override string Description => "Show all orders";
        public override string[] Aliases => new[] { "lo" };
        public ListOrderCommand(PersonService personService, OrderService orderService)
            : base(personService, orderService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            var orders = await _orderService.GetAllAsync(cancellationToken);
            var list = orders.ToList();

            if (!list.Any())
                return CommandResult.Ok("No orders found");

            var message = $"\nTotal Orders: {list.Count}\n" +
                string.Join("\n", list.Select(o =>
                $" #{o.Id} - {o.PersonFullName} | {o.Amount:C} | {o.OrderDate:yyy-MM-dd}"));

            return CommandResult.Ok(message, list);
        }

        protected override string GetUsage() => "orders";
    }
}
