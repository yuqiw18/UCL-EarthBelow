using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// Global Variables
public class GLOBAL: MonoBehaviour
{
    #region JSON_DATABASE_SERIALISATION

    #region GEOGRAPHICAL_DATA
    [Serializable]
    public struct LocationInfo
    {
        public string name;
        public string country;
        public Vector2 coord;
        public string description;
    }

    [Serializable]
    public class LocationDatabase
    {
        public List<LocationInfo> serializableList;
    }

    public static List<LocationInfo> LOCATION_DATABASE = new List<LocationInfo>();
    #endregion

    #region GEOLOGICAL_DATA
    [Serializable]
    public struct PlanetInfo
    {
        public string planet;
        public string structure;
        public string state;
        public string thickness;
        public string temperature;
        public string composition;
        public string highlight;
    }

    [Serializable]
    public class PlanetDatabase
    {
        public List<PlanetInfo> serializableList;
    }

    public static List<PlanetInfo> PLANET_DATABASE = new List<PlanetInfo>();
    #endregion

    #endregion

    #region DATA_LIST
    public static Vector2 USER_LATLONG = new Vector2(51.507351f, -0.127758f); // Default UK latitude and longitude
    public static List<GameObject> PIN_LIST = new List<GameObject>();
    public static Quaternion ROTATE_TO_TOP;
    #endregion

    #region EARTH_GEOLOGY_PARAMETERS
    public static readonly float EARTH_INNER_CORE_RADIUS = 1216000f;
    public static readonly float EARTH_OUTER_CORE_RADIUS = EARTH_INNER_CORE_RADIUS + 2270000f;
    public static readonly float EARTH_LOWER_MANTLE_RADIUS = EARTH_OUTER_CORE_RADIUS + 2885000f;
    public static readonly float EARTH_CRUST_RADIUS = 6371000f;
    #endregion

    #region EARTH_MATH_PARAMETERS
    public static readonly float EARTH_PREFAB_RADIUS = 0.5f;
    public static readonly float EARTH_FLATTENING = 1.0f / 298.257224f;
    public static readonly float EARTH_PREFAB_SCALE_TO_REAL = (1.0f / EARTH_PREFAB_RADIUS) * EARTH_CRUST_RADIUS;
    #endregion

    // Only called once during the application lifetime
    private void Awake()
    {
        StartCoroutine(InitialiseLocationDataFromJSON());
        StartCoroutine(InitialisePlanetDataFromJSON());
    }

    // JSON requires a very strict key-value pair formatting together with special characters escaping
    // This process can be simplified using Visual Studio Code with extensions such as JSON Escaper
    // location.json data source: https://www.latlong.net/
    // layer.json data source: Wikipedia
    private IEnumerator InitialiseLocationDataFromJSON() {
        
        string filePath = Path.Combine(Application.streamingAssetsPath, "Database/" ,"location.json");
        string jsonContent;

        // Read pure text using UnityWebRequest or File.ReadAllText
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            jsonContent = www.downloadHandler.text;
        }
        else
        {
            jsonContent = File.ReadAllText(filePath);
        }

        // Load data into the serializable class and transfer it to the non-serialisable list
        LocationDatabase locationDatabase = JsonUtility.FromJson<LocationDatabase>(jsonContent);
        LOCATION_DATABASE = locationDatabase.serializableList;
    }

    private IEnumerator InitialisePlanetDataFromJSON() {

        string filePath = Path.Combine(Application.streamingAssetsPath, "Database/", "planet.json");
        string jsonContent;

        // Read pure text using UnityWebRequest or File.ReadAllText
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            jsonContent = www.downloadHandler.text;
        }
        else
        {
            jsonContent = File.ReadAllText(filePath);
        }

        // Load data into the serializable class and transfer it to the non-serialisable list
        PlanetDatabase planetDatabase = JsonUtility.FromJson<PlanetDatabase>(jsonContent);
        PLANET_DATABASE = planetDatabase.serializableList;
    }

    private string SaveToJSON() {
        PlanetDatabase locationDatabase = new PlanetDatabase();
        locationDatabase.serializableList = PLANET_DATABASE;
        return JsonUtility.ToJson(locationDatabase);
    }

}
 