using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    #region DYNAMIC_FUNCTIONS
    // For accessing through event system
    public void FadeIn(Text text)
    {
        instance.StartCoroutine(FadeToAlpha(text, 1.0f));
    }

    public void FadeIn(Image image)
    {
        instance.StartCoroutine(FadeToAlpha(image, 1.0f));
    }

    public void FadeIn(SVGImage svg)
    {
        instance.StartCoroutine(FadeToAlpha(svg, 1.0f));
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

    #region STATIC_FUNCTIONS
    // For accessing through script
    public static void StaticFadeIn(Text text, float alpha)
    {
        instance.StartCoroutine(FadeToAlpha(text, alpha));
    }

    public static void StaticFadeIn(Image image, float alpha)
    {
        instance.StartCoroutine(FadeToAlpha(image, alpha));
    }

    public static void StaticFadeIn(SVGImage svg, float alpha)
    {
        instance.StartCoroutine(FadeToAlpha(svg, alpha));
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
    private static IEnumerator FadeToAlpha(Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < alpha)
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
    private static IEnumerator FadeToAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        while (image.color.a < alpha)
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
    private static IEnumerator FadeToAlpha(SVGImage image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        while (image.color.a < alpha)
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
