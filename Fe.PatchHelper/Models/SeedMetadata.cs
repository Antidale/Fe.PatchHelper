using System.Text.Json.Serialization;

namespace Fe.PatchHelper.Models;

public class SeedMetadata
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("flags")]
    public string Flags { get; set; } = string.Empty;

    [JsonPropertyName("seed")]
    public string Seed { get; set; } = string.Empty;

    [JsonPropertyName("binary_flags")]
    public string BinaryFlags { get; set; } = string.Empty;

    /// <summary>
    /// will be empty for seeds before 5.0. In theory I should do better for 5.0, but also not a thing for this project.
    /// </summary>
    [JsonPropertyName("objectives")]
    public List<string> Objectives { get; set; } = [];

    public override string ToString()
    {
        return @$"
    version: {Version}
    flags: {Flags}
    binary flags: {BinaryFlags}
    seed: {Seed}";
    }
}
