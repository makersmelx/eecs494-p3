using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManagerUI : MonoBehaviour
{
    [Tooltip("Main Slider displays the current time left")]
    public Slider mainSlider;

    [Header("Arbitrary Change")] [Tooltip("Decrease Slider highlights the arbitrary time decrease")]
    public Slider decreaseSlider;

    [Tooltip("Increase Slider highlights the arbitrary time decrease")]
    public Slider increaseSlider;

    public Slider increaseHelperSlider;

    public float changeDuration = 0.5f;

    private Coroutine currentUIAnimation;

    private void Start()
    {
        TimeManager.Instance.AddTimeChangeCallback(UpdateTimeChange);
        ResetAllSubSlider();
    }

    private void Update()
    {
        mainSlider.value = TimeManager.Instance.GetCurrentTime() / TimeManager.Instance.timeLimit;
    }

    private void UpdateTimeChange(float previous, float current, float change)
    {
        if (currentUIAnimation != null)
        {
            StopCoroutine(currentUIAnimation);
        }

        ResetAllSubSlider();
        currentUIAnimation = StartCoroutine(DisplayThenResetSlider(previous, current, change));
    }

    private void ResetAllSubSlider()
    {
        decreaseSlider.value = 0;
        increaseSlider.value = 0;
        increaseHelperSlider.value = 0;
    }

    private IEnumerator DisplayThenResetSlider(float previous, float current, float change)
    {
        if (change < 0f)
        {
            float count = 0;
            decreaseSlider.value = previous / TimeManager.Instance.timeLimit;
            while (count <= changeDuration)
            {
                decreaseSlider.value = mainSlider.value - change / TimeManager.Instance.timeLimit;
                count += Time.deltaTime;
                yield return null;
            }

            decreaseSlider.value = 0f;
        }
        else if (change > 0f)
        {
            float count = 0;
            increaseHelperSlider.value = previous / TimeManager.Instance.timeLimit;
            while (count <= changeDuration)
            {
                increaseSlider.value = mainSlider.value;
                increaseHelperSlider.value = increaseSlider.value - change / TimeManager.Instance.timeLimit;
                count += Time.deltaTime;
                yield return null;
            }

            increaseSlider.value = 0f;
            increaseHelperSlider.value = 0f;
        }

        ResetAllSubSlider();
    }
}