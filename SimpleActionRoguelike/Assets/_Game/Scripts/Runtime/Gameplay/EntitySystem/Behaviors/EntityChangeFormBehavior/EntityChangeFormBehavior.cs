using Cysharp.Threading.Tasks;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityChangeFormBehavior : EntityBehavior
    {
        private ChangeFormComponent[] _components;

        public override UniTask<bool> BuildAsync(IEntityData data, CancellationToken cancellationToken)
        {
            _components = transform.GetComponentsInChildren<ChangeFormComponent>();
            data.ChangeFormEvent += OnChangeForm;

            OnChangeForm(0);
            return base.BuildAsync(data, cancellationToken);
        }

        private void OnChangeForm(int i)
        {
            foreach (var component in _components)
            {
                component.ChangeForm(i);
            }
        }
    }
}