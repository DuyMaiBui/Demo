using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;

public class GameManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera menuCamera;

    [Header("Canvas")]
    [SerializeField] private Canvas gamePlayCanvas;
    [SerializeField] private Canvas menuCanvas;

    [Header("UI")]
    [SerializeField] private GameObject loadSceneUI;

    [Header("UI Animator")]
    [SerializeField] private Animator loadingAnim;
    [SerializeField] private Animator loadingBrand;

    [Header("Transform")]
    [SerializeField] private Transform itemParent;


    private int loadingAppearID;
    private int loadingDisappearID;

    private bool loading;
    private float angleZ = 0;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
        menuCanvas.enabled = false;
        menuCamera.enabled = false;
        mainCamera.enabled = false;
        gamePlayCanvas.enabled = false;
        // Hash ID
        loadingAppearID = Animator.StringToHash("Appear");
        loadingDisappearID = Animator.StringToHash("Disappear");

        // Assign Event
        EventDispatcher.LoadGameEvent += LoadGame;

    }

    private void OnDisable()
    {
        EventDispatcher.LoadGameEvent -= LoadGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera.enabled = false;
        menuCamera.enabled = true;

        gamePlayCanvas.enabled = false;
        menuCanvas.enabled = true;
        loading = false;
    }

    private void Update()
    {
        if (loading)
        {
            loadSceneUI.transform.eulerAngles = new Vector3(0, 0, angleZ -= 100f * Time.deltaTime);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadGame(int index)
    {
        loadingAnim.SetTrigger(loadingAppearID);
        if (index == 0)
        {
            StartCoroutine(LoadingMenu());
        }
        else if (index == 1)
        {
            StartCoroutine(LoaddingMap());
        }
    }

    IEnumerator LoadingMenu()
    {
        loading = true;
        yield return new WaitForSecondsRealtime(3f);

        StartCoroutine(ClearMap());

        menuCamera.enabled = true;
        menuCanvas.enabled = true;

        mainCamera.enabled = false;
        gamePlayCanvas.enabled = false;

        loadingAnim.SetTrigger(loadingDisappearID);
        loading = false;
    }

    IEnumerator ClearMap()
    {
        EventDispatcher.ClearMap();
        yield return null;
    }

    IEnumerator LoaddingMap()
    {
        loading = true;
        yield return new WaitForSecondsRealtime(3f);
        StartCoroutine(LoadMap());
    }

    IEnumerator LoadMap()
    {
        EventDispatcher.instance.Load();
        yield return null;
        menuCamera.enabled = false;
        menuCanvas.enabled = false;

        mainCamera.enabled = true;
        gamePlayCanvas.enabled = true;

        loadingAnim.SetTrigger(loadingDisappearID);
        loading = false;
    }
}