using System;
using System.IO;
using System.Linq;
using Woof.DeploymentEx;
using Xunit;


public class DeploymentTests {

    /// <summary>
    /// Creates an archive from selected files, unpacks the archive, creates another one from unpacked files and compares archives contents.
    /// </summary>
    [Fact]
    public void ArcDeflateTest() {
        var sourceDirectory = Path.GetFullPath("..\\..\\..\\.."); // Woof.Standard source
        var sourceFiles =
            Directory
            .EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(i => !i.Contains("obj") && !i.Contains("Tests"))
            .OrderBy(i => i)
            .ToArray();
        var targetDirectory = ".packed.fs";
        var archiveAPath = ".packed.archive.a";
        var archiveBPath = ".packed.archive.b";
        using (var arc = new ArcDeflate { BaseDir = sourceDirectory })
            arc.CreateArchive(archiveAPath, sourceFiles);
        using (var arc = new ArcDeflate())
            arc.ExtractArchive(archiveAPath, targetDirectory);
        using (var arc = new ArcDeflate { BaseDir = targetDirectory })
            arc.CreateArchive(archiveBPath, Directory.EnumerateFiles(targetDirectory, "*", SearchOption.AllDirectories).OrderBy(i => i).ToArray());
        using (var arc = new ArcDeflate()) {
            arc.BaseDir = sourceDirectory;
            arc.CreateArchive(archiveAPath, sourceFiles);
            arc.ExtractArchive(archiveAPath, targetDirectory);
            arc.BaseDir = targetDirectory;
            arc.CreateArchive(archiveBPath, Directory.EnumerateFiles(targetDirectory, "*", SearchOption.AllDirectories).OrderBy(i => i).ToArray());
        }
        var a = File.ReadAllBytes(archiveAPath);
        var b = File.ReadAllBytes(archiveBPath);
        Assert.True(a.Length > 0);
        Assert.True(b.Length > 0);
        Assert.True(a.SequenceEqual(b));
        Directory.Delete(targetDirectory, recursive: true);
        File.Delete(archiveAPath);
        File.Delete(archiveBPath);
    }

}