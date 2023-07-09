using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Manager;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System.Collections.Generic;
using System.Linq;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public struct CollectedArtifact
    {
        public int dataId;
        public ArtifactType artifactType;

        public CollectedArtifact(int dataId, ArtifactType artifactType)
        {
            this.dataId = dataId;
            this.artifactType = artifactType;
        }
    }

    public class MechanicSystemManager
    {
        private List<IArtifactSystem> _artifacts;
        private Stack<CollectedArtifact> _collectedArtifacts;
        private List<ISubscription> _subscriptions;

        public Stack<CollectedArtifact> CollectedArtifacts => _collectedArtifacts;

        public void Init()
        {
            _artifacts = new();
            _collectedArtifacts = new();

            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<InputKeyPressMessage>(OnKeyPress));
        }

        private void OnKeyPress(InputKeyPressMessage keyPressMessage)
        {
            if (keyPressMessage.KeyPressType == KeyPressType.RightMouseButton && GameManager.Instance.CurrentGameStateType == GameStateType.GameplayRunning)
            {
                if (_collectedArtifacts.Count > 0)
                {

                    var collectedArtifact = new CollectedArtifact(0, ArtifactType.None);
                    if (_collectedArtifacts.Count > 0)
                    {
                        collectedArtifact = _collectedArtifacts.Pop();
                    }

                    var artifact = _artifacts.FirstOrDefault(x => x.ArtifactType == collectedArtifact.artifactType && x.DataId == collectedArtifact.dataId);
                    if (artifact.CanTrigger())
                    {
                        var removedCollectedArtifact = artifact.Trigger();
                        if (removedCollectedArtifact)
                        {
                            // Update visual after used.
                            SimpleMessenger.Publish(new UpdateCurrentCollectedArtifactMessage(collectedArtifact.artifactType, collectedArtifact.dataId, UpdatedCurrentCollectedArtifactType.Used));
                        }
                        else
                        {
                            _collectedArtifacts.Push(collectedArtifact);
                        }
                    }
                    else
                    {
                        _collectedArtifacts.Push(collectedArtifact);
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

        public async UniTask ResetForNextStage()
        {
            foreach (var item in _artifacts)
            {
                await item.ResetNewStage();
            }
        }

        public void AddCollectedArtifact(ArtifactType artifactType, int dataId)
        {
            _collectedArtifacts.Push(new CollectedArtifact(dataId, artifactType));
            SimpleMessenger.Publish(new UpdateCurrentCollectedArtifactMessage(artifactType, dataId, UpdatedCurrentCollectedArtifactType.Add));
        }

        public List<ArtifactIdentity> GetCurrentBuffsInGame()
        {
            return _artifacts.Select(x => new ArtifactIdentity(x.ArtifactType, x.Level, x.DataId)).ToList();
        }

        public bool CanAddCollectedArtifact() => _collectedArtifacts.Count < GameplayManager.Instance.GameBalancingConfig.numberStackArtifact;

        public async UniTask AddArtifactystem(IEntityControlData entityData, ArtifactType buffInGameType, int dataId)
        {
            // Add level 1 or increase after time.
            var mechanic = _artifacts.FirstOrDefault(x => x.ArtifactType == buffInGameType);
            if (mechanic == null)
            {
                var dataConfigItem = await DataManager.Config.LoadArtifactDataConfigItem(buffInGameType, 0, dataId);
                var buffInGame = ArtifactSystemFactory.GetArtifactSystem(buffInGameType);

                buffInGame.SetData(dataConfigItem);
                await buffInGame.Init(entityData);
                _artifacts.Add(buffInGame);
                SimpleMessenger.Publish(new UpdateCurrentArtifactMessage(buffInGame, UpdateCurrentArtifactType.Added));
            }
            else
            {
                var currentLevel = mechanic.Level;
                var dataConfigItem = await DataManager.Config.LoadArtifactDataConfigItem(buffInGameType, currentLevel + 1, dataId);

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
