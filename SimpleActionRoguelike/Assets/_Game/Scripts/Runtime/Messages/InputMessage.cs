using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public enum InputAttackType
    {
        Right,
        Left,
        Up,
        Down
    }

    public readonly struct InputAttackMessage : IMessage
    {
        public readonly InputAttackType ArrowType;

        public InputAttackMessage(InputAttackType arrowType)
        {
            ArrowType = arrowType;
        }
    }

    public readonly struct InputMoveVectorMessage : IMessage
    {
        public readonly Vector2 MoveVector;

        public InputMoveVectorMessage(Vector2 moveVector)
        {
            MoveVector = moveVector;
        }
    }

    public enum KeyPressType
    {
        Back = 0,
        OpenInventory = 1,
        Interact = 2,
        Dash = 3,
        Confirm = 4,
    }
    public readonly struct InputKeyPressMessage : IMessage
    {
        public readonly KeyPressType KeyPressType;

        public InputKeyPressMessage(KeyPressType keyPressType)
        {
            KeyPressType = keyPressType;
        }
    }
}