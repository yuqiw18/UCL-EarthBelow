using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Events;
using UnityEngine.UI;

public class PitSpawner : MonoBehaviour
{
    public GameObject spawnerOptions;

    public GameObject indicatorPrefab;
    public GameObject pitPrefab;

    public GameObject canvasWorld;
    public GameObject panelPrefab;

    public UnityEvent onClick;

    private Pose placementPose;
    private ARRaycastManager arRaycastManager;
    private bool placementPoseIsValid = false;
    private bool placementIndicatorEnabled = true;

    private GameObject spawnedPit;
    private GameObject highlightedIndicator;
    private float pitScale = 1.0f;

    private float panelDistanceScale = 40f;
    private float panelScale = 1 / 10f;
    private Vector3 yAxisOffset = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        highlightedIndicator = Instantiate(indicatorPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (placementIndicatorEnabled)
        {
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
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                //if (raycastHit.collider.CompareTag("Layer"))
                //{
                //    panelPrefab.transform.position = Vector3.ProjectOnPlane(raycastHit.collider.transform.position - Camera.main.transform.position, Vector3.up).normalized * panelDistanceScale + yAxisOffset;
  
                //    GLOBAL.LayerInfo selectedLayer = GLOBAL.LAYER_INFO[int.Parse(raycastHit.collider.transform.parent.name)];

                //    // Assign information to the panel
                //    panelPrefab.transform.Find("Label_StructureName").GetComponent<Text>().text = selectedLayer.term;
                //    panelPrefab.transform.Find("Label_StructureExtraInfo").GetComponent<Text>().text = selectedLayer.extra;
                //    panelPrefab.transform.Find("Label_StructureDescription").GetComponent<Text>().text = selectedLayer.detail;

                //    // Scale the panel
                //    panelPrefab.transform.localScale = new Vector3(panelScale, panelScale);

                //    // Rotate the panel to face the user
                //    panelPrefab.transform.LookAt(Camera.main.transform);
                //    panelPrefab.transform.Rotate(new Vector3(0, 180, 0));

                //    // Show the panel
                //    panelPrefab.SetActive(true);

                //}
            }
        }

        // Detect scaling
        if (Input.touchCount == 2)
        {
            if (spawnedPit != null)
            {
                // Get the touch
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
                Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

                float previousTouchDeltaMagnitude = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
                float currentTouchDeltaMagnitude = (firstTouch.position - secondTouch.position).magnitude;

                float touchMagnitudeDifference = previousTouchDeltaMagnitude - currentTouchDeltaMagnitude;

                Vector3 tempPosition = spawnedPit.transform.position;

                pitScale += -0.005f * touchMagnitudeDifference;
                if (pitScale < 1)
                {
                    pitScale = 1;
                }

                // Adjust the tiling of materials so that they do not appear pixelated/blurry while scaling up
                spawnedPit.transform.Find("Pit").Find("Structure").GetComponent<Renderer>().materials[1].SetFloat("_Tiling", 3 + pitScale);
                spawnedPit.transform.Find("Pit").Find("Structure").GetComponent<Renderer>().materials[3].SetFloat("_Tiling", 3 + pitScale);
                spawnedPit.transform.Find("Pit").Find("PitEdge").GetComponent<Renderer>().material.SetFloat("_Tiling", 3 + pitScale);

                // Awayls snap the pit to the surface
                spawnedPit.transform.localScale = new Vector3(pitScale, pitScale, pitScale);
                spawnedPit.transform.position = tempPosition;
            }
        }
    }

    private void OnDisable()
    {
        if (spawnedPit != null)
        {
            spawnedPit.SetActive(false);
        }
        if (highlightedIndicator != null)
        {
            highlightedIndicator.SetActive(false);
        }
        spawnerOptions.SetActive(false);
        canvasWorld.SetActive(false);
        panelPrefab.SetActive(false);
    }

    private void OnEnable()
    {
        if (spawnedPit != null)
        {
            spawnedPit.SetActive(true);
        }
        if (highlightedIndicator != null)
        {
            if (placementIndicatorEnabled)
            {
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

    private void UpdatePlacementIndicator()
    { 
        if (placementPoseIsValid)
        {
            // Show the indicator if the pose is valid and rotate it to match the view
            highlightedIndicator.SetActive(true);
            highlightedIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else {
            // Otherwise hide it
            highlightedIndicator.SetActive(false);
        }
    }

    public void SpawnHole()
    {
        if (placementPoseIsValid)
        {
            panelPrefab.SetActive(false);

            // Only spawn one hole
            Destroy(spawnedPit);
            spawnedPit = Instantiate(pitPrefab, placementPose.position, placementPose.rotation);
            RescalePit();
        }
    }

    public void TogglePlacementIndicator(bool active)
    {
        placementIndicatorEnabled = active;
        highlightedIndicator.SetActive(active); 
    }

    private void RescalePit()
    {
        if (spawnedPit != null)
        {
            Vector3 tempPosition = spawnedPit.transform.position;
            spawnedPit.transform.localScale = new Vector3(pitScale, pitScale, pitScale);
            spawnedPit.transform.position = tempPosition;
        }
    }

}
