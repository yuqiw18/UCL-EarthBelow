using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{

    private bool canUpdate = false;

    private void OnEnable()
    {
        canUpdate = true;
    }

    private void OnDisable()
    {
        canUpdate = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (canUpdate) {
            this.gameObject.transform.LookAt(Camera.main.transform);
        }
    }
}
