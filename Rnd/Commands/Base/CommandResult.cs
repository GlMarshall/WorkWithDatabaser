namespace Rnd.Commands.Base
{
    public class CommandResult
    {
        public bool Success { get; }
        public string Message { get; }
        public object? Data { get; }
        private CommandResult(bool sucess, string message, object? data = null)
        {
            Success = sucess;
            Message = message;
            Data = data;
        }
        public static CommandResult Ok(string message, object? data = null)
            => new(true, message); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public static CommandResult Error(string message)
            => new(false, message);
        public static CommandResult NotFound(string entity, int id)
            => new(false, $"{entity} with Id {id} Not Found");
        public static CommandResult InvalidArgument(string expected)
            => new(false, $"Invalid arguments. Expected: {expected}");
    }
}
