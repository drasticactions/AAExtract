/// <summary>
/// Represents a hash entry in the assembly store.
/// </summary>
public class AssemblyStoreHashEntry
{
    /// <summary>
    /// Gets or sets the hash value of the entry.
    /// </summary>
    public string HashVal { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mapping index of the entry.
    /// </summary>
    public int MappingIndex { get; set; }

    /// <summary>
    /// Gets or sets the local store index of the entry.
    /// </summary>
    public int LocalStoreIndex { get; set; }

    /// <summary>
    /// Gets or sets the store ID of the entry.
    /// </summary>
    public int StoreId { get; set; }
}