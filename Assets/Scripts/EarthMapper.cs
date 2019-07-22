 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.Events;

public class EarthMapper : MonoBehaviour
{
    public GameObject mapperOptions;

    public GameObject indicatorPrefab;
    public GameObject earthObjectToCopy;
    public GameObject earthPlanePrefab;
    public GameObject fakeEarthPrefab;

    public GameObject canvasWorld;
    public GameObject labelPrefab;
    public GameObject panelPrefab;

    public LineRenderer lineRenderer;

    public Text[] debugOutput;

    public UnityEvent onClick;

    public Texture2D[] thumbnail;
    public GameObject[] landmark;

    private ARRaycastManager arRaycastManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject highlightedIndicator;
    private bool placementIndicatorEnabled = true;

    private GameObject mappedEarth;
    private GameObject horizonPrefab;
    private GameObject fakeEarthHorizonPrefab;
    private List<GameObject> labelList = new List<GameObject>();
    private List<GameObject> landmarkList = new List<GameObject>();

    private float pinDistanceScale = 100f;
    private float labelScale = 2/10f;
    private float panelDistanceScale = 0.1f;
    private float panelScale = 1 / 6f;

    // Start is called before the first frame update
    void Start(){
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        highlightedIndicator = Instantiate(indicatorPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the indicator location
        if (placementIndicatorEnabled)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }
        
        // Context is always facing to the camera
        if (labelList.Count != 0)
        {
            foreach (GameObject t in labelList)
            {
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
                    panelPrefab.transform.position = raycastHit.collider.transform.position - raycastHit.collider.transform.position * panelDistanceScale;

                    // Fetch selected pin information
                    GLOBAL.LocationInfo selectedLocation = GLOBAL.LOCATION_DATABASE[int.Parse(raycastHit.collider.name)];

                    // Assign information to the panel
                    panelPrefab.transform.Find("Label_CityName").GetComponent<Text>().text = selectedLocation.name + ", " + selectedLocation.country;
                    panelPrefab.transform.Find("Label_CityCoord").GetComponent<Text>().text = "(" + selectedLocation.coord.x.ToString() + "° N, " + selectedLocation.coord.y.ToString() + "° E)";
                    panelPrefab.transform.Find("Label_CityDescription").GetComponent<Text>().text = selectedLocation.description;

                    //Texture2D thumbnail = Resources.Load("Textures/Thumbnail/"+ raycastHit.collider.name) as Texture2D;
                    panelPrefab.transform.Find("Image_CityLandmark").gameObject.GetComponent<RawImage>().texture = thumbnail[int.Parse(raycastHit.collider.name)];

                    // Scale the panel
                    panelPrefab.transform.localScale = new Vector3(panelScale, panelScale);

                    // Rotate the panel to face the user
                    panelPrefab.transform.LookAt(Camera.main.transform);
                    panelPrefab.transform.Rotate(new Vector3(0, 180, 0));

                    // Show the panel
                    panelPrefab.SetActive(true);
                }
            }
        }
    }

    private void OnEnable()
    {
        mapperOptions.SetActive(true);
        canvasWorld.SetActive(true);
        if (highlightedIndicator != null)
        {
            if (placementIndicatorEnabled)
            {
                highlightedIndicator.SetActive(true);
            }
        }
        if (mappedEarth != null)
        {
            mappedEarth.SetActive(true);
            horizonPrefab.SetActive(true);
            fakeEarthHorizonPrefab.SetActive(true);
        }
    }

    private void OnDisable()
    {
        mapperOptions.SetActive(false);
        highlightedIndicator.SetActive(false);
        canvasWorld.SetActive(false);
        panelPrefab.SetActive(false);
        if (mappedEarth != null)
        {
            mappedEarth.SetActive(false);
            horizonPrefab.SetActive(false);
            fakeEarthHorizonPrefab.SetActive(false);
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
        Destroy(fakeEarthHorizonPrefab);

        foreach (GameObject l in landmarkList)
        {
            Destroy(l);
        }
        landmarkList.Clear();

        foreach (GameObject t in labelList)
        {
            Destroy(t);
        }
        labelList.Clear();

        // Initialise the earth object and the horizon
        // The horizon (range) is 5km x 5km as suggested for a 1.7m human
        Vector3 refOrigin = placementPose.position;
        mappedEarth = Instantiate(earthObjectToCopy, refOrigin, Quaternion.identity);
        horizonPrefab = Instantiate(earthPlanePrefab, refOrigin, Quaternion.identity);
        fakeEarthHorizonPrefab = Instantiate(fakeEarthPrefab, refOrigin, Quaternion.identity);

        Transform pinGroup = mappedEarth.transform.Find("Group_Pins");
        Transform layerGroup = mappedEarth.transform.Find("Group_Layers");
        Transform refTop = mappedEarth.transform.Find("Ref_Top");
        Transform landmarkGroup = mappedEarth.transform.Find("Group_Landmarks");

        // Scale and reposition the Earth
        float scale = GLOBAL.EARTH_PREFAB_SCALE_TO_REAL;

        mappedEarth.transform.localScale = new Vector3(scale, scale, scale);
        
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
        Vector3 impreciseRefPosition = pinGroup.GetChild(0).gameObject.transform.position;
        float preciseScaleFactor = GLOBAL.EARTH_CRUST_RADIUS / impreciseRefPosition.y;

        foreach (Transform pin in pinGroup) {
            pin.position *= preciseScaleFactor;
        }

        //mappedEarth.transform.Translate(new Vector3(0, -scale * GLOBAL.EARTH_PREFAB_RADIUS, 0));

        mappedEarth.transform.position -= new Vector3(0, scale * GLOBAL.EARTH_PREFAB_RADIUS, 0);

        Vector3 preciseRefPosition = pinGroup.GetChild(0).gameObject.transform.position;

        //debugOutput[0].text = preciseRefPosition.ToString();
        //debugOutput[1].text = refOrigin.ToString();

        debugOutput[0].text = mappedEarth.transform.position.ToString();
        debugOutput[1].text = fakeEarthHorizonPrefab.transform.position.ToString();

        mappedEarth.SetActive(true);
        foreach (Transform pin in pinGroup)
        {
            if (pin.position != preciseRefPosition)
            {

                GLOBAL.LocationInfo currentPinLocation = GLOBAL.LOCATION_DATABASE[int.Parse(pin.gameObject.name)];
                int geoDistance = UTIL.DistanceBetweenLatLong(currentPinLocation.coord, GLOBAL.USER_LATLONG);

                //pin.position = (pin.position - preciseRefPosition).normalized;

                // Only scale pins that are below the horizon
                if (geoDistance >= 5)
                {
                    pin.position = pinDistanceScale * (pin.position - refOrigin).normalized;
                    pin.localScale = new Vector3(2 / scale, 2 / scale, 2 / scale);
                }
                else
                {
                    pin.position = geoDistance * 10 * (pin.position - refOrigin).normalized;
                    pin.localScale = new Vector3(0.5f / scale, 0.5f / scale, 0.5f / scale);

                    debugOutput[2].text = pin.position.ToString();

                    lineRenderer.SetPosition(0, pin.position);
                    lineRenderer.SetPosition(1, mappedEarth.transform.position);
                    lineRenderer.gameObject.SetActive(true);
                }

                GameObject l = Instantiate(landmark[int.Parse(pin.gameObject.name)], pin.position, placementPose.rotation, landmarkGroup);
                l.transform.localScale = pin.localScale;
                l.name = pin.gameObject.name;

                if (geoDistance >= 5)
                {
                    l.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
                }
                else
                {

                }

                landmarkList.Add(l);

                // Place hovering labels
                GameObject label = Instantiate(labelPrefab, pin.position, Quaternion.identity, canvasWorld.transform);
                label.transform.Find("Label_PinName").GetComponent<Text>().text = currentPinLocation.name + ", " + currentPinLocation.country;

                if (geoDistance >= 5)
                {
                    label.transform.Find("Label_PinDistance").GetComponent<Text>().text = "▽ ";
                    label.transform.localScale = new Vector3(labelScale, labelScale, labelScale);
                    
                }
                else {
                    label.transform.Find("Label_PinDistance").GetComponent<Text>().text = "▲ ";
                    label.transform.localScale = new Vector3(labelScale/( 10 / geoDistance), labelScale/ (10 / geoDistance), labelScale/ (10 / geoDistance));
                    //l.GetComponent<SnapToSurface>().SetGeoDistance(geoDistance);
                }

                label.transform.Find("Label_PinDistance").GetComponent<Text>().text += (geoDistance.ToString() + "km");

                //label.transform.localScale = new Vector3(labelScale, labelScale, labelScale);
                labelList.Add(label);
            }
            else
            {
                pin.gameObject.SetActive(false);
            }
        }

        foreach (GameObject l in landmarkList)
        {
            l.GetComponent<SnapToSurface>().SetSnapDirection((mappedEarth.transform.position - l.transform.position).normalized);
            l.GetComponent<SnapToSurface>().EnableSnap();
        }

        panelPrefab.transform.SetAsLastSibling();

        // Method A: Keep child pin and remove layer and reftop
        Destroy(pinGroup.gameObject);
        Destroy(refTop.gameObject);
        Destroy(layerGroup.gameObject);

        // Method B: Detach children and destory the object (buggy)
        //pinGroup.DetachChildren();
        //Destroy(mappedEarth);
    }

    public void TogglePlacementIndicator(bool toggle)
    {
        placementIndicatorEnabled = toggle;
        highlightedIndicator.SetActive(toggle);
    }
}
