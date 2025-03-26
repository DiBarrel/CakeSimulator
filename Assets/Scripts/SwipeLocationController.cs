using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeLocationController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public List<Camera> camerasList = new List<Camera>();
    public float swipeThreshold = 50f; // Порог свайпа

    private List<RawImage> _rawImagesList = new List<RawImage>();
    private List<RenderTexture> _renderTexturesList = new List<RenderTexture>();
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private int _curlocationIndex;

    private void Start()
    {
        SetupCanvas();
    }

    private void SetupCanvas()
    {
        for (int i = 0; i < camerasList.Count; i++)
        {
            // Creating target texture for cameras
            RenderTexture renderTexture = new RenderTexture(
                Screen.width,
                Screen.height,
                24,
                RenderTextureFormat.ARGB32);
            renderTexture.name = $"RenderTexture_{camerasList[i].name}";
            renderTexture.Create();
            _renderTexturesList.Add(renderTexture);

            camerasList[i].targetTexture = renderTexture;
            camerasList[i].forceIntoRenderTexture = true;

            // Creating empty object
            GameObject rawImageObj = new GameObject($"RawImage_{camerasList[i].name}");
            rawImageObj.transform.SetParent(transform, false);

            // Creating raw image
            RawImage rawImage = rawImageObj.AddComponent<RawImage>();
            rawImage.texture = renderTexture;
            _rawImagesList.Add(rawImage);

            RectTransform rt = rawImage.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = new Vector2(Screen.width*i, 0);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startTouchPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Можно добавить плавное перемещение текстур во время свайпа
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _endTouchPosition = eventData.position;
        float swipeDistance = _endTouchPosition.x - _startTouchPosition.x;

        if (Mathf.Abs(swipeDistance) > swipeThreshold)
        {
            if (swipeDistance > 0 && _curlocationIndex != 0)
            {
                MoveImages(Screen.width);
                _curlocationIndex -= 1;
            }
            else if (swipeDistance < 0 && _curlocationIndex != camerasList.Count)
            {
                MoveImages(-Screen.width);
                _curlocationIndex += 1;
            }
        }
    }

    //private void UpdateRenderTextures()
    //{
    //    // Уничтожаем старые Render Texture (если есть)
    //    if (cam1RT != null) cam1RT.Release();
    //    if (cam2RT != null) cam2RT.Release();

    //    // Создаём новые Render Texture с текущим размером экрана
    //    cam1RT = new RenderTexture(Screen.width, Screen.height, 24);
    //    cam2RT = new RenderTexture(Screen.width, Screen.height, 24);

    //    // Назначаем камерам новые Render Texture
    //    cam1.targetTexture = cam1RT;
    //    cam2.targetTexture = cam2RT;

    //    // Привязываем RawImage к новым Render Texture
    //    cam1Display.texture = cam1RT;
    //    cam2Display.texture = cam2RT;
    //}

    private void MoveImages(float offsetX)
    {
        for (int i = 0; i < _rawImagesList.Count; i++)
        {
            RectTransform rt = _rawImagesList[i].GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(offsetX, 0);
        }
    }

    private void ClearOldTextures()
    {
        foreach (var rt in _renderTexturesList)
        {
            if (rt != null)
            {
                rt.Release();
                Destroy(rt);
            }
        }
        _renderTexturesList.Clear();
        _rawImagesList.Clear();
    }

    private void OnDestroy()
    {
        ClearOldTextures();
    }
}
