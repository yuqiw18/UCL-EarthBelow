using UnityEngine;
using System.IO;

public class INIT : MonoBehaviour
{
    public int frameRate = 60;

    private void Awake()
    {
        Application.targetFrameRate = frameRate;

        // Load data into the serializable class and transfer it to the non-serialisable list
        // This is an experimental function which can be used for futher development of this application
        // Since Application.streamingAssetsPath is read-only, it is not possible to modify the existing data
        // To overwrite or create new files locally, use Application.persistentDataPath instead
        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(CORE.DATABASE_WEB_SERVER, CORE.DATABASE_FOLDER, CORE.DATABASE_FILE_LOCATION), (webData) =>
        {
            if (!webData.Equals(""))
            {
                // Use online data
                CORE.LocationDatabase locationDatabase = JsonUtility.FromJson<CORE.LocationDatabase>(webData);
                CORE.LOCATION_DATABASE = locationDatabase.serializableList;

                Debug.Log("Online data");
                CORE.DATABASE_LOADED_FLAG = true;
            }else
            {
                StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, CORE.DATABASE_FOLDER, CORE.DATABASE_FILE_LOCATION), (localData) =>
                {
                    // Use local data
                    CORE.LocationDatabase locationDatabase = JsonUtility.FromJson<CORE.LocationDatabase>(localData);
                    CORE.LOCATION_DATABASE = locationDatabase.serializableList;

                    Debug.Log("Local data");
                    CORE.DATABASE_LOADED_FLAG = true;
                }));
            }
        }));

        // Load data into the serializable class and transfer it to the non-serialisable list
        StartCoroutine(CORE.LoadDataFromJSON(Path.Combine(Application.streamingAssetsPath, CORE.DATABASE_FOLDER, CORE.DATABASE_FILE_LAYER), (data) =>
        {
            CORE.PlanetDatabase planetDatabase = JsonUtility.FromJson<CORE.PlanetDatabase>(data);
            CORE.PLANET_DATABASE = planetDatabase.serializableList;
        }));
    }
}
