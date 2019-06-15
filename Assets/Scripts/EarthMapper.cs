using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EarthMapper : MonoBehaviour
{

    public GameObject arSessionOrigin;

    public GameObject earthObjectToCopy;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    //private ARPlaneManager arPlaneManager;
    //private ARPointCloudManager arPointCloudManager;

    // Start is called before the first frame update
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        //arPlaneManager = FindObjectOfType<ARPlaneManager>();
        //arPointCloudManager = FindObjectOfType<ARPointCloudManager>();

        //Vector3 currentPinPosition = (GetLocation.pinList[0].transform.position.normalized - earthObject.transform.position.normalized).normalized;
        //Vector3 targetPinPosition = (earthObject.transform.parent.transform.up - earthObject.transform.position.normalized).normalized;

        //Quaternion rotateToTop = Quaternion.FromToRotation(currentPinPosition, targetPinPosition);
        //earthObject.transform.rotation = rotateToTop;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        arSessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
        arSessionOrigin.GetComponent<ARPointCloudManager>().enabled = true;

    }

    private void OnDisable()
    {
        arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        arSessionOrigin.GetComponent<ARPointCloudManager>().enabled = false;
    }
}
