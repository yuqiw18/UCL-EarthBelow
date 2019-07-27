using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;

public class EarthMapper : MonoBehaviour
{
    public GameObject mapperOptions;

    public GameObject indicatorPrefab;
    public GameObject earthObjectToCopy;
    public GameObject earthHorizonPrefab;

    public GameObject canvasWorld;
    public GameObject labelPrefab;
    public GameObject panelPrefab;
    public GameObject landmarkPrefab;

    public Text[] debugOutput;

    public UnityEvent onClick;

    public Sprite[] cityLandmark;

    private ARRaycastManager arRaycastManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject highlightedIndicator;
    private bool placementIndicatorEnabled = true;

    private GameObject mappedEarth;
    private GameObject earthHorizon;
    private List<GameObject> labelList = new List<GameObject>();
    private List<GameObject> landmarkList = new List<GameObject>();

    private float pinDistanceScale = 128f;
    private float labelScale = 1/5f;
    private float panelScale = 1/3f;

    // Start is called before the first frame update
    void Start(){
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        highlightedIndicator = Instantiate(indicatorPrefab);
    }

    // Update is called once per frame
    void Update()
    {

        debugOutput[0].text = Screen.height.ToString();

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

        if (landmarkList.Count != 0)
        {
            foreach (GameObject l in landmarkList)
            {
                l.transform.LookAt(Camera.main.transform);
                l.transform.Rotate(new Vector3(0, 180, 0));
            }
        }

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
                // Display city profile
                if (raycastHit.collider.CompareTag("Pin"))
                {
                    // Display the panel before the other sprites by shifting a very small value so that it is not occluded
                    panelPrefab.transform.position = raycastHit.collider.transform.position * 0.99f;

                    // Fetch selected landmark information
                    GLOBAL.LocationInfo selectedLocation = GLOBAL.LOCATION_DATABASE[int.Parse(raycastHit.collider.name)];

                    // Assign information to the panel
                    panelPrefab.transform.Find("Label_CityName").GetComponent<Text>().text = selectedLocation.name;
                    panelPrefab.transform.Find("Label_CityCountry").GetComponent<Text>().text = selectedLocation.country + "";
                    panelPrefab.transform.Find("Label_CityDescription").GetComponent<Text>().text = selectedLocation.description;

                    StartCoroutine(LoadImageToSprite(Path.Combine(Application.streamingAssetsPath, "Images/CityThumbnails/", UTIL.FileNameParser(selectedLocation.name) + ".png"), "Image_CityLandmark"));
                    StartCoroutine(LoadImageToSprite(Path.Combine(Application.streamingAssetsPath, "Images/Flags/", UTIL.FileNameParser(selectedLocation.country) + ".png"), "Image_CountryFlag"));

                    // Scale the panel
                    panelPrefab.transform.localScale = new Vector2(panelScale, panelScale);

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
        if (earthHorizon != null)
        {
            earthHorizon.SetActive(true);
        }
    }

    private void OnDisable()
    {
        mapperOptions.SetActive(false);
        highlightedIndicator.SetActive(false);
        canvasWorld.SetActive(false);
        panelPrefab.SetActive(false);
        if (earthHorizon != null)
        {
            earthHorizon.SetActive(false);
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
        Destroy(earthHorizon);
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

        #region EARTH_MAPPING
        // Initialise the earth object and the horizon
        // The horizon (range) is 5km x 5km as suggested for a 1.7m human
        Vector3 refOrigin = placementPose.position;
        mappedEarth = Instantiate(earthObjectToCopy, refOrigin, Quaternion.identity);
        earthHorizon = Instantiate(earthHorizonPrefab, refOrigin, Quaternion.identity);

        Transform pinGroup = mappedEarth.transform.Find("Group_Pins");
        Transform refTop = mappedEarth.transform.Find("Ref_Top");

        // Scale and reposition the Earth
        mappedEarth.transform.localScale = new Vector3(GLOBAL.EARTH_PREFAB_SCALE_TO_REAL, GLOBAL.EARTH_PREFAB_SCALE_TO_REAL, GLOBAL.EARTH_PREFAB_SCALE_TO_REAL);
        
        // Rotate the Earth so that the current position is facing up so that we can do the following calculations
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
        // Those steps need to be combined backwards to get the correct result
        mappedEarth.transform.rotation = rotateToGeographicalNorth * rotateToFacingDirection * GLOBAL.ROTATE_TO_TOP;

        // Fix the floating point imprecision issue due to the large scale
        Vector3 impreciseRefPosition = pinGroup.GetChild(0).gameObject.transform.position;
        float preciseScaleFactor = GLOBAL.EARTH_CRUST_RADIUS / impreciseRefPosition.y;

        foreach (Transform pin in pinGroup) {
            pin.position *= preciseScaleFactor;
        }

        // Shift the mapped earth down so that the y value of current position is the same as the selected surface for mapping
        mappedEarth.transform.position -= new Vector3(0, GLOBAL.EARTH_CRUST_RADIUS, 0);
        #endregion

        #region PIN_ASSIGNMENT
        Vector3 preciseRefPosition = pinGroup.GetChild(0).gameObject.transform.position;
        foreach (Transform pin in pinGroup)
        {
            // Display landmarks excluding the current position
            if (pin.position != preciseRefPosition)
            {

                GLOBAL.LocationInfo currentPinLocation = GLOBAL.LOCATION_DATABASE[int.Parse(pin.gameObject.name)];
                float geoDistance = UTIL.DistanceBetweenLatLong(currentPinLocation.coord, GLOBAL.USER_LATLONG);

                // Compute the relative position
                //pin.position = (pin.position - preciseRefPosition).normalized;
                pin.position = pinDistanceScale * (pin.position - refOrigin).normalized;

                #region CITY_LANDMARK_IMAGE
                // Instantiate the landmark prefab
                GameObject landmark = Instantiate(landmarkPrefab, pin.position, Quaternion.identity, canvasWorld.transform);

                // Set the landmark image
                Transform landmarkImage = landmark.transform.Find("UI_Landmark_Image");

                //StartCoroutine(LoadImageAsSprite(Path.Combine(Application.streamingAssetsPath, "LandmarkIcons/", UTIL.FileNameParser(currentPinLocation.name) + ".png"), "Image_CityLandmark"));

                landmarkImage.GetComponent<Image>().sprite = cityLandmark[int.Parse(pin.gameObject.name)];


                landmarkImage.name = pin.name;

                // Change the color based on the geographical distance
                if (geoDistance > 5)
                {
                    landmarkImage.GetComponent<Image>().color = new Color32(51, 102, 0, 255);
                    landmark.transform.localScale = new Vector2(0.1f, 0.1f);
                }
                else
                {
                    landmarkImage.GetComponent<Image>().color = new Color32(0, 255, 0, 255);
                    float rescale = 0.3f - geoDistance * 0.04f;
                    landmark.transform.localScale = new Vector2(rescale, rescale);
                }

                // Rescale the landmark
                

                landmarkList.Add(landmark);
                #endregion

                #region CITY_INFO_LABEL
                // Place hovering labels
                GameObject label = Instantiate(labelPrefab, pin.position, Quaternion.identity, canvasWorld.transform);
                label.transform.Find("Label_LandmarkName").GetComponent<Text>().text = currentPinLocation.name + ", " + currentPinLocation.country;

                // Display the indicator based on the geographical distance
                if (geoDistance > 5)
                {
                    label.transform.Find("Label_LandmarkDistance").GetComponent<Text>().text = "▽ ";
                }
                else
                {
                    label.transform.Find("Label_LandmarkDistance").GetComponent<Text>().text = "▲ ";
                }

                // Display the geographical distance
                label.transform.Find("Label_LandmarkDistance").GetComponent<Text>().text += (geoDistance.ToString() + "km");

                // Rescale the label
                label.transform.localScale = new Vector3(labelScale, labelScale, labelScale);

                labelList.Add(label);
                #endregion
            }
            else
            {
                pin.gameObject.SetActive(false);
            }
        }

        panelPrefab.transform.SetAsLastSibling();
        #endregion

        // Destroy it
        Destroy(mappedEarth);

    }

    public void TogglePlacementIndicator(bool toggle)
    {
        placementIndicatorEnabled = toggle;
        highlightedIndicator.SetActive(toggle);
    }

    // Load a local image and return as a 2D sprite
    private IEnumerator LoadImageToSprite(string filePath, string targetSprite)
    {
        byte[] imageData;
        Texture2D texture = new Texture2D(2, 2);

        // Read bytes using UnityWebRequest or File.ReadAllBytes
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            imageData = www.downloadHandler.data;
        }
        else
        {
            imageData = File.ReadAllBytes(filePath);
        }

        // Load raw data into Texture2D 
        texture.LoadImage(imageData);

        // Convert Texture2D to Sprite
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), pivot, 100.0f);

        panelPrefab.transform.Find(targetSprite).gameObject.GetComponent<Image>().sprite = sprite;

    }
}
