using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZBase.UnityScreenNavigator.Core.Views
{
    [DisallowMultipleComponent]
    public abstract class Window : View, IViewGroup, IWindow
    {
        private readonly List<IView> views = new();

        private void FindViews()
        {
            views.Clear();

            var count = transform.childCount;

            for (var i = 0; i < count; i++)
            {
                if (transform.GetChild(i).TryGetComponent<IView>(out var view))
                {
                    views.Add(view);
                }
            }
        }

        public IReadOnlyList<IView> GetViews()
        {
            FindViews();
            return views;
        }

        public bool TryGetView(string viewName, out IView view)
        {
            FindViews();

            var count = views.Count;
            view = null;

            for (var i = 0; i < count; i++)
            {
                var item = views[i];

                if (item == null)
                    continue;

                if (string.Equals(item.Name, viewName))
                {
                    view = item;
                    break;
                }
            }

            return view != null;
        }

        public void AddView(IView view, bool worldPositionStays = false)
        {
            if (view == null) return;
            var t = view.RectTransform;
            if (t == null || t.parent == transform) return;
            view.Owner.layer = gameObject.layer;
            t.SetParent(transform, worldPositionStays);
        }

        public virtual void AddView(IView view, Action<RectTransform> onAdd)
        {
            if (view == null)
                return;

            Transform t = view.RectTransform;

            if (t == null)
                return;

            if (t.parent == transform)
            {
                onAdd?.Invoke(view.RectTransform);
                return;
            }

            view.Owner.layer = gameObject.layer;
            t.SetParent(transform, false);
            onAdd?.Invoke(view.RectTransform);
        }

        public void RemoveView(IView view, bool worldPositionStays = false)
        {
            if (view == null) return;
            var t = view.RectTransform;
            if (t == null || t.parent != transform) return;
            t.SetParent(null, worldPositionStays);
        }
    }
}