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

        mappedEarth = Instantiate(earthObjectToCopy, placementPose.position, Quaternion.identity);

        Transform pinGroup = mappedEarth.transform.GetChild(0);
        Transform refTop = mappedEarth.transform.GetChild(2);

        mappedEarth.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().sharedMaterial = earthMaterial;

        //mappedEarth.GetComponent<Renderer>().sharedMaterial = earthMaterial;

        // Scale and reposition the Earth
        float scale = GLOBAL.EARTH_PREFAB_SCALE_TO_REAL;
        mappedEarth.transform.localScale = new Vector3(scale, scale, scale);
        mappedEarth.transform.Translate(new Vector3(0, -scale * GLOBAL.EARTH_PREFAB_RADIUS, 0));

        // Rotate the Earth so that the current position is facing up
        // Use the pre-calculated value
        mappedEarth.transform.rotation = GLOBAL.ROTATE_TO_TOP;

        // Rotate the Earth sp that the current north direction is aligned geographically
        // First, find the current device facing direction (projection on z axis)
        Vector3 facingDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 prefabNorthDirection = Vector3.ProjectOnPlane(refTop.position - mappedEarth.transform.position, Vector3.up).normalized;
        Quaternion rotateToFacingDirection = Quaternion.FromToRotation(prefabNorthDirection, facingDirection);

        // Compute the degree required to rotate the north direction to match the facing direction
        float degree;
        Vector3 axis;
        rotateToFacingDirection.ToAngleAxis(out degree, out axis);
        mappedEarth.transform.RotateAround(transform.position, Vector3.up, degree);

        // Then rotate the Earth again according to the heading direction from the GPS so that the north direction is matched both virtually and physically
        mappedEarth.transform.Rotate(Vector3.up, -Input.compass.trueHeading, Space.World);

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
