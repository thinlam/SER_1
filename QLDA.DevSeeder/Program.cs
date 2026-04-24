using McMaster.Extensions.CommandLineUtils;
using QLDA.DevSeeder.Commands;

namespace QLDA.DevSeeder;

[Command(Name = "devseeder", Description = "QLDA DevSeeder - Generate and manage fake data for development")]
[Subcommand(typeof(SeedCommand))]
[Subcommand(typeof(ClearCommand))]
[HelpOption]
public class Program
{
    public static async Task<int> Main(string[] args) =>
        await CommandLineApplication.ExecuteAsync<Program>(args);

    private int OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
        return 0;
    }
}
