using System.CommandLine;
using System.CommandLine.Parsing;

var folderOption = new Option<DirectoryInfo>("--folder")
{
    Description = "Root folder path to search in.",
    Required = true
};
folderOption.Aliases.Add("-f");

var fileNameOption = new Option<string>("--filename")
{
    Description = "File name (or pattern, e.g. *.txt) to search for.",
    Required = true
};
fileNameOption.Aliases.Add("-n");

var rootCommand = new RootCommand("Recursively searches for a file inside a folder.")
{
    folderOption,
    fileNameOption
};

rootCommand.SetAction((ParseResult result) =>
{
    var folder = result.GetValue(folderOption)!;
    var fileName = result.GetValue(fileNameOption)!;

    if (!folder.Exists)
    {
        Console.Error.WriteLine($"Error: folder '{folder.FullName}' does not exist.");
        return 1;
    }

    Console.WriteLine($"Searching for '{fileName}' in '{folder.FullName}'...");
    Console.WriteLine();

    int count = 0;
    foreach (var file in EnumerateFilesSafe(folder.FullName, fileName))
    {
        Console.WriteLine(file);
        count++;
    }

    Console.WriteLine();
    Console.WriteLine($"Found {count} match(es).");
    return 0;
});

return rootCommand.Parse(args).Invoke();

static IEnumerable<string> EnumerateFilesSafe(string root, string pattern)
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
