using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetLocation : MonoBehaviour
{

    public Text latitudeDebugText;
    public Text longitudeDebugText;

    public static float northRotation;

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
            pinPosition.Add(GLOBAL.ECEFCoordinateFromLonLat(GLOBAL.USER_LATLONG, GLOBAL.EARTH_PREFAB_RADIUS));
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
            northRotation = Input.compass.trueHeading;

            latitudeDebugText.text = Input.location.lastData.latitude.ToString();
            longitudeDebugText.text = Input.location.lastData.longitude.ToString();

            if (pinPosition.Count != 0)
            {
                pinPosition.Clear();
            }

            pinPosition.Add(GLOBAL.ECEFCoordinateFromLonLat(GLOBAL.USER_LATLONG, GLOBAL.EARTH_PREFAB_RADIUS));

            GeneratePredefinedPinLocation();
            GeneratePins();
            ComputeRotation();
        }

        Input.location.Stop();
    }


    private void GeneratePredefinedPinLocation() {

        // Source: https://www.latlong.net/
        //GLOBAL.LATLONG_LIST.Add(new Vector2(51.509865f, -0.118092f)); // London, UK
        GLOBAL.LATLONG_LIST.Add(new Vector2(48.864716f, 2.349014f)); // Paris, FR
        GLOBAL.LATLONG_LIST.Add(new Vector2(40.730610f, -73.935242f)); // New York, US
        GLOBAL.LATLONG_LIST.Add(new Vector2(-37.840935f, 144.946457f)); // Melbourne, AU
        GLOBAL.LATLONG_LIST.Add(new Vector2(35.652832f, 139.839478f)); // Tokyo, JP
        GLOBAL.LATLONG_LIST.Add(new Vector2(-36.848461f, 174.763336f)); // Auckland, NZ
        GLOBAL.LATLONG_LIST.Add(new Vector2(31.224361f, 121.469170f)); // Shanghai, CN
        GLOBAL.LATLONG_LIST.Add(new Vector2(49.246292f, -123.116226f)); // Vancouver, CA
        GLOBAL.LATLONG_LIST.Add(new Vector2(55.751244f, 37.618423f)); // Moscow, RU

        foreach (Vector2 v in GLOBAL.LATLONG_LIST) {
            pinPosition.Add(GLOBAL.ECEFCoordinateFromLonLat(v, GLOBAL.EARTH_PREFAB_RADIUS));
        }
    }


    private void GeneratePins() { 
        for (int i = 0; i < pinPosition.Count; i++) {
            // Instantiate the pin
            GameObject pin = Instantiate(pinPrefab, new Vector3(0, 0, 0), Quaternion.identity, earthObject.gameObject.transform);

            // Shift the pin to the center of the earth
            pin.transform.localPosition += earthCenter;

            // Map the pin to the correct 3D coordinate
            pin.transform.localPosition += pinPosition[i];

            // Set color for important pins
            if (i == 0)
            {
                pin.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                pin.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            }

            // Keep track of pins
            GLOBAL.PIN_LIST.Add(pin);
            //pinList.Add(pin);

            Debug.Log("COORDINATE ADJUSTED:" + pin.transform.localPosition);
        }
    }

    private void ComputeRotation() {

        Debug.Log("LocalPos:" + GLOBAL.PIN_LIST[0].gameObject.transform.localPosition);

        Vector3 currentPinPosition = GLOBAL.PIN_LIST[0].gameObject.transform.localPosition - earthObject.gameObject.transform.localPosition;
        Vector3 targetPinPosition = Vector3.up * GLOBAL.EARTH_PREFAB_RADIUS - earthObject.gameObject.transform.localPosition;
        GLOBAL.ROTATE_TO_TOP = Quaternion.FromToRotation(currentPinPosition, targetPinPosition);

        Debug.Log("current:" + currentPinPosition);
        Debug.Log("target" + targetPinPosition);
        Debug.Log("Qauternion:" + GLOBAL.ROTATE_TO_TOP);

        //earthObject.gameObject.transform.localRotation = GLOBAL.ROTATE_TO_TOP;
    }

}
