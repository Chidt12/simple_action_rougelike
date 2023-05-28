using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MapGate : MonoBehaviour
    {
        [Serializable]
        public class MapGateGraphic
        {
            public GameplayRoomType gateType;
            public GameObject graphic;
            public Animator animator;
        }

        [SerializeField] private MapGateGraphic[] _graphics;

        private MapGateGraphic _currentGraphic;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData.EntityType == EntityType.Hero)
            {
                SimpleMessenger.Publish(new SendToGameplayMessage(SendToGameplayType.GoNextStage, _currentGraphic.gateType));
            }
        }

        public void SetUp(GameplayRoomType gateType)
        {
            _currentGraphic = _graphics.FirstOrDefault(x => x.gateType == gateType);
        }

        public void OpenGate()
        {
            _currentGraphic.animator.Play("gate_open");
        }

        public void CloseGate()
        {
            _currentGraphic.animator.Play("gate_close");
        }
    }

}