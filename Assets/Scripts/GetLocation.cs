using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetLocation : MonoBehaviour
{

    public Text debugText;

    public static Vector2 userLatLong;
    public static float northRotation;

    // Use this for initialization
    IEnumerator Start()
    {

        Debug.Log("Taking GPS Info");

        if (!Input.location.isEnabledByUser)
        {
            userLatLong = new Vector2(51.5f, -0.118f); //Default location - London
            yield break;
        }

        //Initialise location services and compass
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
            print("Timed out");
            yield return false;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield return false;
        }
        else
        {
            userLatLong = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            northRotation = Input.compass.trueHeading;

            Debug.Log("GPS INFOOO");
            Debug.Log(Input.location.lastData.latitude);

            debugText.text = Input.location.lastData.latitude.ToString();
        }

        Input.location.Stop();
    }
}
