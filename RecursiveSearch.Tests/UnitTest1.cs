namespace RecursiveSearch.Tests;

public class FileSearcherTests : IDisposable
{
    private readonly string _root;

    public FileSearcherTests()
    {
        // Build a temp directory tree:
        //  root/
        //    a.txt
        //    b.log
        //    sub1/
        //      c.txt
        //      d.log
        //    sub2/
        //      nested/
        //        e.txt
        _root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_root, "sub1"));
        Directory.CreateDirectory(Path.Combine(_root, "sub2", "nested"));

        File.WriteAllText(Path.Combine(_root, "a.txt"), "");
        File.WriteAllText(Path.Combine(_root, "b.log"), "");
        File.WriteAllText(Path.Combine(_root, "sub1", "c.txt"), "");
        File.WriteAllText(Path.Combine(_root, "sub1", "d.log"), "");
        File.WriteAllText(Path.Combine(_root, "sub2", "nested", "e.txt"), "");
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void FindsAllTxtFilesRecursively()
    {
        var results = FileSearcher.EnumerateFilesSafe(_root, "*.txt").ToList();

        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.EndsWith("a.txt"));
        Assert.Contains(results, r => r.EndsWith("c.txt"));
        Assert.Contains(results, r => r.EndsWith("e.txt"));
    }

    [Fact]
    public void FindsAllLogFilesRecursively()
    {
        var results = FileSearcher.EnumerateFilesSafe(_root, "*.log").ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.EndsWith("b.log"));
        Assert.Contains(results, r => r.EndsWith("d.log"));
    }

    [Fact]
    public void ExactFileNameMatch_ReturnsOnlyThatFile()
    {
        var results = FileSearcher.EnumerateFilesSafe(_root, "a.txt").ToList();

        Assert.Single(results);
        Assert.EndsWith("a.txt", results[0]);
    }

    [Fact]
    public void NoMatchingFiles_ReturnsEmpty()
    {
        var results = FileSearcher.EnumerateFilesSafe(_root, "*.csv").ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void WildcardStar_ReturnsAllFiles()
    {
        var results = FileSearcher.EnumerateFilesSafe(_root, "*").ToList();

        Assert.Equal(5, results.Count);
    }

    [Fact]
    public void EmptyFolder_ReturnsEmpty()
    {
        var empty = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(empty);
        try
        {
            var results = FileSearcher.EnumerateFilesSafe(empty, "*.txt").ToList();
            Assert.Empty(results);
        }
        finally
        {
            Directory.Delete(empty);
        }
    }

    [Fact]
    public void FindsFileInDeeplyNestedDirectory()
    {
        var results = FileSearcher.EnumerateFilesSafe(_root, "e.txt").ToList();

        Assert.Single(results);
        Assert.Contains("nested", results[0]);
    }
}

