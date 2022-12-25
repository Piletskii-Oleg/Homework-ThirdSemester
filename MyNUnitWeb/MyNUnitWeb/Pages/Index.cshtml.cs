namespace MyNUnitWeb.Pages;

using Data;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;

    private readonly TestInfoDbContext infoContext;

    private IWebHostEnvironment environment;
    
    public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment, TestInfoDbContext infoContext)
    {
        this.logger = logger;
        this.environment = environment;
        this.infoContext = infoContext;
    }

    public IList<TestInfo> TestInfos { get; private set; } = new List<TestInfo>();

    public async Task OnGetAsync()
    {
        TestInfos = await infoContext.TestsInfo
            .Include(testsInfo => testsInfo.AssembliesTestInfo)
            .ThenInclude(assemblyInfo => assemblyInfo.ClassesInfo)
            .ThenInclude(classInfo => classInfo.MethodsInfo)
            .OrderBy(info => info.TestInfoId).ToListAsync();
    }
    
    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
    {
        string wwwPath = environment.WebRootPath;

        await CreateFiles(files, wwwPath);

        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostRunTests()
    {
        string wwwPath = environment.WebRootPath;
        string contentPath = environment.ContentRootPath;
        
        var filesPath = Path.Combine(wwwPath, "Uploads");
        var list = MyNUnit.MyNUnit.StartAllTests(filesPath);

        if (list.Count != 0)
        {
            var info = new TestInfo { AssembliesTestInfo = list };
            infoContext.TestsInfo.Add(info);
            await infoContext.SaveChangesAsync();
        }

        DeleteFiles(filesPath);

        return RedirectToPage("./Index");
    }

    private static async Task CreateFiles(List<IFormFile> files, string path)
    {
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var filePath = Path.Combine(path, "Uploads");

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                await using var stream = System.IO.File.Create(Path.Combine(filePath, Path.GetRandomFileName()));
                await file.CopyToAsync(stream);
            }
        }
    }
    
    private void DeleteFiles(string path)
    {
        var files = Directory.GetFiles(path);
        foreach (var fileName in files)
        {
            System.IO.File.Delete(fileName);
        }
    }
}