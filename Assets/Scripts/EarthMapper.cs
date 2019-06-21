using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EarthMapper : MonoBehaviour
{

    public GameObject mapButton;

    public GameObject arSessionOrigin;

    public GameObject indicatorPrefab;
    public GameObject earthObjectToCopy;
    public GameObject earthPlanePrefab;

    public GameObject parentToAttach;
    public Material earthMaterial;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    //private ARPlaneManager arPlaneManager;
    //private ARPointCloudManager arPointCloudManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject highlightedIndicator;

    private GameObject mappedEarth;
    private GameObject mappedPlane;

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

        highlightedIndicator = Instantiate(indicatorPrefab);

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    private void OnEnable()
    {
        //arSessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
        //arSessionOrigin.GetComponent<ARPointCloudManager>().enabled = true;

        mapButton.SetActive(true);
        highlightedIndicator.SetActive(true);
    }

    private void OnDisable()
    {
        //arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        //arSessionOrigin.GetComponent<ARPointCloudManager>().enabled = false;

        mapButton.SetActive(false);
        highlightedIndicator.SetActive(false);
    }



    private void UpdatePlacementPose()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            Vector3 cameraForward = Camera.current.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            // Show the indicator if the pose is valid and rotate it to match the view
            highlightedIndicator.SetActive(true);
            highlightedIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            // Otherwise hide it
            highlightedIndicator.SetActive(false);
        }
    }

    public void MapEarth() {

        Destroy(mappedEarth);
        Destroy(mappedPlane);

        mappedEarth = Instantiate(earthObjectToCopy, placementPose.position, Quaternion.identity, parentToAttach.transform);

        Transform pinGroup = mappedEarth.transform.GetChild(0);

        mappedEarth.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().sharedMaterial = earthMaterial;

        Transform top = mappedEarth.transform.GetChild(2);
        Transform north = mappedEarth.transform.parent.GetChild(0);

        //mappedEarth.GetComponent<Renderer>().sharedMaterial = earthMaterial;

        float scale = GLOBAL.EARTH_PREFAB_SCALE_TO_REAL;

        mappedEarth.transform.localScale = new Vector3(scale, scale, scale);
        mappedEarth.transform.Translate(new Vector3(0, -scale * GLOBAL.EARTH_PREFAB_RADIUS, 0));
        mappedEarth.transform.rotation = GLOBAL.ROTATE_TO_TOP;

        // Scale and display each pin
        Vector3 referencePinPosition = pinGroup.GetChild(0).gameObject.transform.position;
        mappedEarth.SetActive(true);
        float pinScale = 60;
        foreach (Transform pin in pinGroup)
        {
            if (pin.position != referencePinPosition)
            {
                pin.position = pinScale * (pin.position - new Vector3(referencePinPosition.x, 0, referencePinPosition.z)).normalized;
                pin.localScale = new Vector3(1 / scale, 1 / scale, 1 / scale);
                Debug.Log("CalculatedPosition:" + pin.localPosition);
            }
        }

        //int i = 0;

        //List<Transform> pins = new List<Transform>();

        //foreach (Transform pin in mappedEarth.transform)
        //{
        //    pins.Add(pin);
        //}

        //foreach (Transform pin in pins) { 
        //    pin.SetParent(mappedPlane.transform, true);
        //    pin.transform.localScale = new Vector3(20, 20, 20);
        //    i++;
        //    Debug.Log("SetParent:" + i);

        //}

        //Destroy(mappedEarth);

    }




}
