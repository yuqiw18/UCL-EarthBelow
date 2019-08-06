using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class INIT : MonoBehaviour
{

    public int frameRate = 60;

    private void Awake()
    {
        Application.targetFrameRate = frameRate;

        // Load data into the serializable class and transfer it to the non-serialisable list
        // This is an experimental function. Can be used for futher development of this application
        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(CORE.WEB_SERVER_ADDRESS, "location.json"), (webData) =>
        {
            if (!webData.Equals(""))
            {
                // Use online data
                CORE.LocationDatabase locationDatabase = JsonUtility.FromJson<CORE.LocationDatabase>(webData);
                CORE.LOCATION_DATABASE = locationDatabase.serializableList;

                Debug.Log("Online data");
                CORE.DATA_LOADED_FLAG = true;
            }else
            {
                StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, "Database/", "location.json"), (localData) =>
                {
                    // Use local data
                    CORE.LocationDatabase locationDatabase = JsonUtility.FromJson<CORE.LocationDatabase>(localData);
                    CORE.LOCATION_DATABASE = locationDatabase.serializableList;

                    Debug.Log("Local data");
                    CORE.DATA_LOADED_FLAG = true;
                }));
            }
            
        }));

        // Load data into the serializable class and transfer it to the non-serialisable list
        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, "Database/", "planet.json"), (data) =>
        {
            CORE.PlanetDatabase planetDatabase = JsonUtility.FromJson<CORE.PlanetDatabase>(data);
            CORE.PLANET_DATABASE = planetDatabase.serializableList;
        }));


       


    }

}
