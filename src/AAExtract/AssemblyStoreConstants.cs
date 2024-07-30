// <copyright file="AssemblyStoreConstants.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace AAExtract;

/// <summary>
/// Provides constants used in the assembly store.
/// </summary>
public static class AssemblyStoreConstants
{
    /// <summary>
    /// The file name for the assemblies manifest.
    /// </summary>
    public const string FileAssembliesManifest = "assemblies.manifest";

    /// <summary>
    /// The file name for the assemblies JSON configuration.
    /// </summary>
    public const string FileAssembliesJson = "assemblies.json";

    /// <summary>
    /// The format version of the assembly store.
    /// </summary>
    public const int AssemblyStoreFormatVersion = 1;

    /// <summary>
    /// The file name for the assemblies blob.
    /// </summary>
    public const string FileAssembliesBlob = "assemblies.blob";

    /// <summary>
    /// The magic header for the assembly store.
    /// </summary>
    public static readonly byte[] AssemblyStoreMagic = "XABA"u8.ToArray();

    /// <summary>
    /// The magic header for compressed data in the assembly store.
    /// </summary>
    public static readonly byte[] CompressedDataMagic = "XALZ"u8.ToArray();

    /// <summary>
    /// A dictionary mapping architecture names to their corresponding assemblies blob file names.
    /// </summary>
    public static readonly Dictionary<string, string> Architecturemap = new Dictionary<string, string>
    {
        { "arm", FileAssembliesBlobArm },
        { "arm64", FileAssembliesBlobArm64 },
        { "x86", FileAssembliesBlobX86 },
        { "x86_64", FileAssembliesBlobX8664 },
    };

    /// <summary>
    /// The file name for the ARM architecture assemblies blob.
    /// </summary>
    private const string FileAssembliesBlobArm = "assemblies.armeabi_v7a.blob";

    /// <summary>
    /// The file name for the ARM64 architecture assemblies blob.
    /// </summary>
    private const string FileAssembliesBlobArm64 = "assemblies.arm64_v8a.blob";

    /// <summary>
    /// The file name for the x86 architecture assemblies blob.
    /// </summary>
    private const string FileAssembliesBlobX86 = "assemblies.x86.blob";

    /// <summary>
    /// The file name for the x86_64 architecture assemblies blob.
    /// </summary>
    private const string FileAssembliesBlobX8664 = "assemblies.x86_64.blob";
}