using System;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class Toast : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _displayText;
        [SerializeField]
        private VisualToastType _visualToastType;

        public VisualToastType VisualToastType => _visualToastType;
        private Action<Toast> EndToast { get; set; }

        public void Init(string content, Transform positionTransform, Action<Toast> endToastAction)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup)
                canvasGroup.alpha = 1;
            transform.SetParent(positionTransform);
            transform.position = positionTransform.position;
            gameObject.SetActive(true);
            _displayText.text = content;
            EndToast = endToastAction;
        }

        #region Unity Event

        public void Finishing()
        {
            EndToast?.Invoke(this);
            gameObject.SetActive(false);
        }

        #endregion Unity Event
    }
}
