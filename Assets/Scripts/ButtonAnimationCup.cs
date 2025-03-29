using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimationCup : MonoBehaviour
{
    public Animator animator;
    public int clicksToComplete = 10;
    public float smoothTime = 0.2f; // Время плавного перехода

    [Header("Debug")]
    [SerializeField] private int _currentClicks;
    [SerializeField] private float _currentNormalizedTime;
    private float _velocity;

    void Start()
    {
        animator.speed = 0; // Полный ручной контроль
        _currentNormalizedTime = 0f;
        UpdateAnimation();
    }

    void Update()
    {
        // Плавное обновление позиции
        if (!Mathf.Approximately(_currentNormalizedTime, GetTargetTime()))
        {
            _currentNormalizedTime = Mathf.SmoothDamp(_currentNormalizedTime,GetTargetTime(),
                ref _velocity,smoothTime
            );

            UpdateAnimation();
        }
    }

    public void AddProgress()
    {
        _currentClicks++;
        if (_currentClicks > clicksToComplete)
        {
            // Мгновенный сброс без плавного перехода
            _currentClicks = 0;
            _currentNormalizedTime = 0f;
            _velocity = 0f;
            UpdateAnimation();
        }
    }

    private float GetTargetTime()
    {
        return (float)_currentClicks / clicksToComplete;
    }

    private void UpdateAnimation()
    {
        animator.Play("Cup of dough", 0, _currentNormalizedTime);
        animator.Update(0f); // Мгновенное обновление
    }
}
