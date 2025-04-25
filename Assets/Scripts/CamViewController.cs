using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamViewController : MonoBehaviour
{
    public GameObject target;
    [Range(10, 90)]
    public float angle = 27f; // TODO get current
    public float camHeight = 4f; // TODO auto 'far' and height
    [Range(0, 100)]
    public float camBoundsPercent = 20f;

    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = new Bounds(renderers[0].bounds.center, Vector3.zero);
        foreach (Renderer renderer in renderers) bounds.Encapsulate(renderer.bounds);

        transform.position = new Vector3(
            bounds.max.x + bounds.size.x * 0.1f,
            bounds.center.y + camHeight,
            bounds.center.z
        );
        transform.LookAt(bounds.center);

        Camera cam = GetComponent<Camera>();
        cam.orthographicSize = GetRequiredOrthoSize(bounds);
    }

    private float GetRequiredOrthoSize(Bounds bounds)
    {
        float objectWidthX = bounds.size.x * angle / 100;
        float objectWidthZ = bounds.size.z;

        float screenAspect = (float)Screen.width / Screen.height;

        if (objectWidthZ > objectWidthX * screenAspect)
        {
            return objectWidthZ / (2f * screenAspect) * (1 + camBoundsPercent / 100);
        }
        else
        {
            return objectWidthX / 2f * (1 + camBoundsPercent / 100);
        }
    }

    private void OnEnable() => GameEventSystem.OnScreenResolutionChanged += Setup;
    private void OnDisable() => GameEventSystem.OnScreenResolutionChanged -= Setup;
}
