using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    public GameObject aboutPanel;
    public GameObject hintsPanel;
    public GameObject backButton;
    public GameObject options;

    private GameObject menuBackground;

    private Vector3 originPostion;
    private Vector3 targetPosition;

    private float speed = 100.0f;
    private float speedMultiplier = 2.0f;
    private bool canMove = false;
    private string state = "IDLE";

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
            menuBackground.transform.position = Vector3.MoveTowards(menuBackground.transform.position, targetPosition, speed * speedMultiplier * Time.deltaTime);
            Vector3 tempPosition = menuBackground.transform.position;
            if (tempPosition == targetPosition){
                canMove = false;
                switch (state) {
                    case "DOWN":
                        aboutPanel.SetActive(true);
                        backButton.SetActive(true);
                        break;
                    case "UP":
                        backButton.SetActive(true);
                        break;
                    case "IDLE":
                        aboutPanel.SetActive(false);
                        backButton.SetActive(false);
                        options.SetActive(false);
                        options.SetActive(true);
                        break;
                }
            }
         }
    }

    public void MoveBackground(string operation) {
        state = operation;
        switch (operation) {
            case "DOWN":
                targetPosition = originPostion + new Vector3(0, Screen.height / 2, 0);
                break;
            case "UP":
                targetPosition = originPostion + new Vector3(0, -Screen.height / 2, 0);
                break;
            case "PLACEHOLDER1":
                targetPosition = originPostion + new Vector3(-Screen.width / 2, 0, 0);
                break;
            case "PLACEHOLDER2":
                targetPosition = originPostion + new Vector3(Screen.width / 2, 0, 0);
                break;
            case "IDLE":
                targetPosition = originPostion;
                break;
            default: 
                break;
        }
        canMove = true;
        speed = Vector3.Distance(menuBackground.transform.position, targetPosition);
    }
}
