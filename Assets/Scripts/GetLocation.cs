using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetLocation : MonoBehaviour
{
    public GameObject earthObject;
    public GameObject pinPrefab;

    private Vector3 earthCenter;
    private List<Vector3> pinPosition = new List<Vector3>();

    private void Awake()
    {
        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, "Database/", "location.json"),(data)=>{
            // Load data into the serializable class and transfer it to the non-serialisable list
            CORE.LocationDatabase locationDatabase = JsonUtility.FromJson<CORE.LocationDatabase>(data);
            CORE.LOCATION_DATABASE = locationDatabase.serializableList;
        }));

        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, "Database/", "planet.json"), (data) => {
            // Load data into the serializable class and transfer it to the non-serialisable list
            CORE.PlanetDatabase planetDatabase = JsonUtility.FromJson<CORE.PlanetDatabase>(data);
            CORE.PLANET_DATABASE = planetDatabase.serializableList;
        }));
    }

    // Use this for initialization
    IEnumerator Start()
    {
        earthCenter = earthObject.gameObject.transform.position;

        if (!Input.location.isEnabledByUser)
        {

            Debug.Log("Location service is not enabled: Using default location - UK.");

            if (pinPosition.Count != 0) {
                pinPosition.Clear();
            }

            pinPosition.Add(CORE.ECEFCoordinateFromLatLong(CORE.USER_LATLONG, CORE.EARTH_PREFAB_RADIUS));
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

            if (pinPosition.Count != 0)
            {
                pinPosition.Clear();
            }

            // Add current location to the list for futher computation
            pinPosition.Add(CORE.ECEFCoordinateFromLatLong(CORE.USER_LATLONG, CORE.EARTH_PREFAB_RADIUS));

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
            pinPosition.Add(CORE.ECEFCoordinateFromLatLong(L.coord, CORE.EARTH_PREFAB_RADIUS));
        }   
    }

    // Create pin objects using provided coordinates
    private void GeneratePins()
    {
        Transform pinGroup = earthObject.transform.GetChild(0);

        for (int i = 0; i < pinPosition.Count; i++)
        {
            // Instantiate the pin and place it under pin group
            GameObject pin = Instantiate(pinPrefab, new Vector3(0, 0, 0), Quaternion.identity, pinGroup);

            // Shift the pin to the center of the earth
            pin.transform.localPosition += earthCenter;

            // Map the pin to the correct 3D coordinate
            pin.transform.localPosition += pinPosition[i];

            pin.name = (i-1).ToString();

            // Set color for the current location pin
            if (i == 0)
            {
                pin.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                pin.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            }

            // Keep track of pins
            CORE.PIN_LIST.Add(pin);
        }
    }

    private void ComputeRotation()
    {
        Vector3 currentPinPosition = CORE.PIN_LIST[0].gameObject.transform.localPosition - earthObject.gameObject.transform.localPosition;
        Vector3 targetPinPosition = Vector3.up * CORE.EARTH_PREFAB_RADIUS - earthObject.gameObject.transform.localPosition;
        CORE.ROTATE_TO_TOP = Quaternion.FromToRotation(currentPinPosition, targetPinPosition);
    }

}
