using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Manager.Data;
using Runtime.Message;
using System.Collections.Generic;
using System.Linq;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public class MechanicSystemManager
    {
        private List<IArtifactSystem> _artifacts;
        private Queue<ArtifactType> _collectedArtifacts;

        private List<ISubscription> _subscriptions;

        public void Init()
        {
            _artifacts = new();
            _collectedArtifacts = new();

            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<InputKeyPressMessage>(OnKeyPress));
        }

        private void OnKeyPress(InputKeyPressMessage keyPressMessage)
        {
            if(keyPressMessage.KeyPressType == KeyPressType.RightMouseButton)
            {
                if(_collectedArtifacts.Count > 0)
                {
                    var artifactType = UsedCurrentCollectedArtifact();
                    var artifact = _artifacts.FirstOrDefault(x => x.ArtifactType == artifactType);
                    if (artifact.CanTrigger())
                    {
                        artifact.Trigger();
                        // Update visual after used.
                    }
                    else
                    {
                        AddCollectedArtifact(artifactType);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_artifacts != null)
            {
                foreach (var buffItem in _artifacts)
                    buffItem.Dispose();
                _artifacts.Clear();
            }
        }

        public void AddCollectedArtifact(ArtifactType artifactType)
        {
            _collectedArtifacts.Enqueue(artifactType);
        }

        public ArtifactType UsedCurrentCollectedArtifact()
        {
            if(_collectedArtifacts.Count > 0)
            {
                return _collectedArtifacts.Dequeue();
            }

            return ArtifactType.None;
        }

        public List<ArtifactIdentity> GetCurrentBuffsInGame()
        {
            return _artifacts.Select(x => new ArtifactIdentity(x.ArtifactType, x.Level)).ToList();
        }

        public async UniTask AddArtifactystem(IEntityData entityData, ArtifactType buffInGameType)
        {
            // Add level 1 or increase after time.
            var mechanic = _artifacts.FirstOrDefault(x => x.ArtifactType == buffInGameType);
            if (mechanic == null)
            {
                var dataConfigItem = await DataManager.Config.LoadArtifactDataConfigItem(buffInGameType, 0);
                var buffInGame = ArtifactSystemFactory.GetArtifactSystem(buffInGameType);

                buffInGame.SetData(dataConfigItem);
                await buffInGame.Init(entityData);
                _artifacts.Add(buffInGame);
            }
            else
            {
                var currentLevel = mechanic.Level;
                var dataConfigItem = await DataManager.Config.LoadArtifactDataConfigItem(buffInGameType, currentLevel + 1);

                mechanic.SetData(dataConfigItem);
                await mechanic.Init(entityData);
            }
        }

        public void RemoveArtifactSystem(IEntityData entityData, ArtifactType buffInGameType)
        {
            // Remove
            var mechanic = _artifacts.FirstOrDefault(x => x.EntityData.EntityUID == entityData.EntityUID &&
                                                            x.ArtifactType == buffInGameType);
            if(mechanic != null)
            {
                mechanic.Dispose();
                _artifacts.Remove(mechanic);
            }
        }
    }
}
