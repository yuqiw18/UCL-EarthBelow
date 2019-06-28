using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSDebug : MonoBehaviour
{

    public Text latitudeDebugText;
    public Text longitudeDebugText;
    public Text degreeToNorth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.compass.enabled)
        {
            latitudeDebugText.text = Input.location.lastData.latitude.ToString();
            longitudeDebugText.text = Input.location.lastData.longitude.ToString();
            degreeToNorth.text = Input.compass.trueHeading.ToString();
        }
    }
}
