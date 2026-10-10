namespace Modeling.Models.UserInterface
{
    public interface IUserInterfaceConstantsProvider
    {
        public string ApplicationPopupRootName { get; }
        public string DisplayMemberPath { get; }
        public string NavigationFrameName { get; }
        public int AnimationTimeoutMilliseconds { get; }
        public float SlidersStepFrequency { get; }

        public double MinimumTextBoxWidth { get; }

        public float IncrementingStep { get; }
        public float LargeIncrementingStep { get; }
        public int FractionalPartLength { get; }
    }
}
