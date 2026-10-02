using System;
using System.Globalization;
using System.Net.Http;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace UI.Services;

public class UrlToBitmapConverter : IValueConverter
{
    public static readonly UrlToBitmapConverter Instance = new();
    private static readonly HttpClient _http = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrEmpty(url)) return null;
        return LoadAsync(url);
    }

    private static Bitmap? LoadAsync(string url)
    {
        try
        {
            var bytes = _http.GetByteArrayAsync(url).GetAwaiter().GetResult();
            using var ms = new System.IO.MemoryStream(bytes);
            return new Bitmap(ms);
        }
        catch { return null; }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}