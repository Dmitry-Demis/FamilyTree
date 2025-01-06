using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FamilyTree.Presentation.ViewModels.Services.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }
}