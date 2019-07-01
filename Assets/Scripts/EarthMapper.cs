using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class EarthMapper : MonoBehaviour
{

    public GameObject mapperOptions;

    public GameObject indicatorPrefab;
    public GameObject earthObjectToCopy;
    public GameObject earthPlanePrefab;

    public GameObject canvasWorld;
    public Text labelPrefab;
    public GameObject panelPrefab;

    public Texture2D[] thumbnail;

    private ARRaycastManager arRaycastManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject highlightedIndicator;

    private GameObject mappedEarth;
    private GameObject horizonPrefab;
    private List<Text> labelList = new List<Text>();
    private List<GameObject> pinList = new List<GameObject>();

    private float pinScale = 60f;
    private float labelScale = 1/10f;
    private float panelDistanceScale = 6f;

    // Start is called before the first frame update
    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        highlightedIndicator = Instantiate(indicatorPrefab);

    }

    // Update is called once per frame
    void Update()
    {
        // Update the indicator location
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // Context is always facing to the camera
        if (labelList.Count != 0) {
            foreach (Text t in labelList) {
                t.transform.LookAt(Camera.main.transform);
                t.transform.Rotate(new Vector3(0, 180, 0));
            }
        }

        //Panel is always facing to the camera
        if (panelPrefab.activeSelf)
        {
            panelPrefab.transform.LookAt(Camera.main.transform);
            panelPrefab.transform.Rotate(new Vector3(0, 180, 0));
        }

        // Detect tapping
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.CompareTag("Pin"))
                {
                    //raycastHit.collider.transform.gameObject.GetComponent<PinData>().TogglePinInformation();
                    panelPrefab.transform.position = raycastHit.collider.transform.position * panelDistanceScale;

                    // Fetch selected pin information
                    GLOBAL.LocationInfo selectedLocation = GLOBAL.LOCATION_DATABASE[int.Parse(raycastHit.collider.name)];

                    // Assign information to the panel
                    panelPrefab.transform.GetChild(1).gameObject.GetComponent<Text>().text = selectedLocation.name + ", " + selectedLocation.country;
                    //panelPrefab.transform.GetChild(1).gameObject.GetComponent<Text>().text = selectedLocation.coord.ToString();
                    panelPrefab.transform.GetChild(2).gameObject.GetComponent<Text>().text = selectedLocation.description;

                    //Texture2D thumbnail = Resources.Load("Textures/Thumbnail/"+ raycastHit.collider.name) as Texture2D;
                    panelPrefab.transform.GetChild(3).gameObject.GetComponent<RawImage>().texture = thumbnail[int.Parse(raycastHit.collider.name)];

                    // Show the panel
                    panelPrefab.SetActive(true);
                }
            }
        }

    }

    private void OnEnable()
    {
        mapperOptions.SetActive(true);
        highlightedIndicator.SetActive(true);
        canvasWorld.SetActive(true);
        if (mappedEarth != null) {
            mappedEarth.SetActive(true);
            horizonPrefab.SetActive(true);
        }
    }

    private void OnDisable()
    {
        mapperOptions.SetActive(false);
        highlightedIndicator.SetActive(false);
        canvasWorld.SetActive(false);
        panelPrefab.SetActive(false);
        if (mappedEarth != null) {
            mappedEarth.SetActive(false);
            horizonPrefab.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            Vector3 cameraForward = Camera.main.transform.forward;
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

        // Clear old variables
        Destroy(mappedEarth);
        Destroy(horizonPrefab);

        foreach (GameObject g in pinList) {
            Destroy(g);
        }
        pinList.Clear();

        foreach (Text t in labelList) {
            Destroy(t.gameObject);

        }
        labelList.Clear();

        // Initialise the earth object and the horizon
        // The horizon (range) is 5km x 5km as suggested for a 1.7m human
        mappedEarth = Instantiate(earthObjectToCopy, placementPose.position, Quaternion.identity);
        horizonPrefab = Instantiate(earthPlanePrefab, placementPose.position, Quaternion.identity);

        Transform pinGroup = mappedEarth.transform.GetChild(0);
        Transform layerGroup = mappedEarth.transform.GetChild(1);
        Transform refTop = mappedEarth.transform.GetChild(2);

        // Scale and reposition the Earth
        float scale = GLOBAL.EARTH_PREFAB_SCALE_TO_REAL;
        mappedEarth.transform.localScale = new Vector3(scale, scale, scale);
        mappedEarth.transform.Translate(new Vector3(0, -scale * GLOBAL.EARTH_PREFAB_RADIUS, 0));

        // Rotate the Earth so that the current position is facing up
        // Use the pre-calculated value
        mappedEarth.transform.rotation = GLOBAL.ROTATE_TO_TOP;

        // Rotate the Earth so that the current north direction is aligned geographically
        // First, find the current device facing direction (projection on z axis)
        Vector3 facingDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 prefabNorthDirection = Vector3.ProjectOnPlane(refTop.position - mappedEarth.transform.position, Vector3.up).normalized;

        // Then, compute the degree required to rotate the earth to the facing direction
        Quaternion rotateToFacingDirection = Quaternion.FromToRotation(prefabNorthDirection, facingDirection);

        // Lastly, compute the degree required to rotate the earth to the geographical north
        Quaternion rotateToGeographicalNorth = Quaternion.Euler(0, -Input.compass.trueHeading, 0);

        // 1. Use ROTATION
        // Compute the degree required to rotate the north direction to match the facing direction
        //float degree;
        //Vector3 axis;
        //rotateToFacingDirection.ToAngleAxis(out degree, out axis);
        //mappedEarth.transform.RotateAround(transform.position, Vector3.up, degree);

        // Then rotate the Earth again according to the heading direction from the GPS so that the north direction is matched both virtually and physically
        //mappedEarth.transform.Rotate(Vector3.up, -Input.compass.trueHeading, Space.World);

        // OR 2. Use QUATERNION
        //The steps need to be reversed to get the correct result
        mappedEarth.transform.rotation = rotateToGeographicalNorth * rotateToFacingDirection * GLOBAL.ROTATE_TO_TOP;

        // Scale and display each pin
        Vector3 referencePinPosition = pinGroup.GetChild(0).gameObject.transform.position;
        mappedEarth.SetActive(true);
        foreach (Transform pin in pinGroup)
        {
            if (pin.position != referencePinPosition)
            {

                GLOBAL.LocationInfo currentPinLocation = GLOBAL.LOCATION_DATABASE[int.Parse(pin.gameObject.name)];

                // Scale pins
                pin.position = pinScale * (pin.position - new Vector3(referencePinPosition.x, 0, referencePinPosition.z)).normalized;
                pin.localScale = new Vector3(1 / scale, 1 / scale, 1 / scale);

                // Place hovering labels
                Text label = Instantiate(labelPrefab, pin.position, Quaternion.identity, canvasWorld.transform);
                label.text = currentPinLocation.name + ", " + currentPinLocation.country + " (" + UTIL.DistanceBetweenLatLong(currentPinLocation.coord, GLOBAL.USER_LATLONG) + "km)";
                label.transform.localScale = new Vector3(labelScale, labelScale, labelScale);
                labelList.Add(label);
            }
            else {
                pin.gameObject.SetActive(false);
            }

            pinList.Add(pin.gameObject);

        }

        // Method A: Keep child pin and remove layer and reftop
        Destroy(refTop.gameObject);
        Destroy(layerGroup.gameObject);

        // Method B: Detach children and destory the object (buggy)
        //pinGroup.DetachChildren();
        //Destroy(mappedEarth);
    }
}
