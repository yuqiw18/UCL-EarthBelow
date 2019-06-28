using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetLocation : MonoBehaviour
{
    public GameObject earthObject;
    public GameObject pinPrefab;

    private Vector3 earthCenter;
    private List<Vector3> pinPosition = new List<Vector3>();

    // Use this for initialization
    IEnumerator Start()
    {

        earthCenter = earthObject.gameObject.transform.position;

        if (!Input.location.isEnabledByUser)
        {
            if (pinPosition.Count != 0) {
                pinPosition.Clear();
            }
            pinPosition.Add(UTIL.ECEFCoordinateFromLonLat(GLOBAL.USER_LATLONG, GLOBAL.EARTH_PREFAB_RADIUS));
            GLOBAL.USER_POSITION_REAL_SCALE = UTIL.ECEFCoordinateFromLonLat(GLOBAL.USER_LATLONG, GLOBAL.EARTH_CRUST_RADIUS);
            GeneratePredefinedPinLocation();
            GeneratePins();
            ComputeRotation();
            Debug.Log("Location service is not enabled.");
            yield break;

        }

        // Initialise location service
        Input.compass.enabled = true;
        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            Debug.Log("Timeout");
            yield return false;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to get location information");
            yield return false;
        }
        else
        {

            GLOBAL.USER_LATLONG = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            GLOBAL.USER_POSITION_REAL_SCALE = UTIL.ECEFCoordinateFromLonLat(GLOBAL.USER_LATLONG, GLOBAL.EARTH_CRUST_RADIUS);

            if (pinPosition.Count != 0)
            {
                pinPosition.Clear();
            }

            pinPosition.Add(UTIL.ECEFCoordinateFromLonLat(GLOBAL.USER_LATLONG, GLOBAL.EARTH_PREFAB_RADIUS));

            GeneratePredefinedPinLocation();
            GeneratePins();
            ComputeRotation();
        }

        Input.location.Stop();
    }


    private void GeneratePredefinedPinLocation() {
        foreach (Vector2 v in GLOBAL.LATLONG_LIST) {
            pinPosition.Add(UTIL.ECEFCoordinateFromLonLat(v, GLOBAL.EARTH_PREFAB_RADIUS));
            GLOBAL.POSITION_REAL_SCALE_LIST.Add(UTIL.ECEFCoordinateFromLonLat(v, GLOBAL.EARTH_CRUST_RADIUS));
        }
    }


    private void GeneratePins() {

        Transform pinGroup = earthObject.transform.GetChild(0);

        for (int i = 0; i < pinPosition.Count; i++) {

            // Instantiate the pin and place it under pin group
            GameObject pin = Instantiate(pinPrefab, new Vector3(0, 0, 0), Quaternion.identity, pinGroup);

            // Shift the pin to the center of the earth
            pin.transform.localPosition += earthCenter;

            // Map the pin to the correct 3D coordinate
            pin.transform.localPosition += pinPosition[i];

            // Set color for important pins
            if (i == 0)
            {
                pin.name = "London, UK";
                pin.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                pin.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            }
            else {
                // Add name
                string cityName = GLOBAL.CITY_LIST[i-1];
                pin.gameObject.name = cityName;
                switch (cityName) {
                    case "Melbourne, AU":
                        pin.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                        pin.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
                        break;

                    case "Paris, FR":
                        pin.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                        pin.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue);
                        break;
                    default:
                        break;

                }
            }

            //pin.AddComponent<DrawDebugLine>();
            //pin.GetComponent<DrawDebugLine>().targetTransform = earthObject.transform;
            //pin.GetComponent<LineRenderer>().enabled = true;

            // Keep track of pins
            GLOBAL.PIN_LIST.Add(pin);

            //Debug.Log("COORDINATE ADJUSTED:" + pin.transform.localPosition);
        }
    }

    private void ComputeRotation() {

        //
        Vector3 currentPinPosition = GLOBAL.PIN_LIST[0].gameObject.transform.localPosition - earthObject.gameObject.transform.localPosition;
        Vector3 targetPinPosition = Vector3.up * GLOBAL.EARTH_PREFAB_RADIUS - earthObject.gameObject.transform.localPosition;
        GLOBAL.ROTATE_TO_TOP = Quaternion.FromToRotation(currentPinPosition, targetPinPosition);
    }

}
