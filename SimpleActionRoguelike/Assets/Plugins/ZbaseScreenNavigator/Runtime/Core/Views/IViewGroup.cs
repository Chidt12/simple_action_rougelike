using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZBase.UnityScreenNavigator.Core.Views
{
    public interface IViewGroup : IView
    {
        IReadOnlyList<IView> GetViews();

        bool TryGetView(string name, out IView view);

        void AddView(IView view, bool worldPositionStays = false);

        void AddView(IView view, Action<RectTransform> onAdd);

        void RemoveView(IView view, bool worldPositionStays = false);
    }
}