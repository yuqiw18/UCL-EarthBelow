using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    private GameObject menuBackground;

    public enum SHIFTMODE {TOP, BOTTOM, LEFT, RIGHT, CENTER };

    private Vector3 targetPosition;
    //private float targetTransf;

    // Start is called before the first frame update
    void Start()
    {
        menuBackground = this.transform.Find("Image_Background").gameObject;
        Debug.Log("Found");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveBackground(SHIFTMODE operation) {

        switch (operation) {

            case SHIFTMODE.TOP:
                targetPosition = new Vector3(0, Screen.height / 2, 0);
                break;
            case SHIFTMODE.BOTTOM:
                targetPosition = new Vector3(0, -Screen.height / 2, 0);
                break;
            case SHIFTMODE.LEFT:
                targetPosition = new Vector3(0, -Screen.width / 2, 0);
                break;
            case SHIFTMODE.RIGHT:
                targetPosition = new Vector3(0, Screen.width / 2, 0);
                break;
            case SHIFTMODE.CENTER:
                targetPosition = new Vector3(0, 0, 0);
                break;
        }
    }
}
