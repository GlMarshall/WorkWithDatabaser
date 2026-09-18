using Rnd.Commands.Base;
using Rnd.Models.DTOs;
using Rnd.Services;

namespace Rnd.Commands.Handlers.Orders
{
    public class AddOrderCommand : CommandBase
    {
        public override string Name => "add-order";

        public override string Description => "Add order: add-order <PersonId> <Amount> [Description]";
        public override string[] Aliases => new[] { "ao" };
        public AddOrderCommand(PersonService personService, OrderService orderService)
            : base(personService, orderService) { }

        public override async Task<CommandResult> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
        {
            if (!ValidateArgumentCount(args, 3, out var error))
                return error;

            if (!TryParseId(args[1], out var personId, out error))
                return error;

            if (!decimal.TryParse(args[1], out var amount))
                return CommandResult.Error($"Invalid Amount: {args[2]}");

            var dto = new CreateOrderDto
            {
                PersonId = personId,
                Amount = amount,
                Desciption = args.Length > 3 ? args[3] : null
            };

            try
            {
                var order = await _orderService.AddAsync(dto, cancellationToken);
                LogInfo($"Added order: {order.Id} for {order.PersonFullName}, Amount: {order.Amount}");
                return CommandResult.Ok($"Added Order #{order.Id} for {order.PersonFullName}", order);
            }
            catch (Exception ex)
            {
                LogError($"Failed to add order: {ex.Message}");
                return CommandResult.Error($"Failed to add order: {ex.Message}");
            }

        }

        protected override string GetUsage() =>
            "add-order <PersonId> <Amount> [Description]";
    }
}
