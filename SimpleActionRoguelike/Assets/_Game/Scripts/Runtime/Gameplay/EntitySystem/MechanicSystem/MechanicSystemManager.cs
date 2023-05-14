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
        private List<IMechanicSystem> _mechanics;

        protected override void Awake()
        {
            base.Awake();
            _mechanics = new();
        }

        public List<BuffInGameIdentity> GetCurrentBuffsInGame()
        {
            var buffsInGame = new List<BuffInGameIdentity>();
            foreach (var item in _mechanics)
            {
                var mechanicItem = (IBuffInGameSystem)item;
                if (mechanicItem != null)
                {
                    var buffIdentity = new BuffInGameIdentity(mechanicItem.BuffInGameType, mechanicItem.Level);
                    buffsInGame.Add(buffIdentity);
                }
            }

            return buffsInGame;
        }

        public async UniTask AddBuffInGameSystem(IEntityData entityData, BuffInGameType buffInGameType)
        {
            // Add level 1 or increase after time.
            var mechanic = _mechanics.FirstOrDefault(x => x is IBuffInGameSystem && ((IBuffInGameSystem)x).BuffInGameType == buffInGameType);
            if (mechanic == null)
            {
                var dataConfigItem = await ConfigDataManager.Instance.LoadBuffInGameDataConfigItem(buffInGameType, 0);
                var buffInGame = BuffInGameSystemFactory.GetBuffInGameSystem(buffInGameType);

                buffInGame.SetData(dataConfigItem);
                await buffInGame.Init(entityData);
                _mechanics.Add(buffInGame);
            }
            else
            {
                var currentLevel = mechanic.Level;
                var dataConfigItem = await ConfigDataManager.Instance.LoadBuffInGameDataConfigItem(buffInGameType, currentLevel + 1);

                ((IBuffInGameSystem)mechanic).SetData(dataConfigItem);
                await mechanic.Init(entityData);
            }
        }

        public void RemoveBuffInGameSystem(IEntityData entityData, BuffInGameType buffInGameType)
        {
            // Remove
            var mechanic = _mechanics.FirstOrDefault(x => x.EntityData.EntityUID == entityData.EntityUID &&  
                                                            x is IBuffInGameSystem && 
                                                            ((IBuffInGameSystem)x).BuffInGameType == buffInGameType);
            if(mechanic != null)
            {
                mechanic.Dispose();
                _mechanics.Remove(mechanic);
            }
        }

        public void Dispose()
        {
            if(_mechanics != null)
            {
                foreach (var mechanic in _mechanics)
                    mechanic.Dispose();

                _mechanics.Clear();
            }
        }
    }
}
