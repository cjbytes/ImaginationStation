using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace OllamaHub.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is true ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is Visibility v && v == Visibility.Visible;
}

public class BoolToInverseVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is false ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is Visibility.Collapsed;
}

public class BoolInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is bool b && !b;
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is bool b && !b;
}

public class ZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is int i && i == 0 ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

public class FloatToLabelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var label = parameter?.ToString() ?? "Value";
        return value is float f ? $"{label}: {f:F2}" : label;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>Formats a DateTime as a friendly relative string.</summary>
public class DateToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not DateTime dt) return string.Empty;
        var diff = DateTime.Now - dt;
        return diff.TotalMinutes < 1   ? "Just now"
             : diff.TotalHours   < 1   ? $"{(int)diff.TotalMinutes}m ago"
             : diff.TotalDays    < 1   ? $"{(int)diff.TotalHours}h ago"
             : diff.TotalDays    < 7   ? $"{(int)diff.TotalDays}d ago"
             : dt.ToString("MMM d, yyyy");
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}

/// <summary>Formats message count as "N messages".</summary>
public class MessageCountConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is int i ? $"{i} message{(i == 1 ? "" : "s")}" : string.Empty;
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
