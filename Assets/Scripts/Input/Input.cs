using UnityEngine;

namespace Game.Input
{
    public abstract class Input : MonoBehaviour, IInput
    {
        public bool ActiveLeft 
        { 
            get => _activeLeft;
            protected set => _activeLeft = value && !FreezeLeft;
        }
        private bool _activeLeft;

        public bool ActiveRight 
        { 
            get => _activeRight;
            protected set => _activeRight = value && !FreezeRight;
        }
        private bool _activeRight;

        public bool ActiveUp 
        { 
            get => _activeUp;
            protected set => _activeUp = value && !FreezeUp;
        }
        private bool _activeUp;

        public bool ActiveDown 
        { 
            get => _activeDown;
            protected set => _activeDown = value && !FreezeDown;
        }
        private bool _activeDown;

        public bool FreezeLeft { get; private set; }
        public bool FreezeRight { get; private set; }
        public bool FreezeUp { get; private set; }
        public bool FreezeDown { get; private set; }

        public void SetFreezeAll(Obstacle obstacle, bool active)
        {
            SetFreeze(InputType.Left, obstacle, active);
            SetFreeze(InputType.Right, obstacle, active);
            SetFreeze(InputType.Down, obstacle, active);
            SetFreeze(InputType.Up, obstacle, active);
        }

        public void SetFreeze(InputType type, Obstacle obstacle, bool active)
        {
            if (obstacle.IsGuided) 
                return;
            
            switch (type)
            {
                case InputType.Left:
                    FreezeLeft = active;
                    break;

                case InputType.Right:
                    FreezeRight = active;
                    break;

                case InputType.Up:
                    FreezeUp = active;
                    break;

                case InputType.Down:
                    FreezeDown = active;
                    break;
            }
        }
    }
}