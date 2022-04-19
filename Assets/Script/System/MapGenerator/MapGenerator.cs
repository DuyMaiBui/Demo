using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode { NoiseMap, ColourMap, Falloff };
    [SerializeField] private DrawMode drawMode;

    [Range(0, 10)]
    [SerializeField] private float falloffSmooth;
    [Range(0, 5)]
    [SerializeField] private float falloffDensity;

    [SerializeField] private int mapChunkSize;
    [Range(1, 10)]
    [SerializeField] private float noiseScale;

    [SerializeField] private int octaves;
    [Range(0, 1)]
    [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [Space(10)]
    public bool autoUpdate;
    [SerializeField] private bool randomSeed;

    [Space(10)]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject water;
    [SerializeField] private Transform parentTile;
    [SerializeField] private Transform parentEnvironment;

    [Space(10)]
    [SerializeField] private SaveDataParent[] environmentParents;
    [Space(10)]
    [SerializeField] private TerrainType[] regions;

    private float[,] falloffMap;
    private List<TileData> tileDatas = new List<TileData>();
    private string saveEnvironment;

    private void Awake()
    {
        // Assign Event
        EventDispatcher.LoadEvent += LoadMap;
        EventDispatcher.SaveEvent += SaveMap;
        EventDispatcher.DeleteSaveDataEvent += ClearSaveData;
        EventDispatcher.SetMapSizeEvent += SetMapSize;
        EventDispatcher.SetSeedEvent += SetSeed;
        EventDispatcher.SetRandomSeedEvent += SetRandomSeed;

        EventDispatcher.SetPlayerMiniMapEvent += SetPlayerMiniMap;
        EventDispatcher.ClearMapEvent += DeactiveMap;
        player.SetActive(false);
    }

    private void OnDisable()
    {
        //Remove Event
        EventDispatcher.LoadEvent -= LoadMap;
        EventDispatcher.SaveEvent -= SaveMap;
        EventDispatcher.DeleteSaveDataEvent -= ClearSaveData;
        EventDispatcher.SetMapSizeEvent -= SetMapSize;
        EventDispatcher.SetSeedEvent -= SetSeed;
        EventDispatcher.SetRandomSeedEvent -= SetRandomSeed;

        EventDispatcher.SetPlayerMiniMapEvent -= SetPlayerMiniMap;
        EventDispatcher.ClearMapEvent -= DeactiveMap;
    }

    private void SetPlayerMiniMap()
    {
        EventDispatcher.PlayerPosition(player.transform.position, mapChunkSize * 10);
    }

    private void SetMapSize(int size)
    {
        mapChunkSize = size;
    }

    private void SetRandomSeed(bool randomSeed)
    {
        this.randomSeed = randomSeed;
    }

    private void SetSeed(int value)
    {
        seed = value;
    }

    public void GenMap()
    {
        GenerateMap();
        GenerateTile();
        SetSpawnPosition();
    }

    public void DeactiveMap()
    {
        for (int i = 0; i < parentTile.childCount; i++)
        {
            for (int j = 0; j < parentTile.GetChild(i).childCount; j++)
            {
                ObjectPoolManager.instance.DeactiveGameObject(parentTile.GetChild(i).name, parentTile.GetChild(i).GetChild(j).gameObject);
            }
        }
        // Environment
        for (int i = 0; i < parentEnvironment.childCount; i++)
        {
            for (int j = 0; j < parentEnvironment.GetChild(i).childCount; j++)
            {
                ObjectPoolManager.instance.DeactiveGameObject(parentEnvironment.GetChild(i).name, parentEnvironment.GetChild(i).GetChild(j).gameObject);
            }
        }
    }

    public void RemoveMap()
    {
        DeactiveObject(parentTile);
        DeactiveObject(parentEnvironment);
    }

    #region Generate Map
    private void SetSpawnPosition()
    {
        int n = 0;
        while (tileDatas[n].indexTile != 2)
        {
            n++;
        }
        player.SetActive(true);
        player.transform.position = new Vector3(tileDatas[n].position.x, tileDatas[n].position.y + 1f, tileDatas[n].position.z);
    }

    private void GenerateMap()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize, falloffSmooth, falloffDensity);
        tileDatas.Clear();
        water.transform.position = new Vector3(-mapChunkSize * 5, -2f, -mapChunkSize * 5);
        water.transform.localScale = Vector3.one * mapChunkSize;
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (randomSeed)
            seed = (int)UnityEngine.Random.Range(0, 1000);
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, mapChunkSize / noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                    }
                    else
                        break;
                    if (i != 0)
                        if (i < regions.Length - 1)
                        {
                            if (noiseMap[x, y] < regions[i + 1].height && noiseMap[x, y] >= regions[i].height)
                            {
                                tileDatas.Add(new TileData(new Vector3(-x * 10, i, -y * 10), i));
                            }
                        }
                        else
                        {
                            if (noiseMap[x, y] <= 1 && noiseMap[x, y] >= regions[i].height)
                            {
                                tileDatas.Add(new TileData(new Vector3(-x * 10, i, -y * 10), i));
                            }
                        }
                }
            }
        }
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Falloff)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize, falloffSmooth, falloffDensity)));
        }
    }

    private void GenerateTile()
    {
        if (tileDatas.Count > 0)
            for (int i = 0; i < tileDatas.Count; i++)
            {
                var region = regions[tileDatas[i].indexTile];
                ObjectPoolManager.instance.InstanceGameObject(region.keyObjectPool, tileDatas[i].position, Quaternion.identity, region.parent);
                EventDispatcher.GenerateNatureItem(tileDatas[i]);
                for (int j = 1; j < regions.Length; j++)
                {
                    if (tileDatas[i].indexTile == j)
                    {
                        RandomEnvironment(regions[j], tileDatas[i].position);
                    }
                }
            }
    }

    private void RandomEnvironment(TerrainType region, Vector3 position)
    {
        int random = (int)UnityEngine.Random.Range(0, 99);
        if (random < 30)
            return;
        int amount = (int)UnityEngine.Random.Range(0, 10);

        for (int i = 0; i < amount; i++)
        {
            int roll = (int)UnityEngine.Random.Range(0, 99);
            for (int j = 0; j < region.environments.Length; j++)
            {
                if (j == 0)
                {
                    if (0 <= roll && roll < region.environments[j].ratio)
                    {
                        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(position.x - 4f, position.x + 4f), position.y, UnityEngine.Random.Range(position.z - 4f, position.z + 4f));
                        Vector3 randomRot = new Vector3(0f, UnityEngine.Random.Range(0f, 180f), 0f);
                        var go = ObjectPoolManager.instance.InstanceGameObject(region.environments[j].parent.name, region.environments[j].parent);
                        go.transform.position = randomPos;
                        go.transform.eulerAngles = randomRot;
                    }
                }
                else
                {
                    if (region.environments[j - 1].ratio <= roll && roll <= region.environments[j].ratio)
                    {
                        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(position.x - 4f, position.x + 4f), position.y, UnityEngine.Random.Range(position.z - 4f, position.z + 4f));
                        Vector3 randomRot = new Vector3(0f, UnityEngine.Random.Range(0f, 180f), 0f);
                        var go = ObjectPoolManager.instance.InstanceGameObject(region.environments[j].parent.name, region.environments[j].parent);
                        go.transform.position = randomPos;
                        go.transform.eulerAngles = randomRot;
                    }
                }
            }
        }
    }

    private void DeactiveObject(Transform parent)
    {
        tileDatas.Clear();
        for (int i = 0; i < parentTile.childCount; i++)
        {
            while (parent.GetChild(i).childCount != 0)
            {
                DestroyImmediate(parent.GetChild(i).GetChild(0).gameObject);
            }
        }
    }
    #endregion

    #region Save Load
    public void SaveMap()
    {
        SaveEnvironment();
        PlayerPrefs.SetFloat("Falloff_Smooth", falloffSmooth);
        PlayerPrefs.SetFloat("Falloff_Density", falloffDensity);
        PlayerPrefs.SetInt("Map_Chunk_Size", mapChunkSize);
        PlayerPrefs.SetFloat("Noise_Scale", noiseScale);
        PlayerPrefs.SetInt("Octaves", octaves);
        PlayerPrefs.SetFloat("Persistance", persistance);
        PlayerPrefs.SetFloat("Lacunarity", lacunarity);
        PlayerPrefs.SetInt("Seed", seed);
        PlayerPrefs.SetFloat("Offset_X", offset.x);
        PlayerPrefs.SetFloat("Offset_Y", offset.y);
        PlayerPrefs.SetString("Save_Environment", saveEnvironment);
        PlayerPrefs.SetFloat("Player_Position_X", player.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_Y", player.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_Z", player.transform.position.z);
        PlayerPrefs.Save();
    }

    public void LoadMap()
    {
        EventDispatcher.SetMiniMapCamera(new Vector3(mapChunkSize, 0, mapChunkSize));
        DeactiveMap();
        if (PlayerPrefs.HasKey("Save_Environment"))
        {
            falloffSmooth = PlayerPrefs.GetFloat("Falloff_Smooth");
            falloffDensity = PlayerPrefs.GetFloat("Falloff_Density");
            mapChunkSize = PlayerPrefs.GetInt("Map_Chunk_Size");
            noiseScale = PlayerPrefs.GetFloat("Noise_Scale");
            octaves = PlayerPrefs.GetInt("Octaves");
            persistance = PlayerPrefs.GetFloat("Persistance");
            lacunarity = PlayerPrefs.GetFloat("Lacunarity");
            seed = PlayerPrefs.GetInt("Seed");
            offset.x = PlayerPrefs.GetFloat("Offset_X");
            offset.y = PlayerPrefs.GetFloat("Offset_Y");
            saveEnvironment = PlayerPrefs.GetString("Save_Environment");

            GenerateMap();
            GenerateTile();
            LoadEnvironment();

            player.SetActive(true);
            player.transform.position = new Vector3(PlayerPrefs.GetFloat("Player_Position_X"), PlayerPrefs.GetFloat("Player_Position_Y"), PlayerPrefs.GetFloat("Player_Position_Z"));
        }
        else
        {
            GenMap();
        }
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

    private void SaveEnvironment()
    {
        saveEnvironment = string.Empty;
        for (int i = 0; i < environmentParents.Length; i++)
        {
            saveEnvironment += environmentParents[i].GetSaveData;
        }
        Debug.Log(saveEnvironment);
    }

    private void LoadEnvironment()
    {
        string[] saveDatas = saveEnvironment.Split("/");
        for (int i = 0; i < environmentParents.Length - 1; i++)
        {
            environmentParents[i].SetSaveData(saveDatas[i]);
        }
    }
    #endregion

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize, falloffSmooth, falloffDensity);
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    [Range(0, 1)]
    public float height;
    public Color colour;
    public string keyObjectPool;
    public Transform parent;
    public Environment[] environments;
}

[System.Serializable]
public struct Environment
{
    public Transform parent;
    public int ratio;
}
public struct TileData
{
    public Vector3 position;
    public int indexTile;

    public TileData(Vector3 position, int index)
    {
        this.position = position;
        this.indexTile = index;
    }
}