namespace Rnd.Commands.Parser
{
    public class CommandParser
    {
        public static (string Command, string[] Args) Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (string.Empty, Array.Empty<string>());

            var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return (parts[0].ToLowerInvariant(), parts);
        }
    }
}
