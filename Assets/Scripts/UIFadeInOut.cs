using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeInOut : MonoBehaviour
{
    private Text text;
    // Start is called before the first frame update

    void Awake(){
        text = this.GetComponent<Text>();
    }

    private void OnEnable(){
        FadeIn();
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
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FadeToZeroAlpha()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime);
            yield return null;
        }
    }

}
