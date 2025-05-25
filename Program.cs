using System;
using System.IO;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System.Linq;

class MergeUnitypackage
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: MergeUnityPackage <input1Path> <input2Path> [outputPath]");
            return;
        }
        string input1 = args[0], input2 = args[1];
        string output = args.Length > 2 ? args[2] : Path.Combine(Path.GetDirectoryName(input1), "Merged.unitypackage");
        string dir1 = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(input1));
        string dir2 = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(input2));
        string tmp = output + ".tar.gz";
        // 既存ファイル・フォルダの競合チェック
        bool conflict = false;
        if (Directory.Exists(dir1) || Directory.Exists(dir2) || File.Exists(output) || File.Exists(tmp)) conflict = true;
        if (conflict)
        {
            Console.Write("一時フォルダや出力ファイルが既に存在します。削除して続行してもよいですか？ (y/n): ");
            var key = Console.ReadLine();
            if (key == null || key.ToLower() != "y")
            {
                Console.WriteLine("中断しました。");
                return;
            }
            if (Directory.Exists(dir1)) Directory.Delete(dir1, true);
            if (Directory.Exists(dir2)) Directory.Delete(dir2, true);
            if (File.Exists(output)) File.Delete(output);
            if (File.Exists(tmp)) File.Delete(tmp);
        }
        dir1 = Extract(input1); dir2 = Extract(input2);
        if (!Merge(dir1, dir2))
        {
            Console.WriteLine("Merge failed due to name conflicts.");
            return;
        }
        Compress(dir1, output);
        Console.WriteLine($"Unity packages merged successfully! Output: {output}");
    }
    static string Extract(string pkg)
    {
        string dir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(pkg));
        if (Directory.Exists(dir)) Directory.Delete(dir, true);
        Directory.CreateDirectory(dir);
        using var s = File.OpenRead(pkg);
        using var r = ReaderFactory.Open(s);
        while (r.MoveToNextEntry())
            if (!r.Entry.IsDirectory)
                r.WriteEntryToDirectory(dir, new SharpCompress.Common.ExtractionOptions { ExtractFullPath = true, Overwrite = true });
        return dir;
    }
    static bool Merge(string d1, string d2)
    {
        var f1 = Directory.GetFiles(d1, "*", SearchOption.AllDirectories).Select(f => f[(d1.Length+1)..]).ToHashSet();
        foreach (var f in Directory.GetFiles(d2, "*", SearchOption.AllDirectories))
        {
            var rel = f[(d2.Length+1)..];
            if (f1.Contains(rel)) return false;
            var dest = Path.Combine(d1, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dest));
            File.Copy(f, dest);
        }
        return true;
    }
    static void Compress(string folder, string outpkg)
    {
        string tmp = outpkg + ".tar.gz";
        using (var s = File.Create(tmp))
        using (var w = WriterFactory.Open(s, SharpCompress.Common.ArchiveType.Tar, SharpCompress.Common.CompressionType.GZip))
        {
            w.WriteAll(folder, "*", SearchOption.AllDirectories);
        }
        if (File.Exists(outpkg)) File.Delete(outpkg);
        File.Move(tmp, outpkg, true);
    }
}
