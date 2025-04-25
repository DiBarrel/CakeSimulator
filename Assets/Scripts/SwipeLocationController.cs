using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeLocationController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public List<Camera> camerasList = new();
    public float swipeBeginThresholdPercent = 8f; // TODO
    public float swipeThresholdPercent = 25f;

    private List<RawImage> _rawImagesList = new();
    private List<Vector2> _rawImagesVelocityList = new();
    private int _curLocationIndex = 0;
    private bool _isSwiping = false;
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;

    private void Start()
    {
        SetupCanvas();
    }

    public void Update()
    {
        if (!_isSwiping)
        {
            MoveImagesToThierPositions();
        }
    }

    private void SetupCanvas()
    {
        RemoveCanvasItems();

        // Creating background image
        GameObject bgImageObj = new GameObject("bgImage");
        bgImageObj.transform.SetParent(transform, false);
        RawImage bgImage = bgImageObj.AddComponent<RawImage>();
        bgImage.color = new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1f);

        RectTransform rt = bgImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        CreateRenderTextures();
    }

    private void CreateRenderTextures()
    {
        for (int i = 0; i < camerasList.Count; i++)
        {
            // Creating render textures
            RenderTexture renderTexture = new RenderTexture(
                Screen.width,
                Screen.height,
                24,
                RenderTextureFormat.ARGB32);
            renderTexture.name = $"RenderTexture_{camerasList[i].name}";
            renderTexture.Create();

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
            rt.anchoredPosition = new Vector2(Screen.width * i, 0);

            // Creating velocity data
            _rawImagesVelocityList = new List<Vector2>(new Vector2[camerasList.Count]);
        }
    }

    private void RemoveCanvasItems()
    {
        foreach (var rawImage in _rawImagesList)
        {
            if (rawImage != null)
            {
                Destroy(rawImage);
            }
        }
        _rawImagesList.Clear();
        _rawImagesVelocityList.Clear();
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
                CalculateLocationImageXPos(i, _curLocationIndex) + Mathf.Clamp(newPos.x, -width, width), 0);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _endTouchPosition = eventData.position;
        float swipeDistance = _endTouchPosition.x - _startTouchPosition.x;
        float swipePercent = Mathf.Abs(swipeDistance) * 100 / Screen.width;

        if (swipePercent > swipeThresholdPercent)
        {
            if (swipeDistance > 0 && _curLocationIndex != 0)
            {
                _curLocationIndex -= 1;
            }
            else if (swipeDistance < 0 && _curLocationIndex != camerasList.Count-1)
            {
                _curLocationIndex += 1;
            }
        }
        _isSwiping = false;
    }

    private void MoveImagesToThierPositions()
    {
        //Debug.Log(_rawImagesList.Count);
        for (int i = 0; i < _rawImagesList.Count; i++)
        {
            RectTransform rt = _rawImagesList[i].GetComponent<RectTransform>();
            Vector2 targetPos = new Vector2(CalculateLocationImageXPos(i, _curLocationIndex), rt.anchoredPosition.y);
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

    private int CalculateLocationImageXPos(int locationIndex, int curLocation)
    {
        return (locationIndex - curLocation) * Screen.width;
    }

    private void OnEnable() => GameEventSystem.OnScreenResolutionChanged += SetupCanvas;
    private void OnDisable() => GameEventSystem.OnScreenResolutionChanged -= SetupCanvas;
}
