namespace Abstracts.Input
{
    public interface IPlayerInput
    {
        float HorizontalAxis { get; }
        bool IsJumpButtonDown { get; }
        bool IsExitButton { get; }
        bool IsDownButton { get; }
        bool IsInteractButton { get; }
    }
}