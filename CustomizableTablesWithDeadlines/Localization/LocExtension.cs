using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using CustomizableTablesWithDeadlines.Services.Interfaces;

namespace CustomizableTablesWithDeadlines.Localization;

[MarkupExtensionReturnType(typeof(BindingExpression))]
public class LocExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
            return string.Empty;

        var binding = new Binding($"[{Key}]")
        {
            Source = App.Strings,
            Mode = BindingMode.OneWay
        };

        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target)
        {
            if (target.TargetObject is not DependencyObject)
                return binding.ProvideValue(serviceProvider);
        }

        return binding.ProvideValue(serviceProvider);
    }
}
