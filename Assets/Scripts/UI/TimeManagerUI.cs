using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManagerUI : MonoBehaviour
{
    [Header("Time Display")] [Tooltip("Main Slider displays the current time left")]
    public Slider mainSlider;

    public TextMeshProUGUI timer;

    [Header("Arbitrary Change")] [Tooltip("Decrease Slider highlights the arbitrary time decrease")]
    public TextMeshProUGUI changeText;

    public Slider decreaseSlider;


    [Tooltip("Increase Slider highlights the arbitrary time decrease")]
    public Slider increaseSlider;

    public Slider increaseHelperSlider;

    public float changeDuration = 0.5f;

    private Coroutine currentUIAnimation;

    private void Start()
    {
        TimeManager.Instance.AddTimeChangeCallback(UpdateTimeChange);
        ResetAllChangeUI();
    }

    private void Update()
    {
        mainSlider.value = TimeManager.Instance.GetCurrentTime() / TimeManager.Instance.maxTime;
        timer.text = Mathf.Floor(TimeManager.Instance.GetCurrentTime()).ToString(CultureInfo.InvariantCulture);
    }

    private void UpdateTimeChange(float previous, float current, float change)
    {
        if (currentUIAnimation != null)
        {
            StopCoroutine(currentUIAnimation);
        }

        ResetAllChangeUI();
        currentUIAnimation = StartCoroutine(DisplayChangeThenReset(previous, current, change));
    }

    private void ResetAllChangeUI()
    {
        decreaseSlider.value = 0;
        increaseSlider.value = 0;
        increaseHelperSlider.value = 0;
        changeText.gameObject.SetActive(false);
    }

    private IEnumerator DisplayChangeThenReset(float previous, float current, float change)
    {
        if (change < 0f)
        {
            changeText.gameObject.SetActive(true);
            changeText.color = Color.red;
            changeText.text = "- " + Mathf.Abs(change).ToString(CultureInfo.InvariantCulture);
            float count = 0;
            decreaseSlider.value = previous / TimeManager.Instance.maxTime;
            while (count <= changeDuration)
            {
                decreaseSlider.value = mainSlider.value - change / TimeManager.Instance.maxTime;
                count += Time.deltaTime;
                yield return null;
            }

            decreaseSlider.value = 0f;
        }
        else if (change > 0f)
        {
            changeText.gameObject.SetActive(true);
            changeText.color = Color.yellow;
            changeText.text = "+ " + Mathf.Abs(change).ToString(CultureInfo.InvariantCulture);
            float count = 0;
            increaseHelperSlider.value = previous / TimeManager.Instance.maxTime;
            while (count <= changeDuration)
            {
                increaseSlider.value = mainSlider.value;
                increaseHelperSlider.value = increaseSlider.value - change / TimeManager.Instance.maxTime;
                count += Time.deltaTime;
                yield return null;
            }

            increaseSlider.value = 0f;
            increaseHelperSlider.value = 0f;
        }

        ResetAllChangeUI();
    }
}