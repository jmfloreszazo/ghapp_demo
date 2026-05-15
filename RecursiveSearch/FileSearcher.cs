namespace RecursiveSearch;

public static class FileSearcher
{
    /// <summary>
    /// Recursively enumerates files matching <paramref name="pattern"/> under
    /// <paramref name="root"/>, silently skipping directories that are inaccessible.
    /// </summary>
    public static IEnumerable<string> EnumerateFilesSafe(string root, string pattern)
    {
        var queue = new Queue<string>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var dir = queue.Dequeue();

            IEnumerable<string> files = [];
            try { files = Directory.EnumerateFiles(dir, pattern); }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            { Console.Error.WriteLine($"[skipped] {dir}: {ex.Message}"); }

            foreach (var file in files)
                yield return file;

            IEnumerable<string> subDirs = [];
            try { subDirs = Directory.EnumerateDirectories(dir); }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            { Console.Error.WriteLine($"[skipped] {dir}: {ex.Message}"); }

            foreach (var sub in subDirs)
                queue.Enqueue(sub);
        }
    }
}
