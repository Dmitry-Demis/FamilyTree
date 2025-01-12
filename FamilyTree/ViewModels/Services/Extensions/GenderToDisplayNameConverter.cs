using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;
using FamilyTree.DAL.Model;

namespace FamilyTree.Presentation.ViewModels.Services.Extensions;

public class GenderToDisplayNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Gender gender) return null;
        var field = gender.GetType().GetField(gender.ToString());
        var attribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute));
        return attribute?.Name ?? gender.ToString(); // Если нет атрибута Display, возвращаем имя
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value; // Оставляем без изменений, можно реализовать логику конвертации обратно
    }
    }