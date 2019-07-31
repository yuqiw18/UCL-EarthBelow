using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FadeIn(Text text)
    {
        StartCoroutine(FadeToAlpha(text));
    }

    public void FadeIn(Image image)
    {
        StartCoroutine(FadeToAlpha(image));
    }

    public void FadeIn(SVGImage svg)
    {
        StartCoroutine(FadeToAlpha(svg));
    }

    public void FadeOut(Text text)
    {
        StartCoroutine(FadeToZeroAlpha(text));
    }

    public void FadeOut(Image image)
    {
        StartCoroutine(FadeToZeroAlpha(image));
    }

    public void FadeOut(SVGImage svg)
    {
        StartCoroutine(FadeToZeroAlpha(svg));
    }

    #region OVERLOAD_TEXT
    private static IEnumerator FadeToAlpha(Text text)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime * 2.0f);
            yield return null;
        }
    }

    private static IEnumerator FadeToZeroAlpha(Text text) {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime * 2.0f);
            yield return null;
        }
    }
    #endregion

    #region OVERLAOD_IMAGE
    private static IEnumerator FadeToAlpha(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        while (image.color.a < 1.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime * 2.0f);
            yield return null;
        }
    }

    private static IEnumerator FadeToZeroAlpha(Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        while (image.color.a > 0.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime * 2.0f);
            yield return null;
        }
    }
    #endregion

    #region OVERLOAD_SVGIMAGE
    private static IEnumerator FadeToAlpha(SVGImage image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        while (image.color.a < 1.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime * 2.0f);
            yield return null;
        }
    }

    private static IEnumerator FadeToZeroAlpha(SVGImage image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        while (image.color.a > 0.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime * 2.0f);
            yield return null;
        }
    }
    #endregion
}
