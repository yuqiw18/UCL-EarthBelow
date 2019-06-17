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
    public Material earthMaterial;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    //private ARPlaneManager arPlaneManager;
    //private ARPointCloudManager arPointCloudManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject highlightedIndicator;

    private GameObject mappedEarth;

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
        arSessionOrigin.GetComponent<ARPlaneManager>().enabled = true;
        arSessionOrigin.GetComponent<ARPointCloudManager>().enabled = true;

        mapButton.SetActive(true);
        highlightedIndicator.SetActive(true);
    }

    private void OnDisable()
    {
        arSessionOrigin.GetComponent<ARPlaneManager>().enabled = false;
        arSessionOrigin.GetComponent<ARPointCloudManager>().enabled = false;

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
        Debug.Log("Mapping Triggered");
        Destroy(mappedEarth);
        Debug.Log("PLACEMENT-Y" + placementPose.position.y);
        mappedEarth = Instantiate(earthObjectToCopy, placementPose.position, placementPose.rotation);
        mappedEarth.transform.localScale = new Vector3(200, 200, 200);
        mappedEarth.transform.Translate(new Vector3(0, -200*GLOBAL.EARTH_PREFAB_RADIUS, 0));

        mappedEarth.transform.rotation = GLOBAL.ROTATE_TO_TOP;

        //mappedEarth.gameObject.GetComponent<Renderer>().sharedMaterial = earthMaterial;

        mappedEarth.SetActive(true);
        //mappedEarth = Instantiate(largeEarth, new Vector3(placementPose.position.x, placementPose.position.y - GLOBAL.EARTH_PREFAB_RADIUS, placementPose.position.z), placementPose.rotation);

        //mappedEarth.transform.Translate(new Vector3(0, (placementPose.position.y - GLOBAL.EARTH_PREFAB_RADIUS)/2.0f, 0));


        //mappedEarth.transform.localScale = new Vector3(2000, 2000, 2000);
        //mappedEarth.transform.Translate(new Vector3(0, -2000* GLOBAL.EARTH_PREFAB_RADIUS, 0));
    }




}
