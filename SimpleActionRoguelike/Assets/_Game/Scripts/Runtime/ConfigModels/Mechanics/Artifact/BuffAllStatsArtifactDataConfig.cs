using CsvReader;
using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BuffAllStatsArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.BuffAllStats;

        public string changeFormPrefab;
        public int formId;
        [CsvColumnFormat(ColumnFormat = "buff_{0}")]
        public EquipmentStat[] buffStats;
        public float buffDuration;
    }

    public class BuffAllStatsArtifactDataConfig : ArtifactDataConfig<BuffAllStatsArtifactDataConfigItem>
    {
        protected async override UniTask<(string, string)> GetDescription(IEntityData entityData, BuffAllStatsArtifactDataConfigItem itemData, BuffAllStatsArtifactDataConfigItem previousItemData)
        {
            var currentDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(itemData.ArtifactType));
            var previousDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(previousItemData.ArtifactType));
            return (currentDescription, previousDescription);
        }
    }
}