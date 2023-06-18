using UnityEngine;

namespace ZBase.UnityScreenNavigator.Core.Views
{
    public interface IView
    {
        string Name { get; set; }

        bool ActiveSelf { get; set; }

        RectTransform RectTransform { get; }

        float Alpha { get; set; }

        bool Interactable { get; set; }

        CanvasGroup CanvasGroup { get; }
        
        RectTransform Parent { get; }

        GameObject Owner { get; }
    }
}