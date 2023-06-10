using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Manager.UserInput
{
    public class InputManager : MonoSingleton<InputManager>
    {
        private void Update()
        {
            if(GameManager.Instance.CurrentGameStateType == Definition.GameStateType.GameplayRunning)
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
                else if (Input.GetMouseButtonDown(0))
                    SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.LeftMouseButton));
                else if (Input.GetMouseButtonDown(1))
                    SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.RightMouseButton));
                else if (Input.GetKeyDown(KeyCode.Space))
                    SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Dash));

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Right));
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Left));
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Down));
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Up));
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Confirm));

            if (Input.GetKeyDown(KeyCode.Escape))
                SimpleMessenger.Publish(new InputKeyPressMessage(KeyPressType.Back));
        }
    }
}