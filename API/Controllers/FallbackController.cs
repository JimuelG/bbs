using System;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class FallbackController : Controller
{
    public IActionResult Index()
    {
        var indexPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "index.html"
        );

        if (!System.IO.File.Exists(indexPath))
        {
            return NotFound($"index.html not found at: {indexPath}");
        }

        return PhysicalFile(indexPath, "text/html");
    }
}
