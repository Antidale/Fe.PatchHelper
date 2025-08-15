
using Fe.PatchHelper.Commands;
using Spectre.Console.Cli;


var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<GetFileMetadataCommand>("file");
});
return await app.RunAsync(args);
