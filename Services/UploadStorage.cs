namespace MVC_nhaSach.Services;

public static class UploadStorage
{
    public const string RequestPath = "/uploads";

    public static string RootPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MVC_nhaSach",
        "uploads");
}
