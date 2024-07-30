using System.Text.Json;
using AAExtract.Translations;
using ConsoleAppFramework;

var app = ConsoleApp.Create();
app.Add<Commands>();
app.Run(args);

/// <summary>
/// Commands for AAExtract.
/// </summary>
class Commands
{
    /// <summary>Unpack Xamarin.Android/.NET Android Assembly Blob Files.</summary>
    /// <param name="inputPath">-i, Input File.</param>
    /// <param name="arch">-a, Arch blob type to extract. (Ex. arm64 x64)</param>
    /// <param name="force">-f, Force overwrite the output directory.</param>
    /// <param name="outputPath">-o, Output path for the extracted assemblies.</param>
    [Command("unpack")]
    public void Unpack(string inputPath, string arch, bool force = false, string? outputPath = default)
    {
        var path = outputPath ?? Path.Combine(inputPath, "out");
        DoUnpack(inputPath, arch, force, path);
    }
    
    static int DoUnpack(string inDirectory, string inArch, bool force, string outDirectory)
    {
        bool archAssemblies = false;

        if (force && Directory.Exists(outDirectory))
        {
            Directory.Delete(outDirectory, true);
        }

        if (Directory.Exists(outDirectory))
        {
            Console.WriteLine(Common.OutputDirectoryExists, outDirectory);
            return 3;
        }

        var manifestPath = Path.Combine(inDirectory, AssemblyStoreConstants.FileAssembliesManifest);
        var assembliesPath = Path.Combine(inDirectory, AssemblyStoreConstants.FileAssembliesBlob);

        if (!File.Exists(manifestPath))
        {
            Console.WriteLine(Common.ManifestFileDoesNotExist, manifestPath);
            return 4;
        }
        
        if (!File.Exists(assembliesPath))
        {
            Console.WriteLine(Common.MainAssemblyBlobDoesNotExist, assembliesPath);
            return 4;
        }

        var manifestEntries = ReadManifest(manifestPath);

        var jsonData = new Dictionary<string, object>
        {
            ["stores"] = new List<object>(),
            ["assemblies"] = new List<object>()
        };

        Directory.CreateDirectory(outDirectory);

        var assemblyStore = new AssemblyStore(assembliesPath, manifestEntries);

        if (assemblyStore.HdrLec != assemblyStore.HdrGec)
        {
            archAssemblies = true;
            System.Diagnostics.Debug.WriteLine("Found more assemblies to extract.");
        }

        jsonData = assemblyStore.ExtractAll(jsonData, outDirectory);

        if (archAssemblies)
        {
            var archAssembliesPath = Path.Combine(inDirectory, AssemblyStoreConstants.ARCHITECTURE_MAP[inArch]);
            var archAssemblyStore = new AssemblyStore(archAssembliesPath, manifestEntries, primary: false);
            jsonData = archAssemblyStore.ExtractAll(jsonData, outDirectory);
        }

        File.WriteAllText(AssemblyStoreConstants.FileAssembliesJson, JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true }));

        return 0;
    }

    static ManifestList ReadManifest(string inManifest)
    {
        var manifestList = new ManifestList();
        foreach (var line in File.ReadAllLines(inManifest))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Hash")) continue;

            var splitLine = line.Split();
            splitLine = splitLine.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            manifestList.Add(new ManifestEntry(splitLine[0], splitLine[1], int.Parse(splitLine[2]), int.Parse(splitLine[3]), splitLine[4]));
        }
        return manifestList;
    }
}