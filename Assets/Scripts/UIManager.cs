using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    #region DYNAMIC
    // For accessing through event system
    public void FadeIn(Text text)
    {
        instance.StartCoroutine(FadeToAlpha(text));
    }

    public void FadeIn(Image image)
    {
        instance.StartCoroutine(FadeToAlpha(image));
    }

    public void FadeIn(SVGImage svg)
    {
        instance.StartCoroutine(FadeToAlpha(svg));
    }

    public void FadeOut(Text text)
    {
        instance.StartCoroutine(FadeToZeroAlpha(text));
    }

    public void FadeOut(Image image)
    {
        instance.StartCoroutine(FadeToZeroAlpha(image));
    }

    public void FadeOut(SVGImage svg)
    {
        instance.StartCoroutine(FadeToZeroAlpha(svg));
    }
    #endregion

    #region STATIC
    // For accessing through script
    public static void StaticFadeIn(Text text)
    {
        instance.StartCoroutine(FadeToAlpha(text));
    }

    public static void StaticFadeIn(Image image)
    {
        instance.StartCoroutine(FadeToAlpha(image));
    }

    public static void StaticFadeIn(SVGImage svg)
    {
        instance.StartCoroutine(FadeToAlpha(svg));
    }

    public static void StaticFadeOut(Text text)
    {
        instance.StartCoroutine(FadeToZeroAlpha(text));
    }

    public static void StaticFadeOut(Image image)
    {
        instance.StartCoroutine(FadeToZeroAlpha(image));
    }

    public static void StaticFadeOut(SVGImage svg)
    {
        instance.StartCoroutine(FadeToZeroAlpha(svg));
    }
    #endregion

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
