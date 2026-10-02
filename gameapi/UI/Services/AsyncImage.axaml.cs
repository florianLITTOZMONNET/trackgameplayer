using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace UI.Services;

public class AsyncImage : Image
{
    private static readonly HttpClient _http = new();

    public static readonly StyledProperty<string?> UrlProperty =
        AvaloniaProperty.Register<AsyncImage, string?>(nameof(Url));

    public string? Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    static AsyncImage()
    {
        UrlProperty.Changed.AddClassHandler<AsyncImage>((img, _) => img.LoadImage());
    }

    private async void LoadImage()
    {
        if (string.IsNullOrEmpty(Url)) return;
        try
        {
            var bytes = await _http.GetByteArrayAsync(Url);
            using var ms = new System.IO.MemoryStream(bytes);
            Source = new Bitmap(ms);
        }
        catch { Source = null; }
    }
}