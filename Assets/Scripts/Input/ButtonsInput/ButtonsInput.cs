namespace Game.Input
{
    public enum InputType
    {
        Left,
        Right,
        Up,
        Down
    }

    public class ButtonsInput : Input
    {
        public void SetInput(InputType type, bool active)
        {
            if (active)
                ActiveDown = ActiveLeft = ActiveRight = ActiveUp = false;
                
            switch (type)
            {
                case InputType.Left:
                    ActiveLeft = active;
                    break;

                case InputType.Right:
                    ActiveRight = active;
                    break;

                case InputType.Up:
                    ActiveUp = active;
                    break;

                case InputType.Down:
                    ActiveDown = active;
                    break;
            }
        }
    }
}