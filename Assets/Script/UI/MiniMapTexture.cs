using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapTexture : MonoBehaviour
{
    [SerializeField] RectTransform player;
    [SerializeField] Camera miniMapCamera;
    [SerializeField] RenderTexture miniMapTexture;

    private void Awake()
    {
        EventDispatcher.PlayerPositionEvent += SetPlayerPositionMiniMap;
        EventDispatcher.SetMiniMapEvent += SetCamera;
        if (miniMapCamera == null)
            miniMapCamera = GetComponent<Camera>();
    }

    private void OnDisable()
    {
        EventDispatcher.PlayerPositionEvent -= SetPlayerPositionMiniMap;
        EventDispatcher.SetMiniMapEvent -= SetCamera;
    }

    private void SetPlayerPositionMiniMap(Vector3 position, int mapSize)
    {
        player.localPosition = new Vector2(position.x * 800f / mapSize + 400f, position.z * 800f / mapSize + 400f);
    }

    private void SetCamera(Vector3 position)
    {
        transform.position = position;
        miniMapCamera.transform.position = new Vector3(position.x, 100, position.z);
        miniMapCamera.orthographicSize = position.x;
    }
}
