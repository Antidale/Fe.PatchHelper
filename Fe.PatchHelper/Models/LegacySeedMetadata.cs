using System;
using System.Text.Json.Serialization;

namespace Fe.PatchHelper.Models;

public class LegacySeedMetadata
{
    [JsonPropertyName("version")]
    public int[] Version { get; init; } = [];

    [JsonPropertyName("flags")]
    public string Flags { get; init; } = string.Empty;

    [JsonPropertyName("seed")]
    public string Seed { get; init; } = string.Empty;

    [JsonPropertyName("binary_flags")]
    public string BinaryFlags { get; init; } = string.Empty;

    [JsonPropertyName("verification")]
    public List<string> Verification { get; init; } = [];


    public SeedMetadata ToSeedMetadata()
    {
        return new SeedMetadata
        {
            Flags = this.Flags,
            BinaryFlags = this.BinaryFlags,
            Seed = this.Seed,
            Verification = this.Verification,
            Version = $"v{string.Join('.', Version)}"
        };
    }

    public override string ToString()
    {
        return @$"
    version: {Version}
    flags: {Flags}
    binary flags: {BinaryFlags}
    seed: {Seed}";
    }
}
