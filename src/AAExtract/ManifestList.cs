/// <summary>
/// Represents a list of manifest entries.
/// </summary>
public class ManifestList : List<ManifestEntry>
{
    /// <summary>
    /// Gets the manifest entry with the specified blob ID and blob index.
    /// </summary>
    /// <param name="blobId">The blob ID of the manifest entry.</param>
    /// <param name="blobIdx">The blob index of the manifest entry.</param>
    /// <returns>The manifest entry with the specified blob ID and blob index, or null if not found.</returns>
    public ManifestEntry? GetIdx(int blobId, int blobIdx)
    {
        return this.FirstOrDefault(entry => entry.BlobIdx == blobIdx && entry.BlobId == blobId);
    }
}