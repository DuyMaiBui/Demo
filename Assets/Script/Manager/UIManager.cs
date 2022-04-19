using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject tab;
    [SerializeField] private GameObject gameControl;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject craft;
    [SerializeField] private GameObject inventory;

    [Header("Pause")]
    [SerializeField] private Image pauseImage;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Button pauseButton;

    [Header("Exit Button")]
    [SerializeField] private Button exitCraftButton;
    [SerializeField] private Button exitInventoryButton;
    [SerializeField] private Button exitMiniMapButton;

    [Header("Window Button")]
    [SerializeField] private GameObject[] buttonTab;

    [Space(10)]
    [SerializeField] private string floatTextKey;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI fpsShow;

    private InputSetting _input;

    private bool activeTab = false;
    private bool pause = false;

    private float poolTime = 1f;
    private float time;
    private float frameCount;

    private void Awake()
    {
        // Button Event
        pauseButton.onClick.AddListener(delegate { PauseState(); });

        exitInventoryButton.onClick.AddListener(delegate { DeactiveWindow(inventory); });
        exitCraftButton.onClick.AddListener(delegate { DeactiveWindow(craft); EventDispatcher.ChangeDetailRecipeState(false); });
        exitMiniMapButton.onClick.AddListener(delegate { DeactiveWindow(miniMap); });

        AddEventTrigger.AddEvent(buttonTab[0], EventTriggerType.PointerClick, delegate { DeactiveTab(); DeactiveWindow(miniMap); DeactiveWindow(craft); ActiveWindow(inventory); });
        AddEventTrigger.AddEvent(buttonTab[1], EventTriggerType.PointerClick, delegate { DeactiveTab(); DeactiveWindow(miniMap); DeactiveWindow(inventory); ActiveWindow(craft); });
        AddEventTrigger.AddEvent(buttonTab[2], EventTriggerType.PointerClick, delegate { DeactiveTab(); });
        AddEventTrigger.AddEvent(buttonTab[3], EventTriggerType.PointerClick, delegate { DeactiveTab(); });
        AddEventTrigger.AddEvent(buttonTab[4], EventTriggerType.PointerClick, delegate { DeactiveTab(); DeactiveWindow(inventory); DeactiveWindow(craft); EventDispatcher.SetPlayerMiniMap(); ActiveWindow(miniMap); });

        EventDispatcher.SetFloatTextEvent += SetText;
    }

    private void OnDisable()
    {
        EventDispatcher.SetFloatTextEvent -= SetText;
    }

    private void Start()
    {
        _input = InputSetting.instance;
        DeactiveTab();
        DeactiveGameControl();
        DeactiveWindow(miniMap);
        DeactiveWindow(craft);
        DeactiveWindow(inventory);
    }

    private void Update()
    {
        TabState();
        ShowFPS();
    }

    private void ShowFPS()
    {
        time += Time.deltaTime;
        frameCount++;
        if (time >= poolTime)
        {
            int framRate = Mathf.RoundToInt(frameCount / time);
            fpsShow.text = "FPS: " + framRate.ToString();
            time -= poolTime;
            frameCount = 0;
        }
    }

    private void SetText(string text)
    {
        StartCoroutine(SetFloatText(text));
    }

    private IEnumerator SetFloatText(string text)
    {
        var go = ObjectPoolManager.instance.InstanceGameObject(floatTextKey);
        go.transform.SetParent(transform, false);
        var floatText = go.GetComponent<TextMeshProUGUI>();
        floatText.text = text;
        floatText.transform.DOLocalMoveY(200f, 1f);
        floatText.transform.DOScale(Vector3.one * 0.5f, 1f);
        yield return new WaitForSeconds(1f);
        go.transform.localPosition = new Vector2(0, 50);
        go.transform.localScale = Vector3.one;
        ObjectPoolManager.instance.DeactiveGameObject(floatTextKey, go);
    }

    // Window

    public void ActiveWindow(GameObject _gameObject)
    {
        _input.openUI = true;
        _gameObject.transform.DOScaleY(1, 0.25f).SetEase(Ease.Flash);
    }

    public void DeactiveWindow(GameObject _gameObject)
    {
        _input.openUI = false;
        _gameObject.transform.DOScaleY(0, 0.25f).SetEase(Ease.Flash);
    }

    public void PauseState()
    {
        pause = !pause;
        if (pause)
        {
            ActiveGameControl();
        }
        else
        {
            DeactiveGameControl();
        }
    }

    public void ActiveGameControl()
    {
        _input.openUI = true;
        pause = true;
        pauseImage.sprite = playSprite;
        gameControl.SetActive(true);
        gameControl.transform.DOScaleY(1f, 0.25f).SetEase(Ease.Flash);
    }

    public void DeactiveGameControl()
    {
        _input.openUI = false;
        pause = false;
        pauseImage.sprite = pauseSprite;
        gameControl.transform.DOScaleY(0f, 0.25f).SetEase(Ease.Flash);
        gameControl.SetActive(false);
    }

    public void TabState()
    {
        if (_input.openTab)
        {
            activeTab = !activeTab;
            _input.openTab = !_input.openTab;
            if (activeTab)
            {
                ActiveTab();
            }
            else
            {
                DeactiveTab();
            }
        }
    }

    public void ActiveTab()
    {
        activeTab = true;
        tab.SetActive(true);
        tab.transform.DOScale(Vector3.one, 0.25f);
    }

    public void DeactiveTab()
    {
        activeTab = false;
        tab.transform.DOScale(Vector3.one * 0.75f, 0.25f);
        tab.SetActive(false);
    }
}
