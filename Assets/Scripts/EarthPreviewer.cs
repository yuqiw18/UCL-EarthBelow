using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EarthPreviewer : MonoBehaviour
{
    public GameObject previewerOptions;
    public GameObject earthObject;
    public GameObject pinPrefab;
    public Material[] earthMaterialList;

    private Vector3 earthCenter;
    private List<Vector3> tempPinCoord = new List<Vector3>();
    private List<GameObject> pinList = new List<GameObject>();

    private GameObject pinGroup;
    private bool showMagneticField = false;
    private bool showCountryBorder = false;

    private Transform earthTransformPoint;
    private Transform earthSpawnPoint;
    private readonly float rotationSpeed = 0.25f;
    private readonly float scaleSpeed = 0.005f;

    private Renderer earthRenderer;
    private bool inTransition = false;
    private int currentMaterialIndex;

    private bool spawned = false;

    private void Awake()
    {
        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, "Database/", "location.json"), (data) => {
            // Load data into the serializable class and transfer it to the non-serialisable list
            CORE.LocationDatabase locationDatabase = JsonUtility.FromJson<CORE.LocationDatabase>(data);
            CORE.LOCATION_DATABASE = locationDatabase.serializableList;
        }));

        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, "Database/", "planet.json"), (data) => {
            // Load data into the serializable class and transfer it to the non-serialisable list
            CORE.PlanetDatabase planetDatabase = JsonUtility.FromJson<CORE.PlanetDatabase>(data);
            CORE.PLANET_DATABASE = planetDatabase.serializableList;
        }));

        earthTransformPoint = earthObject.transform.parent;
        earthSpawnPoint = earthObject.transform.parent.parent;
        pinGroup = earthObject.transform.Find("Group_Pins").gameObject;
        earthRenderer = earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<Renderer>();
        SetMaterial(0);
    }

    IEnumerator Start()
    {
        earthCenter = earthObject.gameObject.transform.position;

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location service is not enabled: Using default location - UK.");

            tempPinCoord.Add(CORE.ECEFCoordinateFromLatLong(CORE.USER_LATLONG, CORE.EARTH_PREFAB_RADIUS));
            LoadPredefinedPinLocation();
            GeneratePins();
            ComputeRotation();

            yield break;
        }

        // Initialise location service
        Input.compass.enabled = true;
        Input.location.Start();

        int maxWait = 20;
        // Use default value if failed
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            Debug.Log("Timeout: Using default location - UK.");
            yield return false;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to get location information: Using default location - UK.");
            yield return false;
        }
        else
        {
            // Use values from the GPS
            CORE.USER_LATLONG = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);

            // Add current location to the list for futher computation
            tempPinCoord.Add(CORE.ECEFCoordinateFromLatLong(CORE.USER_LATLONG, CORE.EARTH_PREFAB_RADIUS));

            // Add featured location values to the list 
            LoadPredefinedPinLocation();

            // Generate pins based on given values
            GeneratePins();

            // Compute the rotation required for rotating the current location to the top
            ComputeRotation();
        }
        Input.location.Stop();
    }

    // Load predefined locations from the CORE
    private void LoadPredefinedPinLocation()
    {
        foreach (CORE.LocationInfo L in CORE.LOCATION_DATABASE)
        {
            tempPinCoord.Add(CORE.ECEFCoordinateFromLatLong(L.coord, CORE.EARTH_PREFAB_RADIUS));
        }
    }

    // Create pin objects using provided coordinates
    private void GeneratePins()
    {
        for (int i = 0; i < tempPinCoord.Count; i++)
        {
            // Instantiate the pin and place it under pin group
            GameObject pin = Instantiate(pinPrefab, new Vector3(0, 0, 0), Quaternion.identity, pinGroup.transform);

            // Shift the pin to the center of the earth
            pin.transform.localPosition += earthCenter;

            // Map the pin to the correct 3D coordinate
            pin.transform.localPosition += tempPinCoord[i];

            pin.name = (i - 1).ToString();

            // Set color for the current location pin
            if (i == 0)
            {
                pin.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
            }

            // Keep track of pins
            pinList.Add(pin);
        }

        // Clear cahce
        tempPinCoord.Clear();
    }

    private void ComputeRotation()
    {
        Vector3 currentPinPosition = pinList[0].gameObject.transform.localPosition - earthObject.gameObject.transform.localPosition;
        Vector3 targetPinPosition = Vector3.up * CORE.EARTH_PREFAB_RADIUS - earthObject.gameObject.transform.localPosition;
        CORE.ROTATE_TO_TOP = Quaternion.FromToRotation(currentPinPosition, targetPinPosition);
    }

    void Update()
    {
        // Gesture control
        if (spawned)
        {
            // Rotate the Earth manually 
            if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                // Only when the earth is visible
                if (earthObject.GetComponent<Renderer>().isVisible)
                {
                    // Get the displacement delta
                    Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

                    // Use the parent's local axis direction as reference without messing up its own transform
                    earthObject.transform.Rotate(earthObject.transform.parent.up, -deltaPosition.x * rotationSpeed, Space.World);
                    earthObject.transform.Rotate(earthObject.transform.parent.right, deltaPosition.y * rotationSpeed, Space.World);
                }
            }
            else if (Input.touchCount == 2)
            {
                // Only when the earth is visible
                if (earthObject.GetComponent<Renderer>().isVisible)
                {
                    // Get the touch
                    Touch firstTouch = Input.GetTouch(0);
                    Touch secondTouch = Input.GetTouch(1);

                    Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
                    Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

                    float previousTouchDeltaMagnitude = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
                    float currentTouchDeltaMagnitude = (firstTouch.position - secondTouch.position).magnitude;

                    float touchMagnitudeDifference = currentTouchDeltaMagnitude - previousTouchDeltaMagnitude;

                    earthObject.transform.parent.localScale += new Vector3(scaleSpeed * touchMagnitudeDifference, scaleSpeed * touchMagnitudeDifference, scaleSpeed * touchMagnitudeDifference);

                    // Limit the scale to 0.1f so that it will not disappear
                    if (earthObject.transform.parent.localScale.x < 0.1f)
                    {
                        earthObject.transform.parent.localScale = Vector3.one * 0.1f;
                    }
                }
            }
            else
            {
                // Update the tranform parent
                earthObject.transform.parent = null;
                earthTransformPoint.transform.LookAt(Camera.main.transform);
                earthTransformPoint.transform.Rotate(new Vector3(0, 180, 0));
                earthObject.transform.SetParent(earthTransformPoint);
            }
        }
    }

    private void OnDisable()
    {
        earthObject.SetActive(false);
        previewerOptions.SetActive(false);
    }

    private void OnEnable()
    {
        if (spawned)
        {
            earthObject.SetActive(true);
        }
        previewerOptions.SetActive(true);
    }

    public void TransitionMaterial(int index)
    {
        // Only begin transition when it is not in the process of transition
        if (!inTransition)
        {
            inTransition = true;
            if (currentMaterialIndex != index)
            {
                if (Mathf.RoundToInt(GetMaterialValue("_AlphaBlending")) == 0)
                {
                    // Replace the material and start transition
                    SetMaterial(index);
                    StartCoroutine(AlphaBlendToOne());
                }
                else
                {
                    // Transition from 1 to 0 for the first material and then 0 to 1 for the second material
                    StartCoroutine(AlphaBlendOneToOne(index));
                }
            }
            else
            {
                // Transition between 0 and 1 for the same material
                if (Mathf.RoundToInt(GetMaterialValue("_AlphaBlending")) == 0 )
                {
                    StartCoroutine(AlphaBlendToOne());
                }
                else
                {
                    StartCoroutine(AlphaBlendToZero());
                }
            } 
        }
    }

    public void SwitchLayer(int i)
    {
        Transform layers = earthObject.transform.Find("Group_Layers");

        foreach (Transform l in layers)
        {
            l.gameObject.SetActive(false);
        }
        layers.GetChild(i).gameObject.SetActive(true);

        if (layers.GetChild(i).localScale != layers.Find("Earth_Grid").localScale)
        {
            layers.Find("Earth_Grid").gameObject.SetActive(true);
        }

        if (showMagneticField)
        {
            layers.Find("Earth_MagneticField").gameObject.SetActive(true);
        }

        if (showCountryBorder)
        {
            layers.Find("Earth_Border").gameObject.SetActive(true);
        }
    }

    public void ToggleMagneticField()
    {
        if (showMagneticField)
        {
            showMagneticField = false;
        }
        else {
            showMagneticField = true;
        }
        earthObject.transform.Find("Group_Layers").Find("Earth_MagneticField").gameObject.SetActive(showMagneticField);
    }

    public void ToggleCountryBorder()
    {
        if (showCountryBorder)
        {
            showCountryBorder = false;
        }
        else
        {
            showCountryBorder = true;
        }
        earthObject.transform.Find("Group_Layers").Find("Earth_Border").gameObject.SetActive(showCountryBorder);
    }

    // Spawn a new earth
    public void SpawnEarth() {

        // Attach to the camera-related spawn point
        earthObject.transform.parent.SetParent(earthSpawnPoint);

        // Reset transform
        earthObject.transform.parent.transform.localPosition = Vector3.zero;
        earthObject.transform.parent.transform.localScale = Vector3.one;

        // Detach from the camera-related spawn point
        earthObject.transform.parent.parent = null;

        pinGroup.SetActive(false);
        earthObject.SetActive(true);

        StartCoroutine(AlphaClipToZero());
    }

    // Reset earth
    public void ResetEarth()
    {
        SwitchLayer(0);
        earthObject.transform.Find("Group_Layers").Find("Earth_MagneticField").gameObject.SetActive(false);
        earthObject.transform.Find("Group_Layers").Find("Earth_Border").gameObject.SetActive(false);
        showMagneticField = false;
        showCountryBorder = false;
        SetMaterial(0);
        earthObject.SetActive(false);
        spawned = false;
    }

    // Material manipulation
    private IEnumerator AlphaBlendToZero()
    {
        SetMaterialValue("_AlphaBlending", 1);
        while (GetMaterialValue("_AlphaBlending") > 0.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") - Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaBlendToOne()
    {
        SetMaterialValue("_AlphaBlending", 0);
        while (GetMaterialValue("_AlphaBlending") < 1.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") + Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaBlendOneToOne(int index)
    {
        // Fade to zero
        SetMaterialValue("_AlphaBlending", 1);
        while (GetMaterialValue("_AlphaBlending") > 0.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") - Time.deltaTime * 0.5f);
            yield return null;
        }

        // Switch to the target material
        SetMaterial(index);

        // Fade to one again
        SetMaterialValue("_AlphaBlending", 0);
        while (GetMaterialValue("_AlphaBlending") < 1.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") + Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaClipToZero()
    {
        SetMaterialValue("_Dissolve", 1);
        while (GetMaterialValue("_Dissolve") > 0)
        {
            SetMaterialValue("_Dissolve", GetMaterialValue("_Dissolve") - Time.deltaTime * 0.5f);
            yield return null;
        }
        pinGroup.SetActive(true);
        SetMaterial(1);
        spawned = true;
    }

    public Material GetMaterial()
    {
        return earthRenderer.material;
    }

    public void SetMaterial(int index)
    {
        earthRenderer.material = earthMaterialList[index];
        currentMaterialIndex = index;
    }

    public void SetMaterialValue(string property, float value)
    {
        earthRenderer.material.SetFloat(property, value);
    }

    public float GetMaterialValue(string property)
    {
        return earthRenderer.material.GetFloat(property);
    }

}
