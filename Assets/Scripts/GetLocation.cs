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

    private readonly float earthRadius = 1.0f / 2.0f;
    private readonly float flattening = 1.0f / 298.257224f;
    private Vector3 coordOffset;


    // Use this for initialization
    IEnumerator Start()
    {
        coordOffset = this.gameObject.transform.position;

        if (!Input.location.isEnabledByUser)
        {
            userLatLong = new Vector2(51.5f, -0.118f);
            ECEFCoordinateFromLonLat(userLatLong);
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

            ECEFCoordinateFromLonLat(userLatLong);
        }

        Input.location.Stop();
    }


    private void ECEFCoordinateFromLonLat(Vector2 latlon)
    {

        latlon.x = latlon.x + ((latlon.x - 30f) / 2f);
        latlon = latlon * (Mathf.PI) / 180.0f;
       
        //apply latlong -> ecef conversion formula
        var c = 1 / Mathf.Sqrt(Mathf.Cos(latlon.x) * Mathf.Cos(latlon.x) + (1 - flattening) * (1 - flattening) * Mathf.Sin(latlon.x) * Mathf.Sin(latlon.x));
        var s = (1 - flattening) * (1 - flattening) * c;
        var X = (earthRadius * c + 0) * Mathf.Cos(latlon.x) * Mathf.Cos(latlon.y);
        var Y = (earthRadius * c + 0) * Mathf.Cos(latlon.x) * Mathf.Sin(latlon.y);
        var Z = (earthRadius * s + 0) * Mathf.Sin(latlon.x);

        Vector3 coordConverted = new Vector3(-Y, Z, X);

        Debug.Log("COORDINATE:" + coordConverted);

        Instantiate(pinPrefab, coordConverted + coordOffset, Quaternion.identity, this.gameObject.transform);
    }

}
