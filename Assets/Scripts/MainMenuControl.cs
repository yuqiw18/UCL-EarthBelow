using UnityEngine;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    public GameObject aboutPanel;
    public GameObject backButton;
    public Text startText, aboutText;

    private GameObject menuBackground;

    private Vector3 originalPostion, targetPosition;
    private float moveSpeed = 100.0f;
    private readonly float speedMultiplier = 2.0f;
    private bool canMove = false;
    private string state = "IDLE";

    void Start()
    {
        menuBackground = this.transform.Find("Image_Background").gameObject;
        originalPostion = menuBackground.transform.position;
    }

    void Update()
    {
        if (canMove)
        {
            menuBackground.transform.position = Vector3.MoveTowards(menuBackground.transform.position, targetPosition, moveSpeed * speedMultiplier * Time.deltaTime);
            Vector3 tempPosition = menuBackground.transform.position;
            if (tempPosition == targetPosition)
            {
                canMove = false;
                switch (state)
                {
                    case "DOWN":
                        aboutPanel.SetActive(true);
                        backButton.SetActive(true);
                        break;
                    case "UP":
                        this.gameObject.SetActive(false);
                        break;
                    case "IDLE":
                        aboutPanel.SetActive(false);
                        backButton.SetActive(false);
                        UIManager.StaticFadeIn(startText, 1.0f);
                        UIManager.StaticFadeIn(aboutText, 1.0f);
                        break;
                }
            }
        }
    }

    public void MoveBackground(string operation)
    {
        state = operation;
        switch (operation)
        {
            case "DOWN":
                targetPosition = originalPostion + new Vector3(0, Screen.height / 2, 0);
                break;
            case "UP":
                targetPosition = originalPostion + new Vector3(0, -Screen.height / 2, 0);
                break;
            case "IDLE":
                targetPosition = originalPostion;
                break;
            default:
                targetPosition = originalPostion;
                break;
        }
        canMove = true;
        moveSpeed = Vector3.Distance(menuBackground.transform.position, targetPosition);
    }
}
