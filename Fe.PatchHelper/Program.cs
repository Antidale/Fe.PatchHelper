
using Fe.PatchHelper.Commands;
using Spectre.Console.Cli;


var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<GetFileMetadataCommand>("metadata");
    config.AddCommand<CreatePatchCommand>("create-patch");
});
return await app.RunAsync(args);
