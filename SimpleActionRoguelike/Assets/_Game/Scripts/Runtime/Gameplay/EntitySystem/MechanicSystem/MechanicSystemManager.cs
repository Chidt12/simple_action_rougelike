using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Manager.Data;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.EntitySystem
{
    public class MechanicSystemManager : MonoSingleton<MechanicSystemManager>
    {
        private List<IBuffInGameSystem> _buffItems;

        protected override void Awake()
        {
            base.Awake();
            _buffItems = new();
        }

        public List<BuffInGameIdentity> GetCurrentBuffsInGame()
        {
            return _buffItems.Select(x => new BuffInGameIdentity(x.BuffInGameType, x.Level)).ToList();
        }

        public async UniTask AddBuffInGameSystem(IEntityData entityData, BuffInGameType buffInGameType)
        {
            // Add level 1 or increase after time.
            var mechanic = _buffItems.FirstOrDefault(x => x.BuffInGameType == buffInGameType);
            if (mechanic == null)
            {
                var dataConfigItem = await DataManager.Config.LoadBuffInGameDataConfigItem(buffInGameType, 0);
                var buffInGame = BuffInGameSystemFactory.GetBuffInGameSystem(buffInGameType);

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

        public void RemoveBuffInGameSystem(IEntityData entityData, BuffInGameType buffInGameType)
        {
            // Remove
            var mechanic = _buffItems.FirstOrDefault(x => x.EntityData.EntityUID == entityData.EntityUID &&
                                                            x.BuffInGameType == buffInGameType);
            if(mechanic != null)
            {
                mechanic.Dispose();
                _buffItems.Remove(mechanic);
            }
        }

        public void Dispose()
        {
            if(_buffItems != null)
            {
                foreach (var buffItem in _buffItems)
                    buffItem.Dispose();
                _buffItems.Clear();
            }
        }
    }
}
