using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderBar : MonoBehaviour
{
    [Header("Bar Setting")]
    [SerializeField] private Image imageValue;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float speedFill;
    Gradient color;

    private float current;

    private void OnEnable()
    {
        EventDispatcher.ChangedFillBarEvent += ChangedValue;
    }

    private void OnDisable()
    {
        EventDispatcher.ChangedFillBarEvent -= ChangedValue;
    }

    public void ChangedValue(string name, float value)
    {
        if (name == gameObject.name)
            StartCoroutine(Change(value));
    }

    IEnumerator Change(float value)
    {
        if (valueText != null)
            valueText.text = value.ToString();
        current = Mathf.MoveTowards(current, value, speedFill * Time.deltaTime);
        imageValue.fillAmount = Mathf.Lerp(imageValue.fillAmount, value, curve.Evaluate(current));
        yield return null;
    }
}
