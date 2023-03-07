namespace MyNUnit;

using System.Collections.Concurrent;
using System.Reflection;
using global::MyNUnit.Info;

/// <summary>
///     Class that is used to start tests.
/// </summary>
public static class MyNUnit
{
    /// <summary>
    ///     Starts all tests by the given path.
    /// </summary>
    /// <param name="path">Path to the assemblies.</param>
    /// <returns>List with information about tests in each assembly.</returns>
    public static List<AssemblyTestInfo> StartAllTests(string path)
    {
        DirectoryInfo info = new(path);
        var files = info.GetFiles();

        var assemblies = files.Where(CheckIsAssembly).ToList();

        var assembliesInfo = new ConcurrentBag<AssemblyTestInfo>();
        Parallel.ForEach(assemblies, file =>
        {
            var rawAssembly = File.ReadAllBytes(file.FullName);
            Assembly assembly = Assembly.Load(rawAssembly);

            assembliesInfo.Add(AssemblyTestInfo.StartAssemblyTests(assembly));
        });

        return assembliesInfo.ToList();
    }

    private static bool CheckIsAssembly(FileInfo file)
    {
        var isAssembly = true;

        try
        {
            AssemblyName.GetAssemblyName(file.FullName);
        }
        catch (BadImageFormatException)
        {
            isAssembly = false;
        }

        return isAssembly;
    }
}