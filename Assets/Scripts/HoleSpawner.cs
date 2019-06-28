using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class HoleSpawner : MonoBehaviour
{
    public GameObject spawnButton;

    #region Prefab
    public GameObject indicatorPrefab;
    public GameObject holePrefab;
    #endregion

    //private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private ARRaycastManager arRaycastManager;
    private bool placementPoseIsValid = false;

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
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    private void OnDisable()
    {
        if (spawnedHole != null) {
            spawnedHole.SetActive(false);
        }
        if (highlightedIndicator != null) {
            highlightedIndicator.SetActive(false);
        }
        spawnButton.SetActive(false);
    }

    private void OnEnable()
    {
        if (spawnedHole !=null) {
            spawnedHole.SetActive(true);
        }
        if (highlightedIndicator != null) {
            highlightedIndicator.SetActive(true);
        }
        spawnButton.SetActive(true);
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


}
