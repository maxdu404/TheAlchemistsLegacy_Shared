using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OutlineHover : MonoBehaviour
{
    // Start is called before the first frame update
    private Outline outline;
    public Color normalColor = Color.green;
    public Color hoverColor = Color.red;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.OutlineColor = normalColor;
    }

    void OnMouseEnter()
    {
        outline.OutlineColor = hoverColor;
    }

    void OnMouseExit()
    {
        outline.OutlineColor = normalColor;
    }
}
