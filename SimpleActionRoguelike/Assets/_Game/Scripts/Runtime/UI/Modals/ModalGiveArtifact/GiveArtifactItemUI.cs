using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using Runtime.Manager.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class GiveArtifactItemUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Button _selectButton;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _level;

        public async UniTask Init(IEntityData entityData, ArtifactIdentity identity, Action<ArtifactIdentity> selectAction)
        {
            var buffInGameDataConfig = await DataManager.Config.LoadArtifactDataConfig(identity.artifactType);
            var description = await buffInGameDataConfig.GetDescription(entityData, identity.level, identity.dataId);

            _level.text = $"Level {identity.level}";
            _description.text = description.Item1;
            _title.text = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactName(identity.artifactType));

            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(() =>
            {
                selectAction?.Invoke(identity);
            });

            LoadSpriteAsync(identity.artifactType, identity.dataId).Forget();
        }

        private async UniTaskVoid LoadSpriteAsync(ArtifactType artifactType, int dataId)
        {
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"artifact_{(int)artifactType}_{dataId}"), this.GetCancellationTokenOnDestroy());
        }
    }
}
