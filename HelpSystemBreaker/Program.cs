using HelpSystemBreaker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.API;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Hosting.Extensions;
using VTP.Remora.Commands.HelpSystem;

public class Program
{

    // Debug Server: Bot Testing 2, Test Server: Bot Testing, Release Server: None
    private const string DebugServer = "756877443893166110", TestServer = "756572412782575716", ReleaseServer = "0";

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args)
                  .UseConsoleLifetime()
                  .Build();

        var services = host.Services;
        var log = services.GetRequiredService<ILogger<Program>>();

        
        var slashService = services.GetRequiredService<SlashService>();
        var checkSlashSupport = slashService.SupportsSlashCommands();
        if (!checkSlashSupport.IsSuccess)
        {
            log.LogCritical("The registered commands of the bot don't support slash commands: {Reason}", checkSlashSupport.Error?.Message);
            Environment.Exit(-5);
        }

        Console.WriteLine("Select Servers to publish changes to:");
        var input = "001";
        var target = Convert.ToInt32(input, 2);
        
        if ((target & 0b111) == 0)
        {
            log.LogWarning("No update scope selected");
        }
        if ((target & 0b001) == 0b001)
        {
            // Debug Server should be used

            if (!DiscordSnowflake.TryParse(DebugServer, out var debugSnowflake))
            {
                log.LogCritical("Debug Server ID is invalid");
                Environment.Exit(-6);
            }
            var updateDebugSlash = await slashService.UpdateSlashCommandsAsync(debugSnowflake);
            if (!updateDebugSlash.IsSuccess)
            {
                log.LogWarning("Failed to update debug slash commands: {Reason}", updateDebugSlash.Error?.Message);
            }
        }
        if ((target & 0b001) == 0b010)
        {
            // Test Server should be used

            if (!DiscordSnowflake.TryParse(TestServer, out var testSnowflake))
            {
                log.LogCritical("Test Server ID is invalid");
                Environment.Exit(-6);
            }
            var updateTestSlash = await slashService.UpdateSlashCommandsAsync(testSnowflake);
            if (!updateTestSlash.IsSuccess)
            {
                log.LogWarning("Failed to update debug slash commands: {Reason}", updateTestSlash.Error?.Message);
            }
        }
        if ((target & 0b001) == 0b100)
        {
            // Release Server should be used

            if (!DiscordSnowflake.TryParse(ReleaseServer, out var releaseSnowflake))
            {
                log.LogCritical("Debug Server ID is invalid");
                Environment.Exit(-6);
            }
            var updateReleaseSlash = await slashService.UpdateSlashCommandsAsync(releaseSnowflake);
            if (!updateReleaseSlash.IsSuccess)
            {
                log.LogWarning("Failed to update debug slash commands: {Reason}", updateReleaseSlash.Error?.Message);
            }
        }

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .AddDiscordService
                        (
                         services =>
                         {
                             var configuration = services.GetRequiredService<IConfiguration>();

                             return configuration.GetValue<string?>("REMORA_BOT_TOKEN") ??
                                    throw new InvalidOperationException
                                        (
                                         "No bot token has been provided. Set the REMORA_BOT_TOKEN environment variable to a " +
                                         "valid token."
                                        );
                         }
                        )
                   .ConfigureServices((_, services) =>
                    {
                        services.AddDiscordCommands(true)
                                .AddCommandTree()
                                .WithCommandGroup<TestGroup>();
                        services.AddHelpSystem();
                    });
    }
    
}