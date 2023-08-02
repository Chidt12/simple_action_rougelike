using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using Runtime.Gameplay.Manager;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public int numberStackArtifact = 4;

        [HideIf(nameof(cheat))]
        [Header("=== Gen Enemies Config ===")]
        [HideIf(nameof(cheat))] public int minWave = 1; // boss only have 1 wave.
        [HideIf(nameof(cheat))] public int maxWave = 3;
        [HideIf(nameof(cheat))] public int minEnemyTypes = 1;
        [HideIf(nameof(cheat))] public int maxEnemyTypes = 4;
        [HideIf(nameof(cheat))] public float maxSpawnAwayFromHero = 8;
        [HideIf(nameof(cheat))] public float minSpawnAwayFromHero = 3.5f;
        [HideIf(nameof(cheat))] public int heroLevelConvert = 10;
        [HideIf(nameof(cheat))] public string[] allEnemyIds;
        [HideIf(nameof(cheat))] public string[] allEliteEnemyIds;
        [HideIf(nameof(cheat))] public string[] allBossIds;

        [HideIf(nameof(cheat))]
        [Header("=== Wave Config ===")]
        [HideIf(nameof(cheat))] public int waveTimeInSeconds = 60;

        [HideIf(nameof(cheat))]
        [Header("=== Map Config ===")]
        [HideIf(nameof(cheat))] public MapLevelScriptableObject lobbyMap;
        [HideIf(nameof(cheat))] public MapLevelScriptableObject tutorialMap;
        [HideIf(nameof(cheat))] public MapLevelScriptableObject shopMap;
        [HideIf(nameof(cheat))] public MapLevelScriptableObject[] bossMaps;
        [HideIf(nameof(cheat))] public MapLevelScriptableObject[] maps;

        [HideIf(nameof(cheat))]
        [Header("=== Room Config ===")]
        [HideIf(nameof(cheat))] public GameBalancingConfigRoomType[] setupRoomTypes;
        [HideIf(nameof(cheat))] public int stageIntervalToFaceBoss = 15;
        [HideIf(nameof(cheat))] public int stageEndGame = 90;
        [HideIf(nameof(cheat))] public int numberOfGivingShopItemBeforeBoss = 5;
        [HideIf(nameof(cheat))] public int numberOfGivingArtifactBeforeBoss = 5;
        [HideIf(nameof(cheat))] public float eliteFactor = 1.5f;
        [HideIf(nameof(cheat))] public float bossFactor = 2f;

        [HideIf(nameof (cheat))]
        [Header("==== Tutorial Config ===")]
        [HideIf(nameof(cheat))] public StageLoadConfigItem tutorialStageLoadConfigItem;

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
        [ShowIf(nameof(cheat))] public int cheatStageEndGame;
        [ShowIf(nameof(cheat))] public MapLevelScriptableObject cheatMap;
        [ShowIf(nameof(cheat))] public GameplayRoomType[] roomTypes;
        [ShowIf(nameof(cheat))] public StageLoadConfigItem[] stageLoadConfigItems;

        #region Calculate

        public int StageEndGame => !cheat ? stageEndGame : cheatStageEndGame;


        public async UniTask<(MapLevelScriptableObject, StageLoadConfigItem, GameplayRoomType, GameplayGateSetupType)> GetNextStage(int heroPoint, GameplayRoomType roomType, CurrentLoadedStageData currentStageData)
        {
            // Calculate for current stage and predict the next stage too.
            // Order: load map => load current room type => load waves of current map => setup gate type.

            if (cheat)
            {
                var index = currentStageData.StageNumber % roomTypes.Length;

                return (cheatMap, stageLoadConfigItems[index], this.roomTypes[index], GameplayGateSetupType.None);
            }
            else
            {
                var havePresetUpRoom = false;
                var gateSetUpType = GameplayGateSetupType.None;

                if(roomType != GameplayRoomType.TutorialStage) 
                {
                    var preSetUpRoom = setupRoomTypes.FirstOrDefault(x => x.stageNumber == currentStageData.StageNumber);
                    if (preSetUpRoom != null)
                    {
                        havePresetUpRoom = true;
                        roomType = preSetUpRoom.roomType;
                        gateSetUpType = preSetUpRoom.setupGateType;
                    }
                }

                if (roomType == GameplayRoomType.Lobby)
                {
                    // Load Lobby Stage.
                    if (gateSetUpType == GameplayGateSetupType.None)
                        gateSetUpType = GameplayGateSetupType.Normal;
                    return (lobbyMap, default, GameplayRoomType.Lobby, gateSetUpType);
                }
                else if (roomType == GameplayRoomType.TutorialStage)
                {
                    // Load Tutorial Stage.
                    if (gateSetUpType == GameplayGateSetupType.None)
                        gateSetUpType = CalculateForGateSetUp(roomType, currentStageData);
                    return (tutorialMap, tutorialStageLoadConfigItem, GameplayRoomType.TutorialStage, gateSetUpType);
                }
                else if (roomType == GameplayRoomType.Shop)
                {
                    // Load Shop Stage.
                    if (gateSetUpType == GameplayGateSetupType.None)
                        gateSetUpType = CalculateForGateSetUp(roomType, currentStageData);
                    return (shopMap, default, GameplayRoomType.Shop, gateSetUpType);
                }
                else
                {
                    // Set up room and gate types.
                    if (!havePresetUpRoom)
                    {
                        roomType = await CalculateForGameplayRoomType(roomType, currentStageData);
                    }

                    if(gateSetUpType == GameplayGateSetupType.None)
                        gateSetUpType = CalculateForGateSetUp(roomType, currentStageData);

                    // calculate stage point.
                    var heroLevel = heroPoint / heroLevelConvert;
                    var stagePoint = t * Mathf.Pow(heroPoint, n) + z;

                    if (roomType == GameplayRoomType.ElitePower || roomType == GameplayRoomType.EliteSpeed)
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

        private GameplayGateSetupType CalculateForGateSetUp(GameplayRoomType roomType, CurrentLoadedStageData currentStageData)
        {
            // Setup gate type for get shop item, go to shop, boss room reason | not related to artifact.
            var countToBossStage = currentStageData.CountToBossStage;

            // Check boss stage.
            if(countToBossStage >= stageIntervalToFaceBoss)
            {
                if (roomType == GameplayRoomType.Shop)
                    return GameplayGateSetupType.Boss;
                else
                    return GameplayGateSetupType.Shop;
            }
            else
            {
                var numberOfEnteredTheShop = currentStageData.GetNumberOfGateSetUpPassed(GameplayGateSetupType.NormalAndShop);
                if(numberOfEnteredTheShop == 0 && countToBossStage >= stageIntervalToFaceBoss / 2)
                {
                    // Load Shop In the middle of the boss interval
                    return GameplayGateSetupType.NormalAndShop;
                }

                var numberOfEnteredElite = currentStageData.GetNumberOfGateSetUpPassed(GameplayGateSetupType.Elite);

                // Giving shop chance => dont have to be enough limit.
                if(numberOfEnteredElite < numberOfGivingShopItemBeforeBoss)
                {
                    var random = Random.Range(0, 2);
                    if(random > 0)
                        return GameplayGateSetupType.Elite;
                    else
                        return GameplayGateSetupType.Normal;
                }

                return GameplayGateSetupType.Normal;
            }
        }

        private async UniTask<GameplayRoomType> CalculateForGameplayRoomType(GameplayRoomType roomType, CurrentLoadedStageData currentStageData)
        {
            // Set up room type for get artifact reason.

            var numberOfArtifact = currentStageData.GetNumberRoomPassed(GameplayRoomType.NormalHaveArtifact);
            if (numberOfArtifact < numberOfGivingArtifactBeforeBoss)
            {
                // Giving shop chance => must have to be enough limit.
                var lackNumber = numberOfGivingArtifactBeforeBoss - numberOfArtifact;
                if(currentStageData.StageNumber + lackNumber >= stageIntervalToFaceBoss)
                {
                    if (roomType == GameplayRoomType.Normal)
                        return GameplayRoomType.NormalHaveArtifact;
                }
                else
                {
                    var random = Random.Range(0, 2);
                    if (random > 0)
                    {
                        if (roomType == GameplayRoomType.Normal)
                            return GameplayRoomType.NormalHaveArtifact;
                    }
                }
            }

            if(roomType == GameplayRoomType.NormalHaveArtifact)
            {
                var items = await DataManager.Config.LoadCurrentSuitableArtifactItems(GameplayManager.Instance.CurrentBuffInGameItems, GameplayManager.NUMBER_OF_SELECT_ARTIFACT);
                if (items.Count < GameplayManager.NUMBER_OF_SELECT_ARTIFACT)
                    return GameplayRoomType.Normal;
            }

            return roomType;
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
