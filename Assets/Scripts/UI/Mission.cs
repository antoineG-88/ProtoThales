using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour
{
    public string title;
    [TextArea]
    public string description;
    public RectTransform iconRectTransform;
    [HideInInspector] public Vector2 iconPos;
    public Image iconImage;
    public bool isSelectable;
}
