using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTag : MonoBehaviour
{
    public bool isEnabled = true;
    [SerializeField] private List<string> tags = new List<string>();

    public bool HasTag(string tagName)
    {
        return tags.Contains(tagName);
    }

    public void RenameTagByIndex(int index, string tagName)
    {
        tags[index] = tagName;
    }

    public string GetTagByIndex(int index)
    {
        return tags[index];
    }

    public int Count => tags.Count;
}