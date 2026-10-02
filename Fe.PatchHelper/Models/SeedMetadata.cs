using System.Text.Json.Serialization;
using Fe.PatchHelper.Converters;

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

    [JsonPropertyName("metadata_addr")]
    [JsonConverter(typeof(HexStringToUintConverter))]
    public uint MetadataAddress { get; init; } = 0;

    [JsonPropertyName("metadata_len")]
    [JsonConverter(typeof(HexStringToUintConverter))]
    public uint MedataLength { get; init; } = 0;

    public string VerificationString => string.Join(", ", Verification);

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
