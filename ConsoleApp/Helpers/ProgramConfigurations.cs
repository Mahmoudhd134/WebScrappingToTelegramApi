using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Helpers;

public class ProgramConfigurations
{
    public static IConfigurationRoot GetConfigurationRoot(string fileName)
    {
        var builder = new ConfigurationBuilder();
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(fileName, optional: false, reloadOnChange: true);
        return builder.Build();
    }
}