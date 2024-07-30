/// <summary>
/// Represents an assembly stored in the assembly store.
/// </summary>
public class AssemblyStoreAssembly
{
    /// <summary>
    /// Gets the offset of the data in the assembly store.
    /// </summary>
    public int DataOffset { get; init; }

    /// <summary>
    /// Gets the size of the data in the assembly store.
    /// </summary>
    public int DataSize { get; init; }

    /// <summary>
    /// Gets or sets the offset of the debug data in the assembly store.
    /// </summary>
    public int DebugDataOffset { get; set; }

    /// <summary>
    /// Gets or sets the size of the debug data in the assembly store.
    /// </summary>
    public int DebugDataSize { get; set; }

    /// <summary>
    /// Gets or sets the offset of the configuration data in the assembly store.
    /// </summary>
    public int ConfigDataOffset { get; set; }

    /// <summary>
    /// Gets or sets the size of the configuration data in the assembly store.
    /// </summary>
    public int ConfigDataSize { get; set; }
}