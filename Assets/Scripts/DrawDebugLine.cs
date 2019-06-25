using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDebugLine : MonoBehaviour
{

    public Transform targetTransform;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform != null) {
            lineRenderer.SetPosition(0, this.transform.position);
            lineRenderer.SetPosition(1, targetTransform.position);
        }
    }
}
