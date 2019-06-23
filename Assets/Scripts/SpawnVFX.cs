using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVFX : MonoBehaviour
{
    private float currentValue = 1;
    private float speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentValue > 0) {
            currentValue -= speed * Time.deltaTime;
            if (currentValue < 0)
            {
                currentValue = 0;
                GetComponent<Renderer>().sharedMaterial.SetFloat("_Dissolve", currentValue);
            }
            else {
                GetComponent<Renderer>().sharedMaterial.SetFloat("_Dissolve", currentValue);
            }

        }
    }
}
