
using Spectre.Console;

namespace Fe.PatchHelper;

public class FlipsHelper
{
    public static async Task<string> CreateBpsPatchAsync(string filePath, string romPath, string flipsPath)
    {
        var outputPatchName = $"{Guid.NewGuid()}.bps";

        using var flips = new System.Diagnostics.Process();
        flips.StartInfo.FileName = flipsPath;
        flips.StartInfo.Arguments = $"{romPath} {filePath} {outputPatchName}";
        flips.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        flips.StartInfo.UseShellExecute = false;
        flips.StartInfo.CreateNoWindow = true;
        flips.StartInfo.RedirectStandardOutput = true;

        flips.Start();

        //consider redirecting error output, too, and capturing that, assuming flips would give error output.
        var output = await flips.StandardOutput.ReadToEndAsync();
        await flips.WaitForExitAsync();
        return outputPatchName;
    }
}
