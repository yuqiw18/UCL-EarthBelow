using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{

    public GameObject earthPreviewer;
    public GameObject holeSpawner;
    public GameObject earthMapper;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SwitchMode(int mode) {
        ResetMode();
        switch (mode) {
            case 0:
                earthPreviewer.SetActive(true);
                break;
            case 1:
                holeSpawner.SetActive(true);
                break;
            case 2:
                earthMapper.SetActive(true);
                break;
            default:
                break;
        
        }
    }

    private void ResetMode() {
        earthPreviewer.SetActive(false);
        holeSpawner.SetActive(false);
        earthMapper.SetActive(false);
    }
}
