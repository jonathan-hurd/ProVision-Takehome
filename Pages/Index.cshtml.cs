using Microsoft.AspNetCore.Mvc.RazorPages;
using CliWrap;
using CliWrap.Buffered;

namespace ProVision_Takehome.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public Dictionary<string, string> ProgramVersions { get; private set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        ProgramVersions = new Dictionary<string, string>();
    }

    public async Task OnGet()
    {
        ProgramVersions = await GetProgramVersions();
    }

    async Task<Dictionary<string, string>> GetProgramVersions()
    {
        var programVersions = new Dictionary<string, string>();
        var cliCommands = new Dictionary<string, string>
        {
            { "dotnet", "--version" },
            {"npm", "--version"},
            {"python", "--version"},
            {"yarn", "--version"},
            {"node", "--version"},
            {"code", "--version"}
        };

        foreach (var program in cliCommands)
        {
            try 
            {
                var result = await Cli.Wrap(program.Key)
                    .WithArguments(program.Value)
                    .ExecuteBufferedAsync();

                programVersions[program.Key] = result.StandardOutput;
            }
            catch (Exception ex) 
            {
                programVersions[program.Key] = "No version installed";
                _logger.LogError(ex, "There was an error checking a program version");
            }
        }

        return programVersions;
    }
}
