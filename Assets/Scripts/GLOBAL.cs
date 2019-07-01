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
        public Vector2 coord;
        public string description;
    }

    public LocationInfo InitialiseLocationInfo(string name, Vector2 coord, string desc) {
        LocationInfo locationInfo;
        locationInfo.name = name;
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
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Paris, France", new Vector2(48.864716f, 2.349014f), "NULL"));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("New York, United States", new Vector2(40.730610f, -73.935242f), "NULL"));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Melbourne, Australia", new Vector2(-37.840935f, 144.946457f), "NULL"));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Tokyo, Japan", new Vector2(35.652832f, 139.839478f), "NULL"));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Auckland, Zealand", new Vector2(-36.848461f, 174.763336f), "NULL"));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Shanghai, China", new Vector2(31.224361f, 121.469170f), "NULL"));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Vancouver, Canada", new Vector2(49.246292f, -123.116226f), "NULL"));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Moscow, Russia", new Vector2(55.751244f, 37.618423f), "NULL"));

    }
}
 