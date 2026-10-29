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
        public float MediumIncrementingStep { get; }
        public float LargeIncrementingStep { get; }
        public int FractionalPartLength { get; }

        public float MaximumCanvasWidth { get; }
        public float MaximumCanvasHeight { get; }

        public float MinimumCanvasWidth { get; }
        public float MinimumCanvasHeight { get; }

        public float MaximumPixelsPerCentimeter { get; }
        public float MinimumPixelsPerCentimeter { get; }
    }
}
