using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSettings : MonoBehaviour
{

    public int frameRate = 60;

    private void Awake()
    {
        Application.targetFrameRate = frameRate;
    }

}
