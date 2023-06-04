using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Manager.UserInput
{
    public class InputManager : PersistentMonoSingleton<InputManager>
    {
        private void Update()
        {
            // Move
            var horizontalValue = Input.GetAxisRaw("Horizontal");
            var verticalValue = Input.GetAxisRaw("Vertical");
            var controlDirection = new Vector2(horizontalValue, verticalValue);
            SimpleMessenger.Publish(new InputMoveVectorMessage(controlDirection));

            // Attack
            if (Input.GetKey(KeyCode.RightArrow))
            {
                SimpleMessenger.Publish(new InputAttackMessage(InputAttackType.Right));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                SimpleMessenger.Publish(new InputAttackMessage(InputAttackType.Left));
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                SimpleMessenger.Publish(new InputAttackMessage(InputAttackType.Down));
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                SimpleMessenger.Publish(new InputAttackMessage(InputAttackType.Up));
            }

            // Interact
            if (Input.GetKeyDown(KeyCode.E))
                SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Interact));
        }
    }
}