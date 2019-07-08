using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class HoleSpawner : MonoBehaviour
{

    public GameObject spawnerOptions;

    public GameObject indicatorPrefab;
    public GameObject holePrefab;

    public GameObject canvasWorld;
    public LineRenderer lineRenderer;
    public GameObject panelPrefab;

    public Text debugOutput;

    private Pose placementPose;
    private ARRaycastManager arRaycastManager;
    private bool placementPoseIsValid = false;
    private bool placementIndicatorEnabled = true;

    private GameObject spawnedHole;
    private GameObject highlightedIndicator;

    private float panelDistanceScale = 40f;
    private float panelScale = 1 / 10f;
    private Vector3 yAxisOffset = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        //arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        highlightedIndicator = Instantiate(indicatorPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (placementIndicatorEnabled) {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
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
                if (raycastHit.collider.CompareTag("Layer"))
                {
                    debugOutput.text = "Hit " + raycastHit.collider.transform.parent.name;
                    //raycastHit.collider.transform.gameObject.GetComponent<PinData>().TogglePinInformation();
                    panelPrefab.transform.position = Vector3.ProjectOnPlane(raycastHit.collider.transform.position - Camera.main.transform.position, Vector3.up).normalized * panelDistanceScale + yAxisOffset;
                    //panelPrefab.transform.position = raycastHit.collider.transform.position + yAxisOffset;

                    GLOBAL.LayerInfo selectedLayer = GLOBAL.LAYER_INFO[int.Parse(raycastHit.collider.transform.parent.name)];

                    // Assign information to the panel
                    panelPrefab.transform.Find("Label_StructureName").GetComponent<Text>().text = selectedLayer.term;
                    panelPrefab.transform.Find("Label_StructureExtraInfo").GetComponent<Text>().text = selectedLayer.extra;
                    panelPrefab.transform.Find("Label_StructureDescription").GetComponent<Text>().text = selectedLayer.detail;

                    // Scale the panel
                    panelPrefab.transform.localScale = new Vector3(panelScale, panelScale);

                    // Rotate the panel to face the user
                    panelPrefab.transform.LookAt(Camera.main.transform);
                    panelPrefab.transform.Rotate(new Vector3(0, 180, 0));

                    // Show the panel
                    panelPrefab.SetActive(true);

                    //
                    lineRenderer.SetPosition(0, raycastHit.collider.transform.position);
                    lineRenderer.SetPosition(1, panelPrefab.transform.position);
                    lineRenderer.gameObject.SetActive(true);
                }
            }
        }

    }

    private void OnDisable()
    {
        if (spawnedHole != null) {
            spawnedHole.SetActive(false);
        }
        if (highlightedIndicator != null) {
            highlightedIndicator.SetActive(false);
        }
        spawnerOptions.SetActive(false);
        canvasWorld.SetActive(false);
        panelPrefab.SetActive(false);
        lineRenderer.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (spawnedHole !=null) {
            spawnedHole.SetActive(true);
        }
        if (highlightedIndicator != null) {
            if (placementIndicatorEnabled) {
                highlightedIndicator.SetActive(true);
            }
        }
        spawnerOptions.SetActive(true);
        canvasWorld.SetActive(true);
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

    private void UpdatePlacementIndicator() { 
        if (placementPoseIsValid) {
            // Show the indicator if the pose is valid and rotate it to match the view
            highlightedIndicator.SetActive(true);
            highlightedIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else {
            // Otherwise hide it
            highlightedIndicator.SetActive(false);
        }
    }

    public void SpawnHole() {
        if (placementPoseIsValid)
        {
            lineRenderer.gameObject.SetActive(false);
            panelPrefab.SetActive(false);

            // Only spawn one hole
            Destroy(spawnedHole);
            spawnedHole = Instantiate(holePrefab, placementPose.position, placementPose.rotation);
        }
    }

    public void TogglePlacementIndicator(bool active) {
        placementIndicatorEnabled = active;
        highlightedIndicator.SetActive(active);
    }
}
