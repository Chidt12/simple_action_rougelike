using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using Runtime.Gameplay.Manager;
using Runtime.Manager.Gameplay;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.Balancing
{
    [Serializable]
    public class GameBalancingConfigRoomType
    {
        public int stageNumber;
        public GameplayRoomType roomType;
        public GameplayGateSetupType setupGateType;
    }

    [CreateAssetMenu(fileName = AddressableKeys.GAME_BALANCING_CONFIG, menuName = "Balancing/GameBalancingConfig")]
    public partial class GameBalancingConfig : ScriptableObject
    {
        [HideIf(nameof(cheat))]
        [Header("=== Gen Enemies Config ===")]
        [HideIf(nameof(cheat))] public int minWave = 1; // boss only have 1 wave.
        [HideIf(nameof(cheat))] public int maxWave = 3;
        [HideIf(nameof(cheat))] public int minEnemyTypes = 1;
        [HideIf(nameof(cheat))] public int maxEnemyTypes = 4;
        [HideIf(nameof(cheat))] public int heroLevelConvert = 10;
        [HideIf(nameof(cheat))] public string[] allEnemyIds;
        [HideIf(nameof(cheat))] public string[] allEliteEnemyIds;
        [HideIf(nameof(cheat))] public string[] allBossIds;

        [HideIf(nameof(cheat))]
        [Header("=== Wave Config ===")]
        [HideIf(nameof(cheat))] public int waveTimeInSeconds = 60;

        [HideIf(nameof(cheat))]
        [Header("=== Map Config ===")]
        [HideIf(nameof(cheat))] public MapLevel lobbyMap;
        [HideIf(nameof(cheat))] public MapLevel shopMap;
        [HideIf(nameof(cheat))] public MapLevel[] bossMaps;
        [HideIf(nameof(cheat))] public MapLevel[] maps;

        [HideIf(nameof(cheat))]
        [Header("=== Room Config ===")]
        [HideIf(nameof(cheat))] public GameBalancingConfigRoomType[] setupRoomTypes;
        [HideIf(nameof(cheat))] public int stageIntervalToFaceBoss = 15;
        [HideIf(nameof(cheat))] public int stageEndGame = 90;
        [HideIf(nameof(cheat))] public int numberOfShopStageBeforeBoss = 2; // 1 randomly , 1 before boss stage
        [HideIf(nameof(cheat))] public int numberOfGivingShopItemBeforeBoss = 5;
        [HideIf(nameof(cheat))] public int numberOfGivingArtifactBeforeBoss = 5;

        [HideIf(nameof(cheat))]
        [Header("=== Artifact Config ===")]

        [Header("=== Shop Item Config ===")]

        [HideIf(nameof(cheat))]
        [Header("=== Formula Balancing [ T*(heroPoint^N)+Z ] ===")]
        [HideIf(nameof(cheat))] public float t;
        [HideIf(nameof(cheat))] public float n;
        [HideIf(nameof(cheat))] public float z;

        [Header("==== Cheat ====")]
        public bool cheat;
        [ShowIf(nameof(cheat))] public MapLevel cheatMap;
        [ShowIf(nameof(cheat))] public GameplayRoomType roomType;
        [ShowIf(nameof(cheat))] public StageLoadConfigItem stageLoadConfigItem;

        #region Calculate

        public async UniTask<(MapLevel, StageLoadConfigItem, GameplayRoomType, GameplayGateSetupType)> GetNextStage(int heroPoint, GameplayRoomType roomType, CurrentLoadedStageData currentStageData)
        {
            // Calculate for current stage and predict the next stage too.
            // Order: load map => load current room type => load waves of current map => setup gate type.

            if (cheat)
            {
                return (cheatMap, stageLoadConfigItem, roomType, GameplayGateSetupType.None);
            }
            else
            {
                var havePresetUpRoom = false;
                var gateSetUpType = GameplayGateSetupType.None;
                var preSetUpRoom = setupRoomTypes.FirstOrDefault(x => x.stageNumber == currentStageData.StageNumber);
                if (preSetUpRoom != null)
                {
                    havePresetUpRoom = true;
                    roomType = preSetUpRoom.roomType;
                    gateSetUpType = preSetUpRoom.setupGateType;
                }

                if (roomType == GameplayRoomType.Lobby)
                {
                    // Load Lobby Stage.
                    if (!havePresetUpRoom)
                        gateSetUpType = GameplayGateSetupType.Normal;
                    return (lobbyMap, default, GameplayRoomType.Lobby, gateSetUpType);
                }
                else if (roomType == GameplayRoomType.Shop)
                {
                    // Load Shop Stage.
                    if (!havePresetUpRoom)
                        gateSetUpType = CalculateForGateSetUp(roomType);
                    return (shopMap, default, GameplayRoomType.Shop, gateSetUpType);
                }
                else
                {
                    // Set up room and gate types.
                    if (!havePresetUpRoom)
                    {
                        roomType = CalculateForGameplayRoomType(roomType);
                        gateSetUpType = CalculateForGateSetUp(roomType);
                    }

                    // calculate stage point.
                    var heroLevel = heroPoint / heroLevelConvert;
                    var stagePoint = t * Mathf.Pow(heroPoint, n) + z;

                    if (roomType == GameplayRoomType.Elite || roomType == GameplayRoomType.EliteHaveArtifact)
                    {
                        // Set up for the room of elite.
                        return await SetUpForEliteRoomAsync(stagePoint, heroLevel, gateSetUpType, roomType, currentStageData);
                    }
                    else if (roomType == GameplayRoomType.Boss)
                    {
                        // Set up for the room of boss.
                        return await SetUpForBossRoomAsync(stagePoint, heroLevel, gateSetUpType, roomType, currentStageData);
                    }
                    else
                    {
                        // Set up for the normal room
                        return await SetUpForNormalRoomAsync(stagePoint, heroLevel, gateSetUpType, roomType, currentStageData);
                    }
                }
            }
        }

        private GameplayGateSetupType CalculateForGateSetUp(GameplayRoomType roomType)
        {
            // Setup gate type for get shop item, go to shop, boss room reason | not related to artifact.
            return GameplayGateSetupType.None;
        }

        private GameplayRoomType CalculateForGameplayRoomType(GameplayRoomType roomType)
        {
            // Set up room type for get artifact reason.
            return GameplayRoomType.Normal;
        }

        private string GetConfigAssetName<T>(string id = "")
        {
            if (string.IsNullOrEmpty(id))
                return typeof(T).ToString();
            else
                return typeof(T).ToString() + "_" + id + ".csv";
        }

        #endregion
    }
}
