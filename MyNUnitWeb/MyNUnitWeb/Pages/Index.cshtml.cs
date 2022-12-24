namespace MyNUnitWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;

    private IWebHostEnvironment environment;
    
    public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment)
    {
        this.logger = logger;
        this.environment = environment;
    }

    public string Message { get; private set; } = "PageModel in C#\n";
    
    public List<IFormFile> Files { get; set; }

    public void OnGet()
    {
        Message += $"Server time is {DateTime.Now}";
    }

    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
    {
        string wwwPath = environment.WebRootPath;
        string contentPath = environment.ContentRootPath;
        
        await CreateFiles(files, wwwPath);

        return Page();
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

    public IActionResult OnPostRunTests()
    {
        string wwwPath = environment.WebRootPath;
        string contentPath = environment.ContentRootPath;
        
        var filesPath = Path.Combine(wwwPath, "Uploads");
        var list = MyNUnit.MyNUnit.StartAllTests(filesPath);
        
        DeleteFiles(filesPath);
        
        return Page();
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