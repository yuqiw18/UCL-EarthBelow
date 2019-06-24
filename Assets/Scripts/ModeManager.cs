using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public GameObject[] modes;

    public void SwitchMode(int modeIndex) {
        if (modeIndex >= 0 && modeIndex < modes.Length) {
            ResetMode();
            modes[modeIndex].SetActive(true);
        }
    }

    private void ResetMode() {
        foreach (GameObject mode in modes) {
            mode.SetActive(false);
        }
    }
}
