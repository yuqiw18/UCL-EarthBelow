using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// Global Variables and Public Access Functions
public static class CORE
{
    #region CORE.DATA.STRUCTURE
    // Geographical data
    [Serializable]
    public struct LocationInfo
    {
        public string name;
        public string country;
        public Vector2 coord;
        public string description;
        public bool panorama;
        public string landmark;
    }

    [Serializable]
    public class LocationDatabase
    {
        public List<LocationInfo> serializableList;
    }

    public static List<LocationInfo> LOCATION_DATABASE = new List<LocationInfo>();

    // Geological data
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

    #region CORE.PARAMETER
    //
    public static Vector2 USER_LATLONG = new Vector2(51.507351f, -0.127758f); // Default UK latitude and longitude
    public static Quaternion ROTATE_TO_TOP;

    // Geological parameters
    public static readonly float EARTH_INNER_CORE_RADIUS = 1216000f;
    public static readonly float EARTH_OUTER_CORE_RADIUS = EARTH_INNER_CORE_RADIUS + 2270000f;
    public static readonly float EARTH_LOWER_MANTLE_RADIUS = EARTH_OUTER_CORE_RADIUS + 2885000f;
    public static readonly float EARTH_CRUST_RADIUS = 6371000f;

    // Geographical parameters
    public static readonly float EARTH_PREFAB_RADIUS = 0.5f;
    public static readonly float EARTH_FLATTENING = 1.0f / 298.257224f;
    public static readonly float EARTH_PREFAB_SCALE_TO_REAL = (1.0f / EARTH_PREFAB_RADIUS) * EARTH_CRUST_RADIUS;
    #endregion

    #region CORE.JSON.LOADER
    // JSON requires a very strict key-value pair formatting together with special characters escaping
    // This process can be simplified using Visual Studio Code with extensions such as JSON Escaper
    // location.json data source: https://www.latlong.net/
    // layer.json data source: Wikipedia
    // 
    public static IEnumerator LoadDataFromJSON(string filePath, Action<string> callback)
    {
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

        yield return null;
        callback(jsonContent);
    }

    // JSON saving function template
    private static string SaveToJSON() {
        PlanetDatabase locationDatabase = new PlanetDatabase();
        locationDatabase.serializableList = PLANET_DATABASE;
        return JsonUtility.ToJson(locationDatabase);
    }
    #endregion

    #region CORE.ALGORITHM
    // Convert latitude and longitude to ECEF coordniate
    // *MUST USE HIGH-POLY SPHERE MESH*
    // *LOW-POLY SPHERE WILL RESULT IN INCORRECT TEXTURE MAPPING*
    public static Vector3 ECEFCoordinateFromLatLong(Vector2 latlong, float radius)
    {
        // Convert the latitude-longitude to radian
        latlong = latlong * (Mathf.PI) / 180.0f;

        // Convert latitude-longitude to ECEF coordinate
        float c = 1 / Mathf.Sqrt(Mathf.Cos(latlong.x) * Mathf.Cos(latlong.x) + (1 - EARTH_FLATTENING) * (1 - EARTH_FLATTENING) * Mathf.Sin(latlong.x) * Mathf.Sin(latlong.x));
        float s = (1 - EARTH_FLATTENING) * (1 - EARTH_FLATTENING) * c;
        float X = (radius * c + 0) * Mathf.Cos(latlong.x) * Mathf.Cos(latlong.y);
        float Y = (radius * c + 0) * Mathf.Cos(latlong.x) * Mathf.Sin(latlong.y);
        float Z = (radius * s + 0) * Mathf.Sin(latlong.x);
        return new Vector3(-Y, Z, X);
    }

    // Use Haversine Formula
    public static int DistanceBetweenLatLong(Vector2 latlong1, Vector2 latlong2)
    {
        // Use kilometer
        float r = EARTH_CRUST_RADIUS / 1000;

        float phi1 = latlong1.x * Mathf.Deg2Rad;
        float phi2 = latlong2.x * Mathf.Deg2Rad;
        float deltaPhi = (latlong2.x - latlong1.x) * Mathf.Deg2Rad;
        float deltaLambda = (latlong2.y - latlong1.y) * Mathf.Deg2Rad;

        float a = Mathf.Sin(deltaPhi / 2.0f) * Mathf.Sin(deltaPhi / 2.0f) + Mathf.Cos(phi1) * Mathf.Cos(phi2) * Mathf.Sin(deltaLambda / 2.0f) * Mathf.Sin(deltaLambda / 2.0f);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1.0f - a));

        return Mathf.RoundToInt(r * c);
    }
    #endregion

    #region CORE.SPRITE.LOADER
    // Load any local image and return it as a 2D sprite using callback
    public static IEnumerator LoadImageToSprite(string filePath, Action<Sprite> callback)
    {
        byte[] imageData;
        Texture2D texture = new Texture2D(2, 2);

        // Read bytes using UnityWebRequest or File.ReadAllBytes
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            imageData = www.downloadHandler.data;
        }
        else
        {
            imageData = File.ReadAllBytes(filePath);
        }

        // Load raw data into Texture2D 
        texture.LoadImage(imageData);

        // Convert Texture2D to Sprite
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), pivot, 100.0f);

        yield return null;
        callback(sprite);
    }
    #endregion

    #region CORE.TEXTURE.LOADER
    // Load any local image and return it as a 2D texture using callback
    public static IEnumerator LoadImageToTexture(string filePath, Action<Texture2D> callback)
    {
        byte[] imageData;
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        // Read bytes using UnityWebRequest or File.ReadAllBytes
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            imageData = www.downloadHandler.data;
        }
        else
        {
            imageData = File.ReadAllBytes(filePath);
        }

        // Load raw data into Texture2D 
        texture.LoadImage(imageData);

        yield return null;
        callback(texture);
    }
    #endregion

    #region CORE.UTILITY
    // Convert file name for loading files
    public static string FileNameParser(string fileName)
    {
        return (fileName.Replace(" ", "_")).ToLower();
    }
    #endregion

}