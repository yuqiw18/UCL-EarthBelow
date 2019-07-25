using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Public Access Functions
public class UTIL : MonoBehaviour
{
    // Convert latitude and longitude to ECEF coordniate
    // *MUST USE HIGH-POLY SPHERE MESH*
    // *LOW-POLY SPHERE WILL RESULT IN INCORRECT TEXTURE MAPPING*
    public static Vector3 ECEFCoordinateFromLatLong(Vector2 latlong, float radius)
    {
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

    // Use Haversine Formula
    public static int DistanceBetweenLatLong(Vector2 latlong1, Vector2 latlong2)
    {
        // Use kilometer
        float r = GLOBAL.EARTH_CRUST_RADIUS/1000;

        float phi1 = latlong1.x * Mathf.Deg2Rad;
        float phi2 = latlong2.x * Mathf.Deg2Rad;
        float deltaPhi = (latlong2.x - latlong1.x) * Mathf.Deg2Rad;
        float deltaLambda = (latlong2.y - latlong1.y) * Mathf.Deg2Rad;

        float a = Mathf.Sin(deltaPhi / 2.0f) * Mathf.Sin(deltaPhi / 2.0f) + Mathf.Cos(phi1) * Mathf.Cos(phi2) * Mathf.Sin(deltaLambda / 2.0f) * Mathf.Sin(deltaLambda / 2.0f);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1.0f - a));

        return Mathf.RoundToInt(r * c);
    }

    // Convert file name for loading files
    public static string FileNameParser(string fileName) {
        return (fileName.Replace(" ", "-")).ToLower();
    }
}
