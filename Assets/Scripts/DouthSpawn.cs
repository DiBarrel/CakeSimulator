using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DouthSpawn : MonoBehaviour
{
    public GameObject douth;
    public Transform parrentTransform; //родитель для объектов
    private int maxObjects = 12;
    private int middleObject = 6;
    public int countObjectIndex = 0;
    private float zOffset = 0.25f;
    private float xOffset = 0.2f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Vector3 spawnPosition;
    //Создание объекта в сцене
    public void SpawnDouth()
    {
        if (countObjectIndex >= maxObjects) return;
        spawnPosition = CalculatePosition(countObjectIndex);
        GameObject newObj = Instantiate(douth, spawnPosition, Quaternion.identity, parrentTransform);
        newObj.name = $"Douth_{countObjectIndex}";
        spawnedObjects.Add(newObj);
        countObjectIndex++;
    }
    //Высчитывается позиция появления теста
    private Vector3 CalculatePosition(int index)
    {
        float baseX = (index < middleObject) ? -0.23f : -0.23f + xOffset;
        float baseZ = 130.41f + (index % middleObject) * zOffset;
        return new Vector3(baseX, 0.12f, baseZ);
    }

    public void RemoveLastObject()
    {
        if (spawnedObjects.Count == 0) return;

        int lastIndex = spawnedObjects.Count - 1;
        Destroy(spawnedObjects[lastIndex]);
        spawnedObjects.RemoveAt(lastIndex);

        countObjectIndex--;
    }
}
