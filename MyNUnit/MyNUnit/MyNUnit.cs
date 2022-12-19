namespace MyNUnit;

using System.Collections.Concurrent;
using System.Reflection;
using Info;

/// <summary>
/// Class that is used to start tests.
/// </summary>
public static class MyNUnit
{
    /// <summary>
    /// Starts all tests by the given path.
    /// </summary>
    /// <param name="path">Path to the assemblies.</param>
    /// <returns>List with information about tests in each assembly.</returns>
    public static List<AssemblyTestInfo> StartAllTests(string path)
    {
        var info = new DirectoryInfo(path);
        var dllFiles = from file in info.GetFiles()
            where file.Extension == ".dll"
            select file;

        var assembliesInfo = new ConcurrentBag<AssemblyTestInfo>();
        Parallel.ForEach(dllFiles, file =>
        {
            var assembly = Assembly.LoadFrom(file.FullName);
            assembliesInfo.Add(AssemblyTestInfo.StartAssemblyTests(assembly));
        });

        return assembliesInfo.ToList();
    }
}