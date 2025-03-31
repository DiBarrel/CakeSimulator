using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SpawnCake : MonoBehaviour
{
    public GameObject cake;
    public Image imageFill;
   
    private float timeFill = 5f; // время заполнения 
    public Vector3 spawnPosition = new Vector3(-5.2f, 0.53f, 76.8f);
    public DouthSpawn douthDate;

    private float currentTime = 0f;
    private bool isFilling = false;

    
    void Update()
    {
        if (douthDate.countObjectIndex != 0)
        {
            StartFilling();
            if (isFilling)
            {
                currentTime += Time.deltaTime;
                imageFill.fillAmount = currentTime / timeFill;

                if (currentTime >= timeFill)
                {
                    CompliteProgress();
                }
            }
        }
    }

    public void StartFilling()
    {
        if (!isFilling)
        {
            currentTime = 0f;
            isFilling = true;
        }
    }

    private void CompliteProgress()
    {
        isFilling = false;
        imageFill.fillAmount = 0f;

        if(douthDate != null && douthDate.countObjectIndex != 0)
        {
            SpawnCakePosition();
            douthDate.RemoveLastObject();
        }
    }
    private void SpawnCakePosition()
    {
        Instantiate(cake, spawnPosition, Quaternion.identity);
    }

}
