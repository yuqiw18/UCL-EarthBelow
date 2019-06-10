using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLocation : MonoBehaviour
{

    public static Vector2 userLatLong;
    public static float northRotation;

    // Use this for initialization
    IEnumerator Start()
    {
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
        }

        Input.location.Stop();
    }
}
