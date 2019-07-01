using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Global Variables
public class GLOBAL: MonoBehaviour
{
    #region PSEUDO_DATABASE
    public struct LocationInfo {
        public string name;
        public string country;
        public Vector2 coord;
        public string description;
    }

    public LocationInfo InitialiseLocationInfo(string name, string country, Vector2 coord, string desc) {
        LocationInfo locationInfo;
        locationInfo.name = name;
        locationInfo.country = country;
        locationInfo.coord = coord;
        locationInfo.description = desc;
        return locationInfo;
    }

    public static List<LocationInfo> LOCATION_DATABASE = new List<LocationInfo>();
    #endregion


    #region DATA_LIST
    public static Vector2 USER_LATLONG = new Vector2(51.509865f, -0.118092f); // Default UK latitude and longitude
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
    public static readonly float EARTH_PREFAB_SCALE_TO_REAL = 2.0f * EARTH_CRUST_RADIUS;
    #endregion

    // Only called once during the application lifetime
    private void Awake()
    {
        Debug.Log("Global Variable Loaded");
        InitialiseLatlongList();
    }

    private void InitialiseLatlongList()
    {
        // Source: https://www.latlong.net/
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Paris", "France", new Vector2(48.864716f, 2.349014f), "Paris, France's capital, is a major European city and a global center for art, fashion, gastronomy and culture. Its 19th-century cityscape is crisscrossed by wide boulevards and the River Seine."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("New York", "United States", new Vector2(40.730610f, -73.935242f), "New York City comprises 5 boroughs sitting where the Hudson River meets the Atlantic Ocean. At its core is Manhattan, a densely populated borough that’s among the world’s major commercial, financial and cultural centers."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Melbourne", "Australia", new Vector2(-37.840935f, 144.946457f), "Melbourne is the coastal capital of the southeastern Australian state of Victoria. At the city's centre is the modern Federation Square development, with plazas, bars, and restaurants by the Yarra River."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Tokyo", "Japan", new Vector2(35.652832f, 139.839478f), "Tokyo, Japan’s busy capital, mixes the ultramodern and the traditional, from neon-lit skyscrapers to historic temples. The opulent Meiji Shinto Shrine is known for its towering gate and surrounding woods. The Imperial Palace sits amid large public gardens."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Auckland", "Zealand", new Vector2(-36.848461f, 174.763336f), "Auckland, based around 2 large harbours, is a major city in the north of New Zealand’s North Island. In the centre, the iconic Sky Tower has views of Viaduct Harbour, which is full of superyachts and lined with bars and cafes."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Shanghai", "China", new Vector2(31.224361f, 121.469170f), "Shanghai, on China’s central coast, is the country's biggest city and a global financial hub. Its heart is the Bund, a famed waterfront promenade lined with colonial-era buildings."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Vancouver", "Canada", new Vector2(49.246292f, -123.116226f), "Vancouver, a bustling west coast seaport in British Columbia, is among Canada’s densest, most ethnically diverse cities. A popular filming location, it’s surrounded by mountains, and also has thriving art, theatre and music scenes."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Moscow", "Russia", new Vector2(55.751244f, 37.618423f), "Moscow, on the Moskva River in western Russia, is the nation’s cosmopolitan capital. In its historic core is the Kremlin, a complex that’s home to the president and tsarist treasures in the Armoury. Outside its walls is Red Square, Russia's symbolic center."));

    }
}
 