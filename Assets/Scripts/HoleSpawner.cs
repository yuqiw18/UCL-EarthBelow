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
    public GameObject labelPrefab;
    public GameObject panelPrefab;

    public Text debugOutput;

    private Pose placementPose;
    private ARRaycastManager arRaycastManager;
    private bool placementPoseIsValid = false;
    private bool placementIndicatorEnabled = true;

    private GameObject spawnedHole;
    private GameObject highlightedIndicator;

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


        // Detect tapping
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                debugOutput.text = raycastHit.collider.transform.parent.name;
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
