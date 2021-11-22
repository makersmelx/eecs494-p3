using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFadeIn : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeInDuration = 4f;

    void Start()
    {
        if (canvasGroup == null) return;
        StartCoroutine(FadeIn());
    }

    public void SkipScene()
    {

    }

    IEnumerator FadeIn()
    {
        // Begin fade in
        float timeElapsed = 0f;
        canvasGroup.alpha = 0;

        // Fade for fadeInDuration seconds
        while (timeElapsed < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timeElapsed / fadeInDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Fade in is complete
        canvasGroup.alpha = 1;
    }
}
