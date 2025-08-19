using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Fe.PatchHelper.Models;

public class SeedMetadata
{
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    [JsonPropertyName("flags")]
    public string Flags { get; init; } = string.Empty;

    [JsonPropertyName("seed")]
    public string Seed { get; init; } = string.Empty;

    [JsonPropertyName("binary_flags")]
    public string BinaryFlags { get; init; } = string.Empty;

    [JsonPropertyName("verification")]
    public List<string> Verification { get; set; } = [];

    public string VerificationString => string.Join(", ", Verification);

    /// <summary>
    /// will be empty for seeds before 5.0. In theory I should do better for 5.0, but also not a thing for this project.
    /// </summary>
    [JsonPropertyName("objectives")]
    public List<JsonObject> Objectives { get; init; } = [];

    public override string ToString()
    {
        return @$"
    version: {Version}
    flags: {Flags}
    binary flags: {BinaryFlags}
    seed: {Seed}
    verification: {VerificationString}";
    }
}
