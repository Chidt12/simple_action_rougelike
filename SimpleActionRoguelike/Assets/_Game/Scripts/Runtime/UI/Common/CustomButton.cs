using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    [SerializeField] public GameObject selectedGameObject;

    public int Index { get; set; }
    public Action<int> CustomPointEnterAction { get; set; }

    protected override void Awake()
    {
        base.Awake();
        if (selectedGameObject)
            selectedGameObject.SetActive(false);
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
    }
}
