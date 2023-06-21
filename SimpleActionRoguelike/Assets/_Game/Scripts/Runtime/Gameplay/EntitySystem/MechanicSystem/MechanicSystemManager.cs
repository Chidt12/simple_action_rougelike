using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;
using Runtime.Manager.Data;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.EntitySystem
{
    public class MechanicSystemManager
    {
        private List<IArtifactSystem> _buffItems;

        public void Init()
        {
            _buffItems = new();
        }

        public void Dispose()
        {
            if (_buffItems != null)
            {
                foreach (var buffItem in _buffItems)
                    buffItem.Dispose();
                _buffItems.Clear();
            }
        }

        public List<ArtifactIdentity> GetCurrentBuffsInGame()
        {
            return _buffItems.Select(x => new ArtifactIdentity(x.ArtifactType, x.Level)).ToList();
        }

        public async UniTask AddBuffInGameSystem(IEntityData entityData, ArtifactType buffInGameType)
        {
            // Add level 1 or increase after time.
            var mechanic = _buffItems.FirstOrDefault(x => x.ArtifactType == buffInGameType);
            if (mechanic == null)
            {
                var dataConfigItem = await DataManager.Config.LoadBuffInGameDataConfigItem(buffInGameType, 0);
                var buffInGame = ArtifactSystemFactory.GetArtifactSystem(buffInGameType);

                buffInGame.SetData(dataConfigItem);
                await buffInGame.Init(entityData);
                _buffItems.Add(buffInGame);
            }
            else
            {
                var currentLevel = mechanic.Level;
                var dataConfigItem = await DataManager.Config.LoadBuffInGameDataConfigItem(buffInGameType, currentLevel + 1);

                mechanic.SetData(dataConfigItem);
                await mechanic.Init(entityData);
            }
        }

        public void RemoveBuffInGameSystem(IEntityData entityData, ArtifactType buffInGameType)
        {
            // Remove
            var mechanic = _buffItems.FirstOrDefault(x => x.EntityData.EntityUID == entityData.EntityUID &&
                                                            x.ArtifactType == buffInGameType);
            if(mechanic != null)
            {
                mechanic.Dispose();
                _buffItems.Remove(mechanic);
            }
        }
    }
}
