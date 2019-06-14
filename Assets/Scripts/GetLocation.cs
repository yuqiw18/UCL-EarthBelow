using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetLocation : MonoBehaviour
{

    public Text latitudeDebugText;
    public Text longitudeDebugText;

    public static Vector2 userLatLong;
    public static float northRotation;

    public GameObject pinPrefab;

    public static List<GameObject> pinList = new List<GameObject>();
    private List<Vector2> latlongList = new List<Vector2>();

    private readonly float earthRadius = 1.0f / 2.0f;
    private readonly float flattening = 1.0f / 298.257224f;
    private Vector3 earthCenter;

    // Use this for initialization
    IEnumerator Start()
    {


        earthCenter = this.gameObject.transform.position;

        if (!Input.location.isEnabledByUser)
        {
            userLatLong = new Vector2(51.5f, -0.118f);
            //ECEFCoordinateFromLonLat(userLatLong);
            GeneratePredefinedPinLocation();
            TestRotation();
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
            userLatLong = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            northRotation = Input.compass.trueHeading;

            latitudeDebugText.text = Input.location.lastData.latitude.ToString();
            longitudeDebugText.text = Input.location.lastData.longitude.ToString();
            //ECEFCoordinateFromLonLat(userLatLong);
            GeneratePredefinedPinLocation();
            TestRotation();
        }

        Input.location.Stop();
    }


    private void GeneratePredefinedPinLocation() {

        // Source: https://www.latlong.net/
        latlongList.Add(new Vector2(51.509865f, -0.118092f)); // London, UK
        latlongList.Add(new Vector2(48.864716f, 2.349014f)); // Paris, FR
        latlongList.Add(new Vector2(40.730610f, -73.935242f)); // New York, US
        latlongList.Add(new Vector2(-37.840935f, 144.946457f)); // Melbourne, AU
        latlongList.Add(new Vector2(35.652832f, 139.839478f)); // Tokyo, JP
        latlongList.Add(new Vector2(-36.848461f, 174.763336f)); // Auckland, NZ
        latlongList.Add(new Vector2(31.224361f, 121.469170f)); // Shanghai, CN
        latlongList.Add(new Vector2(49.246292f, -123.116226f)); // Vancouver, CA
        latlongList.Add(new Vector2(55.751244f, 37.618423f)); // Moscow, RU

        foreach (Vector2 v in latlongList) {
            ECEFCoordinateFromLonLat(v);
        }

    }

    private void ECEFCoordinateFromLonLat(Vector2 latlong)
    {

        // Determine the sign for latitude conversion
        float sign = 1.0f;
        if (latlong.x >= 0) {
            sign = -1.0f;
        }
        else {
            sign = 1.0f;
        }

        // Preprocess the latitude
        latlong.x = latlong.x + ((latlong.x + 30f*sign) / 2f);

        // Convert the latitude-longitude to radian
        latlong = latlong * (Mathf.PI) / 180.0f;
       
        // Convert latitude-longitude to ECEF coordinate
        float c = 1 / Mathf.Sqrt(Mathf.Cos(latlong.x) * Mathf.Cos(latlong.x) + (1 - flattening) * (1 - flattening) * Mathf.Sin(latlong.x) * Mathf.Sin(latlong.x));
        float s = (1 - flattening) * (1 - flattening) * c;
        float X = (earthRadius * c + 0) * Mathf.Cos(latlong.x) * Mathf.Cos(latlong.y);
        float Y = (earthRadius * c + 0) * Mathf.Cos(latlong.x) * Mathf.Sin(latlong.y);
        float Z = (earthRadius * s + 0) * Mathf.Sin(latlong.x);
        Vector3 coordConverted = new Vector3(-Y, Z, X);

        Debug.Log("COORDINATE CONVERTED:" + coordConverted);

        // Instantiate the pin
        GameObject temporaryPin = Instantiate(pinPrefab, new Vector3(0,0,0), Quaternion.identity, this.gameObject.transform);

        // Shift the pin to the center of the earth
        temporaryPin.transform.localPosition += earthCenter;

        // Map the pin to the correct 3D coordinate
        temporaryPin.transform.localPosition += coordConverted;

        // Keep track of pins
        pinList.Add(temporaryPin);

        Debug.Log("COORDINATE ADJUSTED:" + temporaryPin.transform.localPosition);
    }

    private void TestRotation() {

        Debug.Log("LocalPos:" + pinList[0].gameObject.transform.localPosition);

        Vector3 currentPinPosition = pinList[0].gameObject.transform.localPosition - this.gameObject.transform.localPosition;
        Vector3 targetPinPosition = Vector3.up * earthRadius - this.gameObject.transform.localPosition;
        Quaternion rotateToTop = Quaternion.FromToRotation(currentPinPosition, targetPinPosition);

        Debug.Log("current:" + currentPinPosition);
        Debug.Log("target" + targetPinPosition);
        Debug.Log("Qauternion:" + rotateToTop);

        //this.gameObject.transform.rotation = rotateToTop;

    }

}
