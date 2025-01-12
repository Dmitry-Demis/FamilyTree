using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FamilyTree.DAL.Model;

namespace FamilyTree.Presentation.ViewModels.Services.Extensions;

public class GenderToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Gender gender)
        {
            return gender == Gender.Male ? Brushes.Blue : Brushes.Red;
        }
        return Brushes.Black; // Возвращаем чёрный цвет по умолчанию
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}