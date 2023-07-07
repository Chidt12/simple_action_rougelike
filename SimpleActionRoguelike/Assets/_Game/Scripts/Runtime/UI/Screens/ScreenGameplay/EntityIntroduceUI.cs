using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using Runtime.Manager;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class EntityIntroduceUI : MonoBehaviour
    {
        [SerializeField] private AnimatorHolder _animatorHolder;
        [SerializeField] private TextMeshProUGUI _entityName;
        [SerializeField] private Image _icon;

        public async UniTask StartIntroduce(int entityId, EntityType entityType, CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
            GameManager.Instance.SetGameStateType(GameStateType.GameplayIntroduce, true);
            _animatorHolder.Play("start_introduce");
            _entityName.text = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ENTITY, LocalizeKeys.GetEntityName(entityId));
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"{entityId}_introduce_icon"), cancellationToken);
        }

        public void EndIntroduce(Action endAction)
        {
            _animatorHolder.SetEvents(null, () => {
                gameObject.SetActive(false);
                GameManager.Instance.ReturnPreviousGameState();
                endAction?.Invoke();
            });
            _animatorHolder.Play("end_introduce");
        }
    }
}