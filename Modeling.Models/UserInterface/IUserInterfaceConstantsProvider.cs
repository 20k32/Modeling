namespace Modeling.Models.UserInterface
{
    public interface IUserInterfaceConstantsProvider
    {
        public string ApplicationPopupRootName { get; }
        public string DisplayMemberPath { get; }
        public string NavigationFrameName { get; }
        public int AnimationTimeoutMilliseconds { get; }
        public float SlidersStepFrequency { get; }
    }
}
