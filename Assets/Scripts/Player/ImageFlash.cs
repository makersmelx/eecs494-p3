using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class ImageFlash : MonoBehaviour
{

    Image Img = null;
    Coroutine CurrentFlashRoutine = null;
    // Start is called before the first frame update
    void Awake()
    {
        Img = GetComponent<Image>();
    }

    public void StartFlash(float seconds, float maxAlpha, Color color)
    {
        Img.color = color;
        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);

        if (CurrentFlashRoutine != null)
            StopCoroutine(CurrentFlashRoutine);
        CurrentFlashRoutine = StartCoroutine(Flash(seconds, maxAlpha));
    }

    IEnumerator Flash(float seconds, float maxAlpha)
    {
        float duration = seconds / 2;

        // animate flash in
        for (float t = 0; t <= duration; t += Time.deltaTime)
        {
            Color ThisFrameColor = Img.color;
            ThisFrameColor.a = Mathf.Lerp(0, maxAlpha, t / duration);
            Img.color = ThisFrameColor;
            yield return null;
        }

        // animate flash out
        for (float t = 0; t <= duration; t += Time.deltaTime)
        {
            Color ThisFrameColor = Img.color;
            ThisFrameColor.a = Mathf.Lerp(maxAlpha, 0, t / duration);
            Img.color = ThisFrameColor;
            yield return null;
        }

        // make sure final color is zero
        Img.color = new Color(0, 0, 0, 0);
    }
}
