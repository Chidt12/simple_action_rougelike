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
    public Action OnPointEnter { get; set; }

    protected override void Awake()
    {
        base.Awake();
        if (selectedGameObject)
            selectedGameObject.SetActive(false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (selectedGameObject)
            selectedGameObject.SetActive(true);

        OnPointEnter?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (selectedGameObject)
            selectedGameObject.SetActive(false);
    }
}
