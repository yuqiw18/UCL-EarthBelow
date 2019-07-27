using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFadeInOut : MonoBehaviour
{

    public bool fadeInOnLoad = true;
    private Text text;

    void Awake(){
        text = this.GetComponent<Text>();
        if (text == null) {
            text = this.transform.Find("Text").GetComponent<Text>();
        }
    }

    private void OnEnable(){
        if (fadeInOnLoad) {
            FadeIn();
        }
    }

    public void FadeIn(){
        StartCoroutine(FadeToFullAlpha());
    }

    public void FadeOut(){
        StartCoroutine(FadeToZeroAlpha());
    }

    private IEnumerator FadeToFullAlpha()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime * 2.0f);
            yield return null;
        }
    }

    private IEnumerator FadeToZeroAlpha()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime * 2.0f);
            yield return null;
        }
    }

}
