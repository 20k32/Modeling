using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Modeling.Models.UserInterface;
using Newtonsoft.Json.Linq;
using System;

namespace Modeling.UI.Resources.Controls.Miscellaneous.IncrementingTextBox
{
    public sealed partial class IncrementingTextBox : UserControl
    {
        const float DEFAULT_INCREMENTING_STEP = 1;
        const float DEFAULT_MINIMUM_INCREMENTING_VALUE = -100;
        const float DEFAULT_MAXIMUM_INCREMENTING_VALUE = 100;
        const bool DEFAULT_IN_COMPACT_MODE = false;

        readonly IUserInterfaceConstantsProvider _userInterfaceConstantsProvider;

        public static readonly DependencyProperty IncrementingValueProperty =
            DependencyProperty.Register(nameof(IncrementingValue),
            typeof(float),
            typeof(IncrementingTextBox),
            new PropertyMetadata(default, OnIncrementingTextBoxIncrementingValuePropertyChagned));

        private static void OnIncrementingTextBoxIncrementingValuePropertyChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IncrementingTextBox control && e?.NewValue is float value
                && e?.NewValue != e?.OldValue)
            {
                control.SetNewValueInUserInterface(control.IncrementingBox, value);
            }
        }

        public float IncrementingValue
        {
            get { return (float)GetValue(IncrementingValueProperty); }
            set { SetValue(IncrementingValueProperty, value); }
        }

        public static readonly DependencyProperty IncrementingStepProperty =
            DependencyProperty.Register(nameof(IncrementingStep),
            typeof(float),
            typeof(IncrementingTextBox),
            new PropertyMetadata(DEFAULT_INCREMENTING_STEP));

        public float IncrementingStep
        {
            get { return (float)GetValue(IncrementingStepProperty); }
            set { SetValue(IncrementingStepProperty, value); }
        }

        public static readonly DependencyProperty MinimumIncrementingValueProperty =
            DependencyProperty.Register(nameof(MinimumIncrementingValue),
            typeof(float),
            typeof(IncrementingTextBox),
            new PropertyMetadata(DEFAULT_MINIMUM_INCREMENTING_VALUE));

        public float MinimumIncrementingValue
        {
            get { return (float)GetValue(MinimumIncrementingValueProperty); }
            set { SetValue(MinimumIncrementingValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumIncrementingValueProperty =
            DependencyProperty.Register(nameof(MaximumIncrementingValue),
            typeof(float),
            typeof(IncrementingTextBox),
            new PropertyMetadata(DEFAULT_MAXIMUM_INCREMENTING_VALUE));

        public float MaximumIncrementingValue
        {
            get { return (float)GetValue(MaximumIncrementingValueProperty); }
            set { SetValue(MaximumIncrementingValueProperty, value); }
        }

        public static readonly DependencyProperty InCompactModeProperty =
            DependencyProperty.Register(nameof(InCompactMode),
            typeof(bool),
            typeof(IncrementingTextBox),
            new PropertyMetadata(DEFAULT_IN_COMPACT_MODE));

        public bool InCompactMode
        {
            get { return (bool)GetValue(InCompactModeProperty); }
            set { SetValue(InCompactModeProperty, value); }
        }

        public IncrementingTextBox()
        {
            _userInterfaceConstantsProvider = Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();

            InitializeComponent();

            SetNewValueInUserInterface(IncrementingBox, IncrementingValue);
        }

        [RelayCommand]
        void DecreaseValue()
        {
            if (ValidateIncrementingValue(IncrementingValue - IncrementingStep))
            {
                IncrementingValue -= IncrementingStep;
            }
            else
            {
                IncrementingValue = MinimumIncrementingValue;
            }
        }

        [RelayCommand]
        void IncreaseValue()
        {
            if (ValidateIncrementingValue(IncrementingValue + IncrementingStep))
            {
                IncrementingValue += IncrementingStep;
            }
            else
            {
                IncrementingValue = MaximumIncrementingValue;
            }
        }

        private void OnIncrementingTextBoxTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (float.TryParse(textBox.Text, out var newIncrementingValue)
                    && ValidateTextInputIncrementingValue(newIncrementingValue))
                {
                    IncrementingValue = newIncrementingValue;
                }
                else
                {
                    if (IncrementingValue != 0)
                    {
                        IncrementingValue = 0;
                    }
                    else
                    {
                        SetNewValueInUserInterface(textBox, IncrementingValue);
                    }
                }
            }
        }

        private void SetNewValueInUserInterface(TextBox textBox, float value)
        {
            textBox.TextChanged -= OnIncrementingTextBoxTextBoxTextChanged;

            try
            {
                if (value == float.NegativeZero)
                {
                    value = 0;
                }

                textBox.Text = MathF.Round(value, _userInterfaceConstantsProvider.FractionalPartLength)
                    .ToString();
            }
            finally
            {
                textBox.TextChanged -= OnIncrementingTextBoxTextBoxTextChanged;
                textBox.TextChanged += OnIncrementingTextBoxTextBoxTextChanged;
            }
        }

        private bool ValidateIncrementingValue(float newValue)
            => newValue > MinimumIncrementingValue
            && newValue < MaximumIncrementingValue;

        private bool ValidateTextInputIncrementingValue(float newValue)
            => newValue >= MinimumIncrementingValue
            && newValue <= MaximumIncrementingValue;

        private void OnIncrementingTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void OnIncrementingBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Select(0, 0);
            }
        }

        private void OnIncrementingTextBoxPointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as UIElement);
            var delta = point.Properties.MouseWheelDelta;

            if (delta > 0)
            {
                IncreaseValue();
            }
            else if (delta < 0)
            {
                DecreaseValue();
            }
        }
    }
}
