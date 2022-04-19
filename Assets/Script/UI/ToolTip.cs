using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toolTipText;

    private void OnEnable()
    {
        EventDispatcher.ShowToolTipEvent += Show;
        EventDispatcher.HideToolTipEvent += Hide;
    }

    private void OnDisable()
    {
        EventDispatcher.ShowToolTipEvent -= Show;
        EventDispatcher.HideToolTipEvent -= Hide;
    }


    private void Show(Vector3 position, string text)
    {
        transform.position = position;
        toolTipText.text = text;
    }
    private void Hide()
    {
        toolTipText.text = string.Empty;
    }
}
