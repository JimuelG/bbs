using Microsoft.Extensions.FileProviders;

namespace API.Extensions;

public static class StaticFilesExtensions
{
    public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app, string rootPath)
    {
        var certFolder = Path.Combine(rootPath, "wwwroot", "certificates");
        var idsFolder = Path.Combine(rootPath, "wwwroot", "images", "ids");
        var corcernsFolder = Path.Combine(rootPath, "wwwroot", "images", "concerns");
        var profilesFolder = Path.Combine(rootPath, "wwwroot", "images", "profiles");
        var officialsFolder = Path.Combine(rootPath, "wwwroot", "images", "officials");
        var logosFolder = Path.Combine(rootPath, "wwwroot", "images", "logos");
        var signaturesFolder = Path.Combine(rootPath, "wwwroot", "images", "signatures");
        var audioFolder = Path.Combine(rootPath, "wwwroot", "audio");

        if (!Directory.Exists(certFolder)) Directory.CreateDirectory(certFolder);
        if (!Directory.Exists(idsFolder)) Directory.CreateDirectory(idsFolder);
        if (!Directory.Exists(corcernsFolder)) Directory.CreateDirectory(corcernsFolder);
        if (!Directory.Exists(profilesFolder)) Directory.CreateDirectory(profilesFolder);
        if (!Directory.Exists(officialsFolder)) Directory.CreateDirectory(officialsFolder);
        if (!Directory.Exists(logosFolder)) Directory.CreateDirectory(logosFolder);
        if (!Directory.Exists(signaturesFolder)) Directory.CreateDirectory(signaturesFolder);
        if (!Directory.Exists(audioFolder)) Directory.CreateDirectory(audioFolder);

        app.UseStaticFiles();

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
            FileProvider = new PhysicalFileProvider(corcernsFolder),
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

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(logosFolder),
            RequestPath = "/api/images/logos"
        });

        return app;
    }
}
