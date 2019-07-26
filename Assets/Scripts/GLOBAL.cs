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
    // Geographical info
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

    // Geological info
    [Serializable]
    public struct LayerInfo
    {
        public string term;
        public string extra;
        public string detail;
    }

    public LayerInfo InitialiseStructureInfo(string term, string extra, string detail)
    {
        LayerInfo layerInfo;
        layerInfo.term = term;
        layerInfo.extra = extra;
        layerInfo.detail = detail;
        return layerInfo;
    }

    public static List<LayerInfo> LAYER_INFO = new List<LayerInfo>();
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
        InitialiseEarthStructureInfo();
        StartCoroutine(InitialiseLocationDataFromJSON());
    }

    private void InitialiseEarthStructureInfo()
    {
        LAYER_INFO.Add(InitialiseStructureInfo("Crust", "", "At the very top of the crust is where we live on but deeper down it is all dense rock and metal ores. The Crust is composed of mainly granite, basalt, and diorite rocks. Its thickness can vary from wherever you are. From a continent to the edge of the crust is about 60 km.  From the bottom of the ocean to the edge of the crust is about 10 km. The Crust's temperature is different throughout the entire crust, it starts at about 200°C and can rise up to 400°C. The crust is constantly moving due to the energy exchange in its lower layers. These movements will cause earthquakes and volcanoes to erupt; such a phenomenon is also known as the Theory of Plate Tectonics."));
        LAYER_INFO.Add(InitialiseStructureInfo("Mantle", "", "The Mantle is the second layer of the Earth. It is about 2900 km thick and is the biggest which takes up 84% of the Earth. The Mantle is divided into two sections. The Asthenosphere, the bottom layer of the mantle made of plastic like fluid and The Lithosphere the top part of the mantle made of a cold dense rock. The average temperature of the mantle is 3000°C and it is composed of silicates of iron and magnesium, sulphides and oxides of silicon and magnesium. Convection currents happen inside the mantle and are caused by the continuous circular motion of rocks in the lithosphere being pushed down by hot molasses liquid from the Asthenosphere.  The rocks then melt and float up as molasses liquid because it is less dense and the rocks float down because it is denser."));
        LAYER_INFO.Add(InitialiseStructureInfo("Outer Core", "", "The Outer Core is the second to last layer of the Earth.  It is a magma like liquid layer that surrounds the Inner Core and creates Earth's magnetic field. The Outer Core is about 2200 km thick and is the second largest layer and made entirely out of liquid magma. Its temperature is about 4000 - 5000°C. The Outer Core is composed of iron and some nickel while there is very few rocks and iron and nickel ore left because of the Inner Core melting all the metal into liquid magma. Since the outer core moves around the inner core, Earth's magnetism is created."));
        LAYER_INFO.Add(InitialiseStructureInfo("Inner Core", "", "The Inner Core is the final layer of the Earth which is a solid ball made of metal. It is about 1250 km thick and is the second smallest layer of the Earth. Although it is one of the smallest, the Inner Core is also the hottest layer. The Inner Core is composed of an element named NiFe (Ni for Nickel and Fe for Ferrum also known as Iron). The Inner Core is about 5000-6000°C and it melts all metal ores in the Outer Core causing it to turn into liquid magma."));
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

    private string TestToJSON() {
        LocationDatabase locationDatabase = new LocationDatabase();
        locationDatabase.serializableList = LOCATION_DATABASE;
        return JsonUtility.ToJson(locationDatabase);
    }
}
 