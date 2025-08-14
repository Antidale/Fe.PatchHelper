
using Fe.PatchHelper;

var testPath = "/Users/antidale/Downloads/FF4FE.bBAYAAAAAAAAAAAAAXKgCAAAAAAAAABAAAQABBAEwAAAAAQ.H3AL9UBK2T.smc";
var jsonDoc = SeedReader.GetSeedMetadata(testPath);

Console.WriteLine(jsonDoc?.ToString() ?? "null doc");
Console.ReadKey();