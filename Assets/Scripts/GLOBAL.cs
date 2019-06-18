using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLOBAL: MonoBehaviour
{
    #region DATA_LIST
    public static Vector2 USER_LATLONG = new Vector2(51.509865f, -0.118092f); // Default UK latitude and longitude
    public static Vector3 USER_POSITION_REAL_SCALE = new Vector3(0, 0, 0);
    public static Quaternion ROTATE_TO_TOP;
    public static float ROTATE_TO_NORTH;

    public static Dictionary<string, Vector2> CITY_LATLONG_LIST = new Dictionary<string, Vector2>();

    public static List<GameObject> PIN_LIST = new List<GameObject>();
    public static List<Vector2> LATLONG_LIST = new List<Vector2>();

    public static List<Vector3> POSITION_REAL_SCALE_LIST = new List<Vector3>();
    #endregion

    #region EARTH_MATH_PARAMETERS
    public static readonly float EARTH_PREFAB_RADIUS = 0.5f;
    public static readonly float EARTH_FLATTENING = 1.0f / 298.257224f;
    public static readonly float EARTH_PREFAB_SCALE_TO_REAL = 2.0f * EARTH_CRUST_RADIUS;
    #endregion

    #region EARTH_GEOLOGY_PARAMETERS
    public static readonly float EARTH_INNER_CORE_RADIUS = 1216000f;
    public static readonly float EARTH_OUTER_CORE_RADIUS = EARTH_INNER_CORE_RADIUS + 2270000f;
    public static readonly float EARTH_LOWER_MANTLE_RADIUS = EARTH_OUTER_CORE_RADIUS + 2885000f;
    public static readonly float EARTH_CRUST_RADIUS = 6371000f;
    #endregion

    private void Awake()
    {
        Debug.Log("GLOBAL VAR LOADED");
    }

    private void InitialiseLatlongList()
    {
        // Source: https://www.latlong.net/
        LATLONG_LIST.Add(new Vector2(51.509865f, -0.118092f)); // London, UK
        LATLONG_LIST.Add(new Vector2(48.864716f, 2.349014f)); // Paris, FR
        LATLONG_LIST.Add(new Vector2(40.730610f, -73.935242f)); // New York, US
        LATLONG_LIST.Add(new Vector2(-37.840935f, 144.946457f)); // Melbourne, AU
        LATLONG_LIST.Add(new Vector2(35.652832f, 139.839478f)); // Tokyo, JP
        LATLONG_LIST.Add(new Vector2(-36.848461f, 174.763336f)); // Auckland, NZ
        LATLONG_LIST.Add(new Vector2(31.224361f, 121.469170f)); // Shanghai, CN
        LATLONG_LIST.Add(new Vector2(49.246292f, -123.116226f)); // Vancouver, CA
        LATLONG_LIST.Add(new Vector2(55.751244f, 37.618423f)); // Moscow, RU
    }

    public static Vector3 ECEFCoordinateFromLonLat(Vector2 latlong, float radius) {
        // Determine the sign for latitude conversion
        float sign = 1.0f;
        if (latlong.x >= 0){
            sign = -1.0f;
        }else{
            sign = 1.0f;
        }

        // Preprocess the latitude
        latlong.x = latlong.x + ((latlong.x + 30f * sign) / 2f);

        // Convert the latitude-longitude to radian
        latlong = latlong * (Mathf.PI) / 180.0f;

        // Convert latitude-longitude to ECEF coordinate
        float c = 1 / Mathf.Sqrt(Mathf.Cos(latlong.x) * Mathf.Cos(latlong.x) + (1 - GLOBAL.EARTH_FLATTENING) * (1 - GLOBAL.EARTH_FLATTENING) * Mathf.Sin(latlong.x) * Mathf.Sin(latlong.x));
        float s = (1 - GLOBAL.EARTH_FLATTENING) * (1 - GLOBAL.EARTH_FLATTENING) * c;
        float X = (radius * c + 0) * Mathf.Cos(latlong.x) * Mathf.Cos(latlong.y);
        float Y = (radius * c + 0) * Mathf.Cos(latlong.x) * Mathf.Sin(latlong.y);
        float Z = (radius * s + 0) * Mathf.Sin(latlong.x);
        return new Vector3(-Y, Z, X);
    }


}
 