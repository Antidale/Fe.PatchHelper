
using Fe.PatchHelper;

var testPath = "/Users/antidale/Downloads/FF4FE.bBAYAAAAAAAAAAAAAXKgCAAAAAAAAABAAAQABBAEwAAAAAQ.H3AL9UBK2T.smc";

var success = SeedReader.TryGetSeedMetadata(testPath, out var jsonDoc);

Console.WriteLine(success ? jsonDoc.ToString() : "failed to read metadata");
Console.ReadKey();