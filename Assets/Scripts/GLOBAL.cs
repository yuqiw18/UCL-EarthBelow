using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLOBAL: MonoBehaviour
{
    #region DATA_LIST
    public static List<GameObject> PIN_LIST = new List<GameObject>();
    public static List<Vector2> LATLONG_LIST = new List<Vector2>();
    #endregion

    #region EARTH_PARAMETERS
    public static readonly float EARTH_INNER_CORE_RADIUS = 1216f;
    public static readonly float EARTH_OUTER_CORE_RADIUS = EARTH_INNER_CORE_RADIUS + 2270f;
    public static readonly float EARTH_LOWER_MANTLE_RADIUS = EARTH_OUTER_CORE_RADIUS + 2885f;
    public static readonly float EARTH_CRUST_RADIUS = 6371f;
    #endregion

    private void Awake()
    {
        Debug.Log("Loaded");
    }

}
 