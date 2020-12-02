using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material LineMaterial;
    public Color defaultColor;
    float h, s, v;

    // Update is called once per frame
    void Update()
    {
        Color.RGBToHSV(LineMaterial.color, out h, out s, out v);
        //Debug.Log("h: " + h + " s: " + s + " v: " + v);
        h += .003f;
        //Debug.Log("aa: " + h);
        LineMaterial.color = Color.HSVToRGB(h, s, v);
    }
}
