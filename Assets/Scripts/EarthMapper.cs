using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class EarthMapper : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public GameObject earthToReference;
    public GameObject earthHorizonPrefab;

    public GameObject canvasWorld;
    public Sprite spriteNearPin, spriteFarPin;
    public GameObject labelPrefab;
    public GameObject profilePanel;
    public GameObject landmarkPrefab;

    public GameObject legendPanel;
    public GameObject panoramaPanel;

    public GameObject viewDistanceSlider;
    public Text viewDistanceLabel;
    public GameObject viewDistanceOccluder;

    private ARRaycastManager arRaycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject highlightedIndicator;
    private bool placementIndicatorEnabled = true;

    private GameObject referenceEarth, mappedEarth;
    private Vector3 referenceOrigin = Vector3.zero;

    private List<GameObject> labelList = new List<GameObject>();
    private List<GameObject> landmarkList = new List<GameObject>();
    
    private readonly float UILabelScale = 1.0f;
    private readonly float UIPanelScaleDynamic = 1.25f;
    private readonly float UIPanelScaleFixed = 0.00125f;

    private string url;
    private Sprite defaultFlag, defaultLandmark;

    void Start(){
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        highlightedIndicator = Instantiate(indicatorPrefab);
        defaultLandmark = profilePanel.transform.Find("Image_CityLandmark").gameObject.GetComponent<Image>().sprite;
        defaultFlag = profilePanel.transform.Find("Image_CountryFlag").gameObject.GetComponent<Image>().sprite;
    }

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

        if (landmarkList.Count != 0)
        {
            foreach (GameObject l in landmarkList)
            {
                l.transform.LookAt(Camera.main.transform);
                l.transform.Rotate(new Vector3(0, 180, 0));
            }
        }

        if (profilePanel.activeSelf)
        {
            profilePanel.transform.LookAt(Camera.main.transform);
            profilePanel.transform.Rotate(new Vector3(0, 180, 0));
        }

        // Detect tapping
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                #region SHOW_CITY_PROFILE
                // Display city profile
                if (raycastHit.collider.CompareTag("Pin"))
                {
                    // Compute the scaling factor
                    float distanceScale = (raycastHit.collider.transform.position - referenceOrigin).magnitude/1000;

                    // Display the panel before the other sprites by shifting a very small value so that it is not occluded
                    // A. Dynamic distance
                    //profilePanel.transform.position = raycastHit.collider.transform.position * 0.99f;

                    // B. Fixed distance
                    profilePanel.transform.position = Camera.main.transform.position + 1.0f * (raycastHit.point - Camera.main.transform.position).normalized;

                    // Fetch selected landmark information
                    CORE.LocationInfo selectedLocation = CORE.LOCATION_DATABASE[int.Parse(raycastHit.collider.name)];

                    // Assign information to the panel
                    profilePanel.transform.Find("Label_CityName").GetComponent<Text>().text = selectedLocation.name;
                    profilePanel.transform.Find("Label_CityCountry").GetComponent<Text>().text = selectedLocation.country + "";
                    profilePanel.transform.Find("Label_CityDescription").GetComponent<Text>().text = selectedLocation.description;
                    profilePanel.transform.Find("Button_Portal").gameObject.SetActive(selectedLocation.panorama);

                    // Reset images just in case the target images are not available
                    profilePanel.transform.Find("Image_CityLandmark").gameObject.GetComponent<Image>().sprite = defaultLandmark;
                    profilePanel.transform.Find("Image_CountryFlag").gameObject.GetComponent<Image>().sprite = defaultFlag;

                    // Load images from local resources
                    StartCoroutine(CORE.LoadImageToSprite(Path.Combine(Application.streamingAssetsPath, CORE.IMAGE_FOLDER_CITY, CORE.FileNameParser(selectedLocation.name) + CORE.IMAGE_FORMAT), (result) =>
                    {
                        profilePanel.transform.Find("Image_CityLandmark").gameObject.GetComponent<Image>().sprite = result;
                    }));

                    StartCoroutine(CORE.LoadImageToSprite(Path.Combine(Application.streamingAssetsPath, CORE.IMAGE_FOLDER_FLAG, CORE.FileNameParser(selectedLocation.country) + CORE.IMAGE_FORMAT), (result) =>
                    {
                        profilePanel.transform.Find("Image_CountryFlag").gameObject.GetComponent<Image>().sprite = result;
                    }));

                    StartCoroutine(CORE.LoadImageToTexture(Path.Combine(Application.streamingAssetsPath, CORE.IMAGE_FOLDER_PANORAMA, CORE.FileNameParser(selectedLocation.name) + CORE.IMAGE_FORMAT), (result) =>
                    {
                        mappedEarth.transform.Find("Portal_Panorama").GetComponent<Renderer>().material.SetTexture("_Texture", result);
                    }));

                    panoramaPanel.transform.Find("Text").GetComponent<Text>().text = selectedLocation.landmark;

                    // Append formatted string to the url
                    url = Path.Combine(CORE.SEARCH_ENGINE_PATH, CORE.FileNameParser(selectedLocation.name));

                    // Scale the panel
                    // A. Dynamic scale
                    //profilePanel.transform.localScale = Vector3.one * UIPanelScaleDynamic * distanceScale;

                    // B. Fixed scale
                    profilePanel.transform.localScale = Vector3.one * UIPanelScaleFixed;

                    // Rotate the panel to face the user
                    profilePanel.transform.LookAt(Camera.main.transform);
                    profilePanel.transform.Rotate(new Vector3(0, 180, 0));

                    // Show the panel
                    profilePanel.SetActive(true);
                }
                #endregion
            }
        }
    }

    private void OnEnable()
    {
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
            viewDistanceOccluder.SetActive(true);
        }
        
    }

    private void OnDisable()
    {
        highlightedIndicator.SetActive(false);
        canvasWorld.SetActive(false);
        if (mappedEarth != null)
        {
            mappedEarth.SetActive(false);
            mappedEarth.SetActive(false);
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

        profilePanel.SetActive(false);

        // Clear old variables
        Destroy(mappedEarth);

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
        referenceOrigin = placementPose.position;
        referenceEarth = Instantiate(earthToReference, referenceOrigin, Quaternion.identity);
        mappedEarth = Instantiate(earthHorizonPrefab, referenceOrigin, Quaternion.identity);

        Transform pinGroup = referenceEarth.transform.Find("Group_Pins");
        Transform refTop = referenceEarth.transform.Find("Ref_Top");

        // Scale and reposition the Earth
        referenceEarth.transform.localScale = Vector3.one * CORE.EARTH_PREFAB_SCALE_TO_REAL;

        // Rotate the Earth so that the current position is facing up so that we can do the following calculations
        // Use the pre-calculated value
        referenceEarth.transform.rotation = CORE.ROTATE_TO_TOP;

        // Rotate the Earth so that the current north direction is aligned geographically
        // First, find the current device facing direction (projection on z axis)
        Vector3 facingDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 prefabNorthDirection = Vector3.ProjectOnPlane(refTop.position - referenceEarth.transform.position, Vector3.up).normalized;

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
        referenceEarth.transform.rotation = rotateToGeographicalNorth * rotateToFacingDirection * CORE.ROTATE_TO_TOP;

        // Fix the floating point imprecision issue due to the large scale
        Vector3 impreciseRefPosition = pinGroup.GetChild(0).gameObject.transform.position;
        float preciseScaleFactor = CORE.EARTH_CRUST_RADIUS / impreciseRefPosition.y;

        foreach (Transform pin in pinGroup) {
            pin.position *= preciseScaleFactor;
        }

        // Shift the mapped earth down so that the y value of current position is the same as the selected surface for mapping
        referenceEarth.transform.position -= new Vector3(0, CORE.EARTH_CRUST_RADIUS, 0);
        #endregion

        mappedEarth.transform.Find("Mapping_Border").rotation = referenceEarth.transform.rotation;

        #region PIN_ASSIGNMENT
        Vector3 preciseRefPosition = pinGroup.GetChild(0).gameObject.transform.position;
        foreach (Transform pin in pinGroup)
        {
            // Display landmarks excluding the current position
            if (pin.position != preciseRefPosition)
            {

                CORE.LocationInfo currentPinLocation = CORE.LOCATION_DATABASE[int.Parse(pin.gameObject.name)];
                float geoDistance = CORE.DistanceBetweenLatLong(currentPinLocation.coord, CORE.USER_LATLONG);
                float distanceScale = (pin.position - referenceOrigin).magnitude/1000;

                #region CITY_LANDMARK_IMAGE
                // Instantiate the landmark prefab
                GameObject landmark = Instantiate(landmarkPrefab, pin.position, Quaternion.identity, canvasWorld.transform);

                // Set the landmark image
                Transform landmarkImage = landmark.transform.Find("UI_Landmark_Image");
                landmarkImage.name = pin.name;

                // Rescale the landmark
                landmark.transform.localScale *= distanceScale;

                // Change the sprite based on the geographical distance
                if (geoDistance > 5)
                {
                    landmarkImage.GetComponent<Image>().sprite = spriteFarPin;
                }
                else
                {
                    landmarkImage.GetComponent<Image>().sprite = spriteNearPin;
                }

                // Fill the color
                landmarkImage.GetComponent<Image>().color = new Color32(0, 255, 0, 255);

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
                label.transform.localScale = Vector3.one * UILabelScale * distanceScale;
                labelList.Add(label);
                #endregion
            }
            else
            {
                pin.gameObject.SetActive(false);
            }
        }

        profilePanel.transform.SetAsLastSibling();
        #endregion

        legendPanel.SetActive(true);

        // Destroy it since it is no longer needed
        Destroy(referenceEarth);

    }

    public void TogglePlacementIndicator(bool toggle)
    {
        placementIndicatorEnabled = toggle;
        highlightedIndicator.SetActive(toggle);
    }

    public void OpenBrowser()
    {
        Application.OpenURL(url);
    }

    public void TogglePanorama(bool active)
    {
        if (active)
        {
            mappedEarth.transform.Find("Portal_Panorama").gameObject.SetActive(true);
            mappedEarth.transform.Find("Mapping_Grid").gameObject.SetActive(false);
            mappedEarth.transform.Find("Mapping_Border").gameObject.SetActive(false);
            mappedEarth.transform.Find("Mapping_Reference").gameObject.SetActive(false);
            canvasWorld.SetActive(false);
            viewDistanceSlider.SetActive(false);
            viewDistanceOccluder.SetActive(false);
        }
        else
        {
            mappedEarth.transform.Find("Portal_Panorama").gameObject.SetActive(false);
            mappedEarth.transform.Find("Mapping_Grid").gameObject.SetActive(true);
            mappedEarth.transform.Find("Mapping_Border").gameObject.SetActive(true);
            mappedEarth.transform.Find("Mapping_Reference").gameObject.SetActive(true);
            canvasWorld.SetActive(true);
            viewDistanceSlider.SetActive(true);
            viewDistanceOccluder.SetActive(true);
        } 
    }

    public void AdjustRenderingDistance(float distanceScaleFactor)
    {
        float scale = distanceScaleFactor * 4;
        viewDistanceOccluder.transform.localScale = Vector3.one * scale * 1000;
        viewDistanceLabel.text = ((int)scale).ToString() + "km";
    }
}
