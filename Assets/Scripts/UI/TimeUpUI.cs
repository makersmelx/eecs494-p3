using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUpUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject panel;
    public GameObject text;
    public GameObject button;
    public GameObject buttonText;

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioManager.Instance.PlayFailSound();
    }

    // Update is called once per frame
    public void OnClick()
    {
        LevelManager.Instance.isDead = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        TimeManager.Instance.ResetTimer();
        gameObject.SetActive(false);
    }

    public IEnumerator DeathSceneTransaction()
    {
        LevelManager.Instance.isDead = true;
        button.SetActive(false);
        yield return CameraFade(true, 0.3f);
        yield return new WaitForSeconds(0.2f);
        yield return TextFadeIn(text, 3.2f, 4f);
        button.SetActive(true);
    }

    IEnumerator CameraFade(bool fadeIn, float duration)
    {
        float start = Time.time;
        float progress = (Time.time - start) / duration;
        while (progress < 1f)
        {
            progress = (Time.time - start) / duration;

            float alpha = fadeIn ? Mathf.Sqrt(progress) : Mathf.Sqrt(1 - progress);

            panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }

    IEnumerator TextFadeIn(GameObject target, float duration, float powBase)
    {
        float start = Time.time;
        float progress = (Time.time - start) / duration;
        while (progress < 1f)
        {
            progress = (Time.time - start) / duration;

            float alpha = Mathf.Pow(progress, powBase);

            target.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }

    IEnumerator ButtonFadeIn(float duration)
    {
        float start = Time.time;
        float progress = (Time.time - start) / duration;
        while (progress < 1f)
        {
            progress = (Time.time - start) / duration;

            float alpha = Mathf.Sqrt(progress);

            button.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }
}