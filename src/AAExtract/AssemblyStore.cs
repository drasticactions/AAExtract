using AAExtract.Translations;
using K4os.Compression.LZ4;

/// <summary>
/// Represents a store for assemblies, providing methods to read and extract assembly data.
/// </summary>
public class AssemblyStore
{
    /// <summary>
    /// Gets or sets the raw byte data of the assembly store.
    /// </summary>
    private byte[] Raw { get; set; }

    /// <summary>
    /// Gets or sets the file name of the assembly store.
    /// </summary>
    private string FileName { get; set; }

    /// <summary>
    /// Gets or sets the manifest entries associated with the assembly store.
    /// </summary>
    private ManifestList ManifestEntries { get; set; }

    /// <summary>
    /// Gets or sets the magic header of the assembly store.
    /// </summary>
    private byte[] HdrMagic { get; set; }

    /// <summary>
    /// Gets or sets the version of the assembly store.
    /// </summary>
    private int HdrVersion { get; set; }

    /// <summary>
    /// Gets or sets the LEC header value of the assembly store.
    /// </summary>
    public int HdrLec { get; set; }

    /// <summary>
    /// Gets or sets the GEC header value of the assembly store.
    /// </summary>
    public int HdrGec { get; set; }

    /// <summary>
    /// Gets or sets the store ID of the assembly store.
    /// </summary>
    private int HdrStoreId { get; set; }

    /// <summary>
    /// Gets or sets the list of assemblies in the assembly store.
    /// </summary>
    private List<AssemblyStoreAssembly> AssembliesList { get; set; }

    /// <summary>
    /// Gets or sets the list of 32-bit global hash entries in the assembly store.
    /// </summary>
    private List<AssemblyStoreHashEntry>? GlobalHash32 { get; set; }

    /// <summary>
    /// Gets or sets the list of 64-bit global hash entries in the assembly store.
    /// </summary>
    private List<AssemblyStoreHashEntry>? GlobalHash64 { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyStore"/> class.
    /// </summary>
    /// <param name="inFileName">The input file name of the assembly store.</param>
    /// <param name="manifestEntries">The manifest entries associated with the assembly store.</param>
    /// <param name="primary">Indicates whether the assembly store is the primary store.</param>
    public AssemblyStore(string inFileName, ManifestList manifestEntries, bool primary = true)
    {
        ManifestEntries = manifestEntries;
        FileName = Path.GetFileName(inFileName);

        using var blobFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
        Raw = new byte[blobFile.Length];
        blobFile.Read(Raw, 0, Raw.Length);

        blobFile.Seek(0, SeekOrigin.Begin);

        HdrMagic = new byte[4];
        blobFile.Read(HdrMagic, 0, 4);
        if (!HdrMagic.SequenceEqual(AssemblyStoreConstants.AssemblyStoreMagic))
        {
            throw new Exception($"Invalid Magic: {BitConverter.ToString(HdrMagic)}");
        }

        HdrVersion = ReadInt(blobFile);
        if (HdrVersion > AssemblyStoreConstants.AssemblyStoreFormatVersion)
        {
            throw new Exception(
                $"This version is higher than expected! Max = {AssemblyStoreConstants.AssemblyStoreFormatVersion}, got {HdrVersion}");
        }

        HdrLec = ReadInt(blobFile);
        HdrGec = ReadInt(blobFile);
        HdrStoreId = ReadInt(blobFile);

        AssembliesList = new List<AssemblyStoreAssembly>();

        for (int i = 0; i < HdrLec; i++)
        {
            var assembly = new AssemblyStoreAssembly
            {
                DataOffset = ReadInt(blobFile),
                DataSize = ReadInt(blobFile),
                DebugDataOffset = ReadInt(blobFile),
                DebugDataSize = ReadInt(blobFile),
                ConfigDataOffset = ReadInt(blobFile),
                ConfigDataSize = ReadInt(blobFile)
            };

            AssembliesList.Add(assembly);
        }

        if (!primary) return;

        GlobalHash32 = new List<AssemblyStoreHashEntry>();
        for (int i = 0; i < HdrLec; i++)
        {
            var hashEntry = new AssemblyStoreHashEntry
            {
                HashVal = $"0x{ReadInt(blobFile):X8}",
                MappingIndex = ReadInt(blobFile),
                LocalStoreIndex = ReadInt(blobFile),
                StoreId = ReadInt(blobFile)
            };

            GlobalHash32.Add(hashEntry);
        }

        GlobalHash64 = new List<AssemblyStoreHashEntry>();
        for (int i = 0; i < HdrLec; i++)
        {
            var hashEntry = new AssemblyStoreHashEntry
            {
                HashVal = $"0x{ReadLong(blobFile):X16}",
                MappingIndex = ReadInt(blobFile),
                LocalStoreIndex = ReadInt(blobFile),
                StoreId = ReadInt(blobFile)
            };

            GlobalHash64.Add(hashEntry);
        }
    }

    /// <summary>
    /// Reads an integer from the specified file stream.
    /// </summary>
    /// <param name="stream">The file stream to read from.</param>
    /// <returns>The integer value read from the stream.</returns>
    private int ReadInt(FileStream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        return BitConverter.ToInt32(buffer, 0);
    }

    /// <summary>
    /// Reads a long integer from the specified file stream.
    /// </summary>
    /// <param name="stream">The file stream to read from.</param>
    /// <returns>The long integer value read from the stream.</returns>
    private long ReadLong(FileStream stream)
    {
        var buffer = new byte[8];
        stream.Read(buffer, 0, 8);
        return BitConverter.ToInt64(buffer, 0);
    }

    /// <summary>
    /// Extracts all assemblies from the assembly store and writes them to the specified output path.
    /// </summary>
    /// <param name="jsonConfig">The JSON configuration dictionary to update with extracted assembly information.</param>
    /// <param name="outPath">The output path to write the extracted assemblies to.</param>
    /// <returns>The updated JSON configuration dictionary.</returns>
    public Dictionary<string, object> ExtractAll(Dictionary<string, object> jsonConfig, string outPath)
    {
        var storeJson = new Dictionary<string, object>
        {
            [FileName] = new Dictionary<string, object>
            {
                ["header"] = new
                {
                    version = HdrVersion,
                    lec = HdrLec,
                    gec = HdrGec,
                    store_id = HdrStoreId
                }
            }
        };

        for (int i = 0; i < AssembliesList.Count; i++)
        {
            var assembly = AssembliesList[i];
            var entry = ManifestEntries.GetIdx(HdrStoreId, i);
            if (entry == null)
            {
                throw new Exception(string.Format(Common.IndexMissingException, i));
            }
            var assemblyDict = new Dictionary<string, object>
            {
                ["name"] = entry.Name,
                ["store_id"] = entry.BlobId,
                ["blob_idx"] = entry.BlobIdx,
                ["hash32"] = entry.Hash32,
                ["hash64"] = entry.Hash64,
                ["file"] = $"{outPath}/{entry.Name}.dll",
                ["lz4"] = false
            };

            var assemblyData = Raw.Skip(assembly.DataOffset).Take(assembly.DataSize).ToArray();
            if (assemblyData.Take(4).SequenceEqual(AssemblyStoreConstants.CompressedDataMagic))
            {
                assemblyData = DecompressLz4(assemblyData);
                assemblyDict["lz4"] = true;
                assemblyDict["lz4_desc_idx"] = BitConverter.ToInt32(assemblyData, 4);
            }

            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }

            var assemblyName = assemblyDict["file"]?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new Exception(string.Format(Common.AssemblyNameCouldNotBeFound, entry.Name));
            }
            
            File.WriteAllBytes(assemblyName, assemblyData);

            jsonConfig["assemblies"] = jsonConfig.TryGetValue("assemblies", out var value1)
                ? (List<object>)value1
                : [];
            ((List<object>)jsonConfig["assemblies"]).Add(assemblyDict);
        }

        jsonConfig["stores"] =
            jsonConfig.TryGetValue("stores", out var value) ? (List<object>)value : [];
        ((List<object>)jsonConfig["stores"]).Add(storeJson);
        return jsonConfig;
    }
    
    /// <summary>
    /// Decompresses LZ4 compressed data.
    /// </summary>
    /// <param name="compressedData">The compressed data to decompress.</param>
    /// <returns>The decompressed data.</returns>
    /// <exception cref="Exception">Thrown when the decompressed size does not match the expected size.</exception>
    byte[] DecompressLz4(byte[] compressedData)
    {
        var packedPayloadLen = BitConverter.ToInt32(compressedData, 8);
        var compressedPayload = compressedData.Skip(12).ToArray();
        var decompressedPayload = new byte[packedPayloadLen];
        var decompressedSize = LZ4Codec.Decode(compressedPayload, 0, compressedPayload.Length, decompressedPayload, 0, decompressedPayload.Length);
        if (decompressedSize != packedPayloadLen)
        {
            throw new Exception($"Decompressed size mismatch! Expected {packedPayloadLen}, got {decompressedSize}");
        }
        return decompressedPayload;
    }
}