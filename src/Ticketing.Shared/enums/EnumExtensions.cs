using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).First();

        var display = member.GetCustomAttribute<DisplayAttribute>();

        return display?.Name ?? value.ToString();
    }
}
