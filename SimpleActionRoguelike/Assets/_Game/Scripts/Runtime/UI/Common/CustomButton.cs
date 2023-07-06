using Cysharp.Threading.Tasks;
using Runtime.Manager.Audio;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    [SerializeField] public GameObject selectedGameObject;
    [SerializeField] private string _soundFx;
    [SerializeField] private string _selectedSoundFx;

    public int Index { get; set; }
    public Action<int> CustomPointEnterAction { get; set; }

    protected override void Awake()
    {
        base.Awake();
        if (selectedGameObject)
            selectedGameObject.SetActive(false);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        if (!string.IsNullOrEmpty(_soundFx))
            AudioManager.Instance.PlaySfx(_soundFx).Forget();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        CustomPointEnterAction?.Invoke(Index);
    }

    public virtual void ToggleSelect(bool value)
    {
        if (selectedGameObject)
            selectedGameObject.SetActive(value);

        if (value)
        {
            if (!string.IsNullOrEmpty(_selectedSoundFx))
                AudioManager.Instance.PlaySfx(_selectedSoundFx).Forget();
        }
    }
}
