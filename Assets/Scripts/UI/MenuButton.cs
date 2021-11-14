using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI text;

    public Color onBlur = Color.white;

    private void Start()
    {
        text.color = onBlur;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = new Color(255 / 255f, 224 / 255f, 66 / 255f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = onBlur;
    }
}