using System;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System.Linq;

class MergeUnitypackage
{
    static void Main(string[] args)
    {
        // Check if at least two arguments (input1Path and input2Path) are provided
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: MergeUnityPackage <input1Path> <input2Path> [outputPath]");
            return;
        }

        string input1Path = args[0];
        string input2Path = args[1];

        // If outputPath is not provided, set it to the same directory as input1Path with the name 'Merged.unitypackage'
        string outputPath;
        if (args.Length >= 3)
        {
            outputPath = args[2];
        }
        else
        {
            string input1Dir = Path.GetDirectoryName(input1Path);
            outputPath = Path.Combine(input1Dir, "Merged.unitypackage");
        }
        // Step 1: Extract both Input1 and Input2
        string extractedInput1Dir = ExtractUnityPackage(input1Path);
        string extractedInput2Dir = ExtractUnityPackage(input2Path);

        // Step 2: Check for conflicts and copy contents
        if (CheckAndMerge(extractedInput1Dir, extractedInput2Dir))
        {
            // Step 3: Compress the merged folder and rename to .unitypackage
            CompressToUnityPackage(extractedInput1Dir, outputPath);
            Console.WriteLine("Unity packages merged successfully!");
        }
        else
        {
            Console.WriteLine("Merge failed due to name conflicts.");
        }
    }

    static string ExtractUnityPackage(string unityPackagePath)
    {
        string extractDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(unityPackagePath));
        Directory.CreateDirectory(extractDir);

        using (Stream stream = File.OpenRead(unityPackagePath))
        using (var reader = ReaderFactory.Open(stream))
        {
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    reader.WriteEntryToDirectory(extractDir, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                }
            }
        }
        return extractDir;
    }
    static bool CheckAndMerge(string input1Dir, string input2Dir)
    {
        var input1Files = Directory.GetFiles(input1Dir, "*", SearchOption.AllDirectories)
            .Select(f => GetRelativePath(input1Dir, f)).ToHashSet();
        var input2Files = Directory.GetFiles(input2Dir, "*", SearchOption.AllDirectories);

        foreach (var file in input2Files)
        {
            string relativePath = GetRelativePath(input2Dir, file);
            if (input1Files.Contains(relativePath))
            {
                Console.WriteLine($"Conflict detected: {relativePath} exists in both packages.");
                return false;
            }
            // Copy the file if no conflict
            string destPath = Path.Combine(input1Dir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            File.Copy(file, destPath);
        }
        return true;
    }

    public static string GetRelativePath(string basePath, string fullPath)
    {
        Uri baseUri = new Uri(basePath.EndsWith("\\") ? basePath : basePath + "\\");
        Uri fullUri = new Uri(fullPath);
        Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

        return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
    }

    static void CompressToUnityPackage(string folderToCompress, string outputUnityPackagePath)
    {
        string tempTarGzPath = outputUnityPackagePath + ".tar.gz";

        using (Stream stream = File.Create(tempTarGzPath))
        using (var writer = WriterFactory.Open(stream, ArchiveType.Tar, CompressionType.GZip))
        {
            writer.WriteAll(folderToCompress, "*", SearchOption.AllDirectories);
        }

        // Rename the .tar.gz to .unitypackage
        File.Move(tempTarGzPath, outputUnityPackagePath);
    }
}
