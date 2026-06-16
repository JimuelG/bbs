using Microsoft.Extensions.FileProviders;

namespace API.Extensions;

public static class StaticFilesExtensions
{
    public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        var contentRoot = env.ContentRootPath;

        Directory.CreateDirectory(webRoot);

        var uploadsRoot = Path.Combine(contentRoot, "uploads");

        var certFolder = Path.Combine(uploadsRoot, "certificates");
        var idsFolder = Path.Combine(uploadsRoot, "images", "ids");
        var concernsFolder = Path.Combine(uploadsRoot, "images", "concerns");
        var profilesFolder = Path.Combine(uploadsRoot, "images", "profiles");
        var officialsFolder = Path.Combine(uploadsRoot, "images", "officials");
        var signaturesFolder = Path.Combine(uploadsRoot, "images", "signatures");
        var audioFolder = Path.Combine(uploadsRoot, "audio");

        var logosFolder = Path.Combine(webRoot, "public", "images", "logos");

        Directory.CreateDirectory(certFolder);
        Directory.CreateDirectory(idsFolder);
        Directory.CreateDirectory(concernsFolder);
        Directory.CreateDirectory(profilesFolder);
        Directory.CreateDirectory(officialsFolder);
        Directory.CreateDirectory(signaturesFolder);
        Directory.CreateDirectory(audioFolder);

        if (Directory.Exists(logosFolder))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
               FileProvider = new PhysicalFileProvider(logosFolder),
               RequestPath = "/api/images/logos" 
            });
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(certFolder),
            RequestPath = "/api/certificates"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(idsFolder),
            RequestPath = "/api/images/ids"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(concernsFolder),
            RequestPath = "/api/images/concerns"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(profilesFolder),
            RequestPath = "/api/images/profiles"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(officialsFolder),
            RequestPath = "/api/images/officials"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(signaturesFolder),
            RequestPath = "/api/images/signatures"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(audioFolder),
            RequestPath = "/api/audio"
        });

        app.UseStaticFiles();

        return app;
    }
}
