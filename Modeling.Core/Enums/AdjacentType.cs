namespace Modeling.Core.Enums
{
    public enum AdjacentType
    {
        None,
        Inner, // if shape A is subset of shape B
        Outer, // if shape A superset of B (this is helper to not check first case)
        Nearby, // if shape A is nearby B or vise versa
    }
}
