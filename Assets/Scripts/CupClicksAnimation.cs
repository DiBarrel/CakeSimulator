using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class CupClicksAnimation : MonoBehaviour
{
    private Animator animator;
    private float animationSpeed = 1f;
    private float clicksProgress = 0f;
    private int maxClicks = 10;
    private int currentClicks = 0;

    void Start()
    {
        animator = GetComponent<Animator>(); // получаем компоненты анимации для пыток над собой
        animator.speed = 0f;
    }
    void Update()
    {
        if (clicksProgress < (float)currentClicks / maxClicks)
        {
            clicksProgress = Mathf.MoveTowards(clicksProgress, (float)currentClicks / maxClicks, 
                animationSpeed * Time.deltaTime);
        }
        animator.Play(0, 1, clicksProgress);
    }

    private void OnObjectClicks()
    {
        currentClicks++;
        if (currentClicks >= maxClicks)
        {
            currentClicks = 0;
            clicksProgress = 0f;
        }
    }
}
