using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System.Collections.Generic;
using System.Linq;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public class MechanicSystemManager
    {
        private List<IArtifactSystem> _artifacts;
        private Stack<ArtifactType> _collectedArtifacts;
        private List<ISubscription> _subscriptions;

        public Stack<ArtifactType> CollectedArtifacts => _collectedArtifacts;

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

                    var artifactType = ArtifactType.None;
                    if (_collectedArtifacts.Count > 0)
                    {
                        artifactType = _collectedArtifacts.Pop();
                    }

                    var artifact = _artifacts.FirstOrDefault(x => x.ArtifactType == artifactType);
                    if (artifact.CanTrigger())
                    {
                        artifact.Trigger();
                        // Update visual after used.
                        SimpleMessenger.Publish(new UpdateCurrentCollectedArtifactMessage(artifactType, UpdatedCurrentCollectedArtifactType.Used));
                    }
                    else
                    {
                        _collectedArtifacts.Push(artifactType);
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
            _collectedArtifacts.Push(artifactType);
            SimpleMessenger.Publish(new UpdateCurrentCollectedArtifactMessage(artifactType, UpdatedCurrentCollectedArtifactType.Add));
        }

        public List<ArtifactIdentity> GetCurrentBuffsInGame()
        {
            return _artifacts.Select(x => new ArtifactIdentity(x.ArtifactType, x.Level)).ToList();
        }

        public bool CanAddCollectedArtifact() => _collectedArtifacts.Count < GameplayManager.Instance.GameBalancingConfig.numberStackArtifact;

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
                SimpleMessenger.Publish(new UpdateCurrentArtifactMessage(buffInGame, UpdateCurrentArtifactType.Added));
            }
            else
            {
                var currentLevel = mechanic.Level;
                var dataConfigItem = await DataManager.Config.LoadArtifactDataConfigItem(buffInGameType, currentLevel + 1);

                mechanic.SetData(dataConfigItem);
                await mechanic.Init(entityData);
                SimpleMessenger.Publish(new UpdateCurrentArtifactMessage(mechanic, UpdateCurrentArtifactType.LevelUp));
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

                SimpleMessenger.Publish(new UpdateCurrentArtifactMessage(mechanic, UpdateCurrentArtifactType.Removed));
            }
        }
    }
}
