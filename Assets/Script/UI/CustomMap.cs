using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class CustomMap : MonoBehaviour
{
    [Header("Custom Map Window")]
    [SerializeField] private Transform customMapWindow;
    [SerializeField] private Transform mainMenu;

    [Header("Map Size")]
    [SerializeField] private Toggle smallToggle;
    [SerializeField] private Toggle mediumToggle;
    [SerializeField] private Toggle massiveToggle;

    [Header("Seed")]
    [SerializeField] private Toggle randomSeedToggle;
    [SerializeField] private Slider seedSlider;
    [SerializeField] private TMP_InputField seedInputField;

    private void Awake()
    {
        DeactiveCustomMapWindow();
    }

    public void ActiveCustomMapWindow()
    {
        customMapWindow.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.Flash);
        mainMenu.transform.DOScale(0, 0.25f).SetEase(Ease.Flash);
    }

    public void DeactiveCustomMapWindow()
    {
        customMapWindow.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Flash);
        mainMenu.transform.DOScale(1, 0.25f).SetEase(Ease.Flash);
    }

    public void SetMapSize()
    {
        if (smallToggle.isOn)
        {
            EventDispatcher.SetMapSize(100);
        }
        else if (mediumToggle.isOn)
        {
            EventDispatcher.SetMapSize(150);
        }
        else if (massiveToggle.isOn)
        {
            EventDispatcher.SetMapSize(200);
        }
    }

    public void SetRandomSeed()
    {
        EventDispatcher.SetRandomSeed(randomSeedToggle.isOn);
        if(randomSeedToggle.isOn)
        {
            seedSlider.gameObject.SetActive(false);
            seedInputField.gameObject.SetActive(false);
        }
        else
        {
            seedSlider.gameObject.SetActive(true);
            seedInputField.gameObject.SetActive(true);
        }
    }

    public void SetSeedInputField(string seed)
    {
        int seedValue = Int32.Parse(seed);
        if (seedValue <= 0)
            seedValue = 0;
        if (seedValue >= 1000)
            seedValue = 1000;
        seedSlider.value = seedValue;
        EventDispatcher.SetSeed(seedValue);
    }

    public void SetSeedSlider()
    {
        seedInputField.text = ((int)seedSlider.value).ToString();
        EventDispatcher.SetSeed((int)seedSlider.value);
    }
}
