using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SwipeLocationController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public List<Camera> camerasList = new List<Camera>();
    public float swipeBeginThresholdPercent = 8f; // TODO
    public float swipeThresholdPercent = 25f;

    private List<RawImage> _rawImagesList = new List<RawImage>();
    private List<Vector2> _rawImagesVelocityList;
    private List<RenderTexture> _renderTexturesList = new List<RenderTexture>();
    private int _curlocationIndex;
    private bool _isSwiping = false;
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;

    private void Start()
    {
        SetupCanvas();
    }

    private void SetupCanvas()
    {
        // Creating background image
        GameObject bgImageObj = new GameObject("bgImage");
        bgImageObj.transform.SetParent(transform, false);
        RawImage bgImage = bgImageObj.AddComponent<RawImage>();
        bgImage.color = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 1f);

        RectTransform rt = bgImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

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

            rt = rawImage.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = new Vector2(Screen.width*i, 0);

            // Creating velocity data
            _rawImagesVelocityList = new List<Vector2>(new Vector2[camerasList.Count]);
        }
    }

    public void Update()
    {
        if (!_isSwiping)
        {
            MoveImagesToThierPositions();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //this.enabled = true;
        _isSwiping = true;
        _startTouchPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isSwiping) return;

        RectTransform canvasRect = GetComponent<RectTransform>();
        float width = canvasRect.rect.width;

        float offsetX = eventData.position.x - _startTouchPosition.x;
        Vector2 newPos = new Vector2(offsetX, 0);

        for (int i = 0; i < _rawImagesList.Count; i++)
        {
            RectTransform rt = _rawImagesList[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(
                CalculateXPos(i, _curlocationIndex) + Mathf.Clamp(newPos.x, -width, width), 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _endTouchPosition = eventData.position;
        float swipeDistance = _endTouchPosition.x - _startTouchPosition.x;
        float swipePercent = Mathf.Abs(swipeDistance) * 100 / Screen.width;

        if (swipePercent > swipeThresholdPercent)
        {
            //camerasList[_curlocationIndex].tag = "Untagged";
            if (swipeDistance > 0 && _curlocationIndex != 0)
            {
                _curlocationIndex -= 1;
            }
            else if (swipeDistance < 0 && _curlocationIndex != camerasList.Count-1)
            {
                _curlocationIndex += 1;
            }
            //camerasList[_curlocationIndex].tag = "MainCamera";
            //this.enabled = false;
        }
        _isSwiping = false;
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

    private void MoveImagesToThierPositions()
    {
        Debug.Log($"NOW on loc #{_curlocationIndex}");
        for (int i = 0; i < _rawImagesList.Count; i++)
        {
            RectTransform rt = _rawImagesList[i].GetComponent<RectTransform>();
            Vector2 targetPos = new Vector2(CalculateXPos(i, _curlocationIndex), rt.anchoredPosition.y);
            Vector2 currentVelocity = _rawImagesVelocityList[i];
            rt.anchoredPosition = Vector2.SmoothDamp(
                rt.anchoredPosition,
                targetPos,
                ref currentVelocity,
                0.1f
            );
            _rawImagesVelocityList[i] = currentVelocity;
        }
    }

    private int CalculateXPos(int locationIndex, int curLocation)
    {
        return (locationIndex - curLocation) * Screen.width;
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
