# AAExtract - Android Assembly Extract

This program extracts Xamarin.Android / .NET Android DLLs from their blob files, stored in the Android APK. The underlying algorithms we're based on [pyxamstore](https://github.com/jakev/pyxamstore) and ported to .NET.

## How to use

```
Usage: unpack [options...] [-h|--help] [--version]

Unpack Xamarin.Android/.NET Android Assembly Blob Files.

Options:
  -i|--input-path <string>      Input Path to assemblies. (Required)
  -a|--arch <string>            Arch blob type to extract. (Ex. arm64 x64). (Required)
  -f|--force                    Force overwrite the output directory. (Optional)
  -o|--output-path <string?>    Output path for the extracted assemblies. (Default: null)

```

## How it works

Upon extracting your Android APK, you should find an `assemblies` directory. This should contain a list of files, such as...

```
assemblies.arm64_v8a.blob
assemblies.armeabi_v7a.blob
assemblies.blob
assemblies.manifest
```

The `assemblies.manifest` contains a list of files contained within the blobs, with their hashes and sizes. We can extract these with LZ4.

## Third-Party Libraries

- ConsoleAppFramework
- K4os.Compression.LZ4