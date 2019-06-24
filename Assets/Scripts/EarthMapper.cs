using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class EarthMapper : MonoBehaviour
{

    public GameObject mapButton;

    public GameObject indicatorPrefab;
    public GameObject earthObjectToCopy;
    public GameObject earthPlanePrefab;

    public GameObject canvasWorld;
    public Text labelPrefab;

    private ARRaycastManager arRaycastManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject highlightedIndicator;

    private GameObject mappedEarth;
    private GameObject mappedPlane;
    private List<Text> labelList = new List<Text>();

    private float pinScale = 60f;
    private float labelScale = 10f;

    // Start is called before the first frame update
    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        highlightedIndicator = Instantiate(indicatorPrefab);

    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if (labelList.Count != 0) {
            foreach (Text t in labelList) {
                t.transform.LookAt(Camera.main.transform);
                t.transform.Rotate(new Vector3(0, 180, 0));
            }
        }

    }

    private void OnEnable()
    {
        mapButton.SetActive(true);
        highlightedIndicator.SetActive(true);
        canvasWorld.SetActive(true);
        if (mappedEarth != null) {
            mappedEarth.SetActive(true);
        }
    }

    private void OnDisable()
    {
        mapButton.SetActive(false);
        highlightedIndicator.SetActive(false);
        canvasWorld.SetActive(false);
        if (mappedEarth != null) {
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

        // Clear old variables
        Destroy(mappedEarth);
        Destroy(mappedPlane);
        foreach (Text t in labelList) {
            Destroy(t.gameObject);

        }
        labelList.Clear();

        // Initialise
        mappedEarth = Instantiate(earthObjectToCopy, placementPose.position, Quaternion.identity);
        mappedPlane = Instantiate(earthPlanePrefab, placementPose.position, Quaternion.identity);

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
        foreach (Transform pin in pinGroup)
        {
            if (pin.position != referencePinPosition)
            {
                // Scale pins
                pin.position = pinScale * (pin.position - new Vector3(referencePinPosition.x, 0, referencePinPosition.z)).normalized;
                pin.localScale = new Vector3(1 / scale, 1 / scale, 1 / scale);

                // Place hovering labels
                Text label = Instantiate(labelPrefab, pin.position, Quaternion.identity, canvasWorld.transform);
                label.text = pin.gameObject.name;
                label.transform.localScale = new Vector3(1 / labelScale, 1 / labelScale, 1 / labelScale);
                labelList.Add(label);
            }
        }

        foreach (Transform t in layerGroup) {
            t.gameObject.SetActive(false);
        }
    }
}
