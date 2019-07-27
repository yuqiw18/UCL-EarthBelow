using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    private GameObject menuBackground;


    private Vector3 originPostion;
    private Vector3 targetPosition;
    
    private float speed = 100.0f;

    private bool canMove = false;
    //private float targetTransf;

    // Start is called before the first frame update
    void Start()
    {
        menuBackground = this.transform.Find("Image_Background").gameObject;
        originPostion = menuBackground.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
         if (canMove){
            float step =  speed * Time.deltaTime;
            menuBackground.transform.position = Vector3.MoveTowards(menuBackground.transform.position, targetPosition, step);
            Vector3 tempPosition = menuBackground.transform.position;
            if (tempPosition == targetPosition){
                canMove = false;
                Debug.Log("Reached");
            }
         }
    }

    public void MoveBackground(string operation) {

        switch (operation) {
            case "TOP":
                targetPosition = originPostion + new Vector3(0, Screen.height / 2, 0);
                break;
            case "BOTTOM":
                targetPosition = originPostion + new Vector3(0, -Screen.height / 2, 0);
                break;
            case "LEFT":
                targetPosition = originPostion + new Vector3(-Screen.width / 2, 0, 0);
                break;
            case "RIGHT":
                targetPosition = originPostion + new Vector3(Screen.width / 2, 0, 0);
                break;
            case "CENTER":
                targetPosition = originPostion;
                break;
            default: 
                break;
        }
        canMove = true;
        speed = Vector3.Distance(menuBackground.transform.position, targetPosition);

        Debug.Log("New Speed:" + speed);
        Debug.Log("Triggered");
    }
}
