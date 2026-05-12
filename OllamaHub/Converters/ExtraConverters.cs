using Microsoft.UI.Xaml.Data;

namespace OllamaHub.Converters;

/// <summary>Inverts a bool â€” used for IsEnabled while a task is running.</summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is bool b ? !b : true;
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is bool b ? !b : true;
}

/// <summary>Converts milliseconds (double) to a friendly string like "450ms" or "1.23s".</summary>
public class MsToSecConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double ms) return ms < 1000 ? $"{ms:F0}ms" : $"{ms / 1000:F2}s";
        return value?.ToString() ?? string.Empty;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
