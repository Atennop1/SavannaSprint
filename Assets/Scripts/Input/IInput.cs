namespace Game.Input
{
    public interface IInput
    {
        bool ActiveLeft { get; }
        bool ActiveRight { get; }
        bool ActiveUp { get; }
        bool ActiveDown { get; }

        bool FreezeLeft { get; }
        bool FreezeRight { get; }
        bool FreezeUp { get; }
        bool FreezeDown { get; }

        void SetFreezeAll(Obstacle obstacle, bool active);
        void SetFreeze(InputType type, Obstacle obstacle, bool active);
    }
}