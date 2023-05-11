using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MapGate : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var entityHolder = collision.GetComponent<IEntityHolder>();
            if(entityHolder != null && entityHolder.EntityData.EntityType == EntityType.Hero)
            {
                SimpleMessenger.Publish(new GoToNextLevelMapMessage());
            }
        }
    }

}