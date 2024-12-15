using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FamilyTree.Presentation.ViewModels.Services.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNotNullOrWhiteSpace(params string?[] strings) =>
            strings.All(str => !string.IsNullOrWhiteSpace(str));
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }
    }
}
