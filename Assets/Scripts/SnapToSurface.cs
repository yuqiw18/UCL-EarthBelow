using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToSurface : MonoBehaviour
{

    private int geoDistance = 6;
    private Vector3 snapDirection;
    private bool canSnap = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canSnap) {
            this.gameObject.transform.position += snapDirection * Time.deltaTime;
            Debug.Log("Moving!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        canSnap = false;
    }

    public void SetSnapDirection(Vector3 target) {
        snapDirection = target;
    }

    public void EnableSnap() {
        if (geoDistance <= 5) {
            canSnap = true;
        }
    }

    public void SetGeoDistance(int g) {
        geoDistance = g;
    }
}
