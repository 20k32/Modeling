using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace Modeling.UI.Behaviors
{
    static class ViewModelsInitializationBehavior
    {
        static bool _shouldExecuteInitialization = true;
        private const string IsSettingsViewModelInitializationEnabledPropertyName = "IsSettingsViewModelInitializationEnabled";
        private const string IsDrawingViewModelInitializationEnabledPropertyName = "IsDrawingViewModelInitializationEnabled";

        public static readonly DependencyProperty IsDrawingViewModelInitializationEnabledProperty =
        DependencyProperty.RegisterAttached(
            IsDrawingViewModelInitializationEnabledPropertyName,
            typeof(IRelayCommand),
            typeof(ViewModelsInitializationBehavior),
            new PropertyMetadata(default, OnIsViewModelInitializationEnabledChanged));

        public static IRelayCommand GetIsDrawingViewModelInitializationEnabled(DependencyObject obj) =>
            (IRelayCommand)obj.GetValue(IsDrawingViewModelInitializationEnabledProperty);

        public static void SetIsDrawingViewModelInitializationEnabled(DependencyObject obj, IRelayCommand value) =>
            obj.SetValue(IsDrawingViewModelInitializationEnabledProperty, value);

        public static readonly DependencyProperty IsSettingsViewModelInitializationEnabledProperty =
        DependencyProperty.RegisterAttached(
            IsSettingsViewModelInitializationEnabledPropertyName,
            typeof(IRelayCommand),
            typeof(ViewModelsInitializationBehavior),
            new PropertyMetadata(default, OnIsViewModelInitializationEnabledChanged));

        private static void OnIsViewModelInitializationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (element.IsLoaded)
                {
                    OnControlLoaded(element, default);
                }
                else
                {
                    element.Loaded -= OnControlLoaded;
                    element.Loaded += OnControlLoaded;
                }
            }
        }

        private static async void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && _shouldExecuteInitialization)
            {
                _shouldExecuteInitialization = false;
                element.Loaded -= OnControlLoaded;

                var settingsViewModelInitializationCommand = GetIsSettingsViewModelInitializationEnabled(element);
                var drawingViewModelInitializationCommand = GetIsDrawingViewModelInitializationEnabled(element);

                await ExecuteCommandAsync(settingsViewModelInitializationCommand);
                await ExecuteCommandAsync(drawingViewModelInitializationCommand);
            }
        }

        public static IRelayCommand GetIsSettingsViewModelInitializationEnabled(DependencyObject obj) =>
            (IRelayCommand)obj.GetValue(IsSettingsViewModelInitializationEnabledProperty);

        public static void SetIsSettingsViewModelInitializationEnabled(DependencyObject obj, IRelayCommand value) =>
            obj.SetValue(IsSettingsViewModelInitializationEnabledProperty, value);

        private static async Task ExecuteCommandAsync(IRelayCommand command)
        {
            if (command is IAsyncRelayCommand asyncRelayCommand)
            {
                await asyncRelayCommand.ExecuteAsync(default);
            }
            else
            {
                command?.Execute(default);
            }
        }
    }
}
