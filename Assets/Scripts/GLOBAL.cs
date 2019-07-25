using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Global Variables
public class GLOBAL: MonoBehaviour
{
    #region PSEUDO_DATABASE
    // Geographical info
    public struct LocationInfo
    {
        public string name;
        public string country;
        public Vector2 coord;
        public string description;
    }

    public LocationInfo InitialiseLocationInfo(string name, string country, Vector2 coord, string desc)
    {
        LocationInfo locationInfo;
        locationInfo.name = name;
        locationInfo.country = country;
        locationInfo.coord = coord;
        locationInfo.description = desc;
        return locationInfo;
    }

    public static List<LocationInfo> LOCATION_DATABASE = new List<LocationInfo>();

    // Geological info
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
        Debug.Log("Global Variable Loaded");
        InitialiseLatlongList();
        InitialiseEarthStructureInfo();
    }

    private void InitialiseLatlongList()
    {
        // Source: https://www.latlong.net/
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Paris", "France", new Vector2(48.864716f, 2.349014f), "Paris, France's capital, is a major European city and a global center for art, fashion, gastronomy and culture. Its 19th-century cityscape is crisscrossed by wide boulevards and the River Seine."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("New York", "United States", new Vector2(40.730610f, -73.935242f), "New York City comprises 5 boroughs sitting where the Hudson River meets the Atlantic Ocean. At its core is Manhattan, a densely populated borough that’s among the world’s major commercial, financial and cultural centers."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Melbourne", "Australia", new Vector2(-37.840935f, 144.946457f), "Melbourne is the coastal capital of the southeastern Australian state of Victoria. At the city's centre is the modern Federation Square development, with plazas, bars, and restaurants by the Yarra River."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Tokyo", "Japan", new Vector2(35.652832f, 139.839478f), "Tokyo, Japan’s busy capital, mixes the ultramodern and the traditional, from neon-lit skyscrapers to historic temples. The opulent Meiji Shinto Shrine is known for its towering gate and surrounding woods. The Imperial Palace sits amid large public gardens."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Auckland", "New Zealand", new Vector2(-36.848461f, 174.763336f), "Auckland, based around 2 large harbours, is a major city in the north of New Zealand’s North Island. In the centre, the iconic Sky Tower has views of Viaduct Harbour, which is full of superyachts and lined with bars and cafes."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Shanghai", "China", new Vector2(31.224361f, 121.469170f), "Shanghai, on China’s central coast, is the country's biggest city and a global financial hub. Its heart is the Bund, a famed waterfront promenade lined with colonial-era buildings."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Vancouver", "Canada", new Vector2(49.246292f, -123.116226f), "Vancouver, a bustling west coast seaport in British Columbia, is among Canada’s densest, most ethnically diverse cities. A popular filming location, it’s surrounded by mountains, and also has thriving art, theatre and music scenes."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("Moscow", "Russia", new Vector2(55.751244f, 37.618423f), "Moscow, on the Moskva River in western Russia, is the nation’s cosmopolitan capital. In its historic core is the Kremlin, a complex that’s home to the president and tsarist treasures in the Armoury. Outside its walls is Red Square, Russia's symbolic center."));
        LOCATION_DATABASE.Add(InitialiseLocationInfo("London", "United Kingdom", new Vector2(51.507351f, -0.127758f), "London, the capital of England and the United Kingdom, is a 21st-century city with history stretching back to Roman times. At its centre stand the imposing Houses of Parliament, the iconic ‘Big Ben’ clock tower and Westminster Abbey, site of British monarch coronations."));
    }

    private void InitialiseEarthStructureInfo()
    {
        LAYER_INFO.Add(InitialiseStructureInfo("Crust", "", "At the very top of the crust is where we live on but deeper down it is all dense rock and metal ores. The Crust is composed of mainly granite, basalt, and diorite rocks. Its thickness can vary from wherever you are. From a continent to the edge of the crust is about 60 km.  From the bottom of the ocean to the edge of the crust is about 10 km. The Crust's temperature is different throughout the entire crust, it starts at about 200°C and can rise up to 400°C. The crust is constantly moving due to the energy exchange in its lower layers. These movements will cause earthquakes and volcanoes to erupt; such a phenomenon is also known as the Theory of Plate Tectonics."));
        LAYER_INFO.Add(InitialiseStructureInfo("Mantle", "", "The Mantle is the second layer of the Earth. It is about 2900 km thick and is the biggest which takes up 84% of the Earth. The Mantle is divided into two sections. The Asthenosphere, the bottom layer of the mantle made of plastic like fluid and The Lithosphere the top part of the mantle made of a cold dense rock. The average temperature of the mantle is 3000°C and it is composed of silicates of iron and magnesium, sulphides and oxides of silicon and magnesium. Convection currents happen inside the mantle and are caused by the continuous circular motion of rocks in the lithosphere being pushed down by hot molasses liquid from the Asthenosphere.  The rocks then melt and float up as molasses liquid because it is less dense and the rocks float down because it is denser."));
        LAYER_INFO.Add(InitialiseStructureInfo("Outer Core", "", "The Outer Core is the second to last layer of the Earth.  It is a magma like liquid layer that surrounds the Inner Core and creates Earth's magnetic field. The Outer Core is about 2200 km thick and is the second largest layer and made entirely out of liquid magma. Its temperature is about 4000 - 5000°C. The Outer Core is composed of iron and some nickel while there is very few rocks and iron and nickel ore left because of the Inner Core melting all the metal into liquid magma. Since the outer core moves around the inner core, Earth's magnetism is created."));
        LAYER_INFO.Add(InitialiseStructureInfo("Inner Core", "", "The Inner Core is the final layer of the Earth which is a solid ball made of metal. It is about 1250 km thick and is the second smallest layer of the Earth. Although it is one of the smallest, the Inner Core is also the hottest layer. The Inner Core is composed of an element named NiFe (Ni for Nickel and Fe for Ferrum also known as Iron). The Inner Core is about 5000-6000°C and it melts all metal ores in the Outer Core causing it to turn into liquid magma."));
    }
}
 