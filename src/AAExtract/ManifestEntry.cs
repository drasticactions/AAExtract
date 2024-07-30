/// <summary>
/// Represents an entry in the manifest.
/// </summary>
/// <param name="hash32">The 32-bit hash of the manifest entry.</param>
/// <param name="hash64">The 64-bit hash of the manifest entry.</param>
/// <param name="blobId">The blob ID of the manifest entry.</param>
/// <param name="blobIdx">The blob index of the manifest entry.</param>
/// <param name="name">The name of the manifest entry.</param>
public class ManifestEntry(string hash32, string hash64, int blobId, int blobIdx, string name)
{
    /// <summary>
    /// Gets or sets the 32-bit hash of the manifest entry.
    /// </summary>
    public string Hash32 { get; set; } = hash32;

    /// <summary>
    /// Gets or sets the 64-bit hash of the manifest entry.
    /// </summary>
    public string Hash64 { get; set; } = hash64;

    /// <summary>
    /// Gets or sets the blob ID of the manifest entry.
    /// </summary>
    public int BlobId { get; set; } = blobId;

    /// <summary>
    /// Gets or sets the blob index of the manifest entry.
    /// </summary>
    public int BlobIdx { get; set; } = blobIdx;

    /// <summary>
    /// Gets or sets the name of the manifest entry.
    /// </summary>
    public string Name { get; set; } = name;
}