namespace MyNUnit;

using System.Collections.Concurrent;
using System.Reflection;
using Info;

public static class MyNUnit
{
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