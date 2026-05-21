using System.Globalization;

namespace AiTutor.Maui.Converters;

public class InvertedBoolConverter : IValueConverter
{
    /// <summary>
    /// 将 bool 取反，用于图片预览与空状态互斥显示。
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : true;
    }

    /// <summary>
    /// 反向转换同样取反。
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : true;
    }
}
