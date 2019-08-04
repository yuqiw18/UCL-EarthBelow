using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class PitSpawner : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public GameObject pitPrefab;
    public GameObject detailPanel;

    private Pose placementPose;
    private ARRaycastManager arRaycastManager;
    private bool placementPoseIsValid = false;
    private bool placementIndicatorEnabled = true;

    private GameObject spawnedPit, highlightedIndicator;
    private float pitScale = 1.0f;

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

        // Detect tapping
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.CompareTag("Layer"))
                {
                    CORE.PlanetInfo selectedLayer = CORE.PLANET_DATABASE[int.Parse(raycastHit.collider.transform.parent.name)];

                    detailPanel.transform.Find("Structure").Find("Text").GetComponent<Text>().text = selectedLayer.structure;
                    detailPanel.transform.Find("State").Find("Text").GetComponent<Text>().text = selectedLayer.state;
                    detailPanel.transform.Find("Thickness").Find("Text").GetComponent<Text>().text = selectedLayer.thickness;
                    detailPanel.transform.Find("Temperature").Find("Text").GetComponent<Text>().text = selectedLayer.temperature;
                    detailPanel.transform.Find("Composition").Find("Text").GetComponent<Text>().text = selectedLayer.composition;
                    detailPanel.transform.Find("Highlight").Find("Text").GetComponent<Text>().text = selectedLayer.highlight;

                    // Show the panel
                    detailPanel.SetActive(true);
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            if (spawnedPit != null)
            {
                if (spawnedPit.GetComponent<Renderer>().isVisible)
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
            detailPanel.SetActive(false);

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
            // Reposition the hole to the target surface after rescaling
            Vector3 tempPosition = spawnedPit.transform.position;
            spawnedPit.transform.localScale = new Vector3(pitScale, pitScale, pitScale);
            spawnedPit.transform.position = tempPosition;
        }
    }
}
