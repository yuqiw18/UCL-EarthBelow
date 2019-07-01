using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorAnimator2 : MonoBehaviour
{

    private float maxScale = 0.5f;
    private float scaleStep = 0.5f;

    private float currentScale;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.localScale = new Vector3(0, 0, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.localScale += new Vector3(Time.deltaTime * scaleStep, Time.deltaTime * scaleStep, 0);
        if (this.gameObject.transform.localScale.x >= maxScale) {

            this.gameObject.transform.localScale = new Vector3(0, 0, 0.1f);
        }
    }
}
