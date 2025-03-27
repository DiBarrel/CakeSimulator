using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarFilling : MonoBehaviour
{
    public Image progressImage;
    private float fillSpeed = 8f;
    private float targetFillAmount = 0f;
    private bool isFilling = false;
    private int maxClicks = 10;
    private int currentClicks = 0;
    private void Update()
    {
        if (isFilling)
        {
            // Плавное заполнение индикатора
            progressImage.fillAmount = Mathf.Lerp(progressImage.fillAmount,targetFillAmount,
                fillSpeed * Time.deltaTime);

            // Стоп анимации при конце индикатора
            if (Mathf.Abs(progressImage.fillAmount - targetFillAmount) < 0.01f)
            {
                progressImage.fillAmount = targetFillAmount;
                isFilling = false;
            }
        }
    }
    public void AddProgress()
    {
        currentClicks++;

        // сброс нажатий
        if (currentClicks >= maxClicks)
        {
            currentClicks = 0;
            targetFillAmount = 0f;
        }
        else
        {
            // заполнение нажатий
            targetFillAmount = (float)currentClicks / maxClicks;
        }

        isFilling = true;
    }
}
