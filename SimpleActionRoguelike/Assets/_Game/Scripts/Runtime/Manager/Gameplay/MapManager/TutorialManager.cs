using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _guideText;

    private void Awake()
    {
        _guideText.text = "Left Click To Attack, Space To Dash";
    }

    public void SetText(string text)
    {
        _guideText.text = text;
    }
}
