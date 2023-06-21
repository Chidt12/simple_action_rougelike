using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class IngameBuffItemUI : MonoBehaviour
    {
        [SerializeField] private Button _selectButton;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _level;

        public async UniTask Init(IEntityData entityData, ArtifactIdentity identity, Action<ArtifactIdentity> selectAction)
        {
            var buffInGameDataConfig = await DataManager.Config.LoadBuffInGameDataConfig(identity.artifactType);
            var description = await buffInGameDataConfig.GetDescription(entityData, identity.level);

            _level.text = $"Level {identity.level}";
            _description.text = description;
            _title.text = identity.artifactType.ToString();

            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(() =>
            {
                selectAction?.Invoke(identity);
            });
        }
    }
}
