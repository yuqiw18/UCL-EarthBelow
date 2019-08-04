using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    public GameObject[] modes;
    public GameObject[] modeOptions;
    public Image[] modeIcons;

    void Start()
    {
        SwitchMode(0);
    }

    public void SwitchMode(int modeIndex)
    {
        if (modeIndex >= 0 && modeIndex < modes.Length)
        {
            ResetMode();
            modes[modeIndex].SetActive(true);
            modeIcons[modeIndex].color = new Color(1, 0.8f, 0, modeIcons[modeIndex].color.a);
            modeOptions[modeIndex].SetActive(true);
        }
    }

    private void ResetMode()
    {
        foreach (GameObject mode in modes)
        {
            mode.SetActive(false);
        }

        foreach (Image icon in modeIcons)
        {
            icon.color = new Color(1,1,1, 0.75f);
        }

        foreach (GameObject option in modeOptions)
        {
            option.SetActive(false);
        }
    }
}
