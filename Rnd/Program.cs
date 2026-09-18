using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rnd.Commands.Base;
using Rnd.Commands.Factory;
using Rnd.Commands.Handlers;
using Rnd.Commands.Parser;
using Rnd.Data;
using Rnd.Data.Repositories;
using Rnd.Mapping;
using Rnd.Models;
using Rnd.Services;
using Serilog;
using Serilog.Settings.Configuration;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        Console.CancelKeyPress += (sender, e) =>
        {
            Log.Warning($"Ctrl+C pressed. Cancelling operations...");
            e.Cancel = true;
            cts.Cancel();
        };

        var configuration = GetConfiguration();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration,
                new ConfigurationReaderOptions(Assembly.Load("Serilog.Sinks.Console"), Assembly.Load("Serilog.Sinks.File"))
            )
            .WriteTo.Console(
                outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code
            )
            .WriteTo.File(
                path: "logs/app-.txt",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            ).CreateLogger();

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(Log.Logger);
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var autoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PersonProfile>();
            cfg.AddProfile<OrderProfile>();
        }, loggerFactory);

#if DEBUG
        autoMapperConfig.AssertConfigurationIsValid();
#endif

        var mapper = autoMapperConfig.CreateMapper();

        try
        {
            Log.Information("Starting application");

            var optionBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionBuilder.LogTo(
                    message => Log.Information(message),
                    LogLevel.Information,
                    DbContextLoggerOptions.DefaultWithUtcTime | DbContextLoggerOptions.SingleLine
                );
            using var context = new ApplicationContext(optionBuilder.Options);

            Log.Information("Checking for pending migrations");
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Log.Information("Applying pending migrations");
                await context.Database.MigrateAsync();
                Log.Information("Pending migrations applied successfully");
            }
            else
            {
                Log.Information("No pending migrations found");
            }
            var personRepository = new Repository<Person>(context);
            var personService = new PersonService(personRepository, mapper);

            var orderRepository = new Repository<Order>(context);
            var orderService = new OrderService(orderRepository, mapper);

            var environment = configuration.GetValue<string>("Environment");
            var commands = new List<ICommand>()
            {
                new AddCommand(personService),
                new ListCommand(personService),
                new GetCommand(personService),
                new UpdateCommand(personService),
                new DeleteCommand(personService),
                new RestoreCommand(personService),
                new SearchCommand(personService),
                new PageCommand(personService),
                new ExitCommand()
            };

            if (environment is "Development")
            {
                commands.Add(new TestCancellCommand(personService));
            }

            var commandFactory = new CommandFactory(commands);
            var helpCommand = new HelpCommand(personService, commandFactory);
            commands.Add(helpCommand);
            var finalFactory = new CommandFactory(commands);

            while (!token.IsCancellationRequested)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var (commandName, cmdArgs) = CommandParser.Parse(input);
                var command = finalFactory.GetCommand(commandName);

                if (command is null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Unknows Command '{commandName}'. Type 'help'");
                    Console.ResetColor();
                    continue;
                }

                var result = await command.ExecuteAsync(cmdArgs, token);

                if (result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(result.Message);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"X {result.Message}");
                    Console.ResetColor();
                }
            }

        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
    private static IConfigurationRoot GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}