using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GLOBAL;

public class Pin : MonoBehaviour
{
    private PinData pinData;

    public void InitialiseData(string[] dataIn) {
        pinData.cityName = dataIn[0];
        pinData.cityCoord = dataIn[1];
        pinData.cityDesc = dataIn[2];

        //Debug.Log("PinData" + pinData.cityName);
        //Debug.Log("PinData" + pinData.cityCoord);
        //Debug.Log("PinData" + pinData.cityDesc);
    }

    public PinData GetPinData() {
        return pinData;
    }


}
