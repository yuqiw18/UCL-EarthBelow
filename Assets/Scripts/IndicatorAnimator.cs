using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorAnimator : MonoBehaviour
{

    private const float minHeight = 0.25f;
    private const float maxHeight = 0.4f;
    private Vector3 displacement = new Vector3(0, 0.15f, 0);

    private float currentHeight;
    private float targetHeight;

    private bool rise;

    // Start is called before the first frame update
    void Start()
    {
        currentHeight = this.gameObject.transform.localPosition.y;
        rise = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (rise) {

            this.gameObject.transform.localPosition += displacement * Time.deltaTime;
            currentHeight = this.gameObject.transform.localPosition.y;

            if (currentHeight > maxHeight) {
                rise = false;
            }

        }
        else { 
            this.gameObject.transform.localPosition -= displacement * Time.deltaTime;
            currentHeight = this.gameObject.transform.localPosition.y;

            if (currentHeight < minHeight) {
                rise = true;
            }
        }
    }
}
