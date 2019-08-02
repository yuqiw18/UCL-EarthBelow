using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMovement : MonoBehaviour
{
    public Transform UITargetPositon;
    public int speedMultiplier = 1;

    private Image icon;
    private Vector3 targetPosition, originalPosition;
    private bool canMove = false;
    private bool isMovingBack = false;
    private float moveSpeed, originalAlpha;

    void Start()
    {
        icon = this.transform.Find("Image_Icon").GetComponent<Image>();
        originalAlpha = icon.color.a;
        targetPosition = Vector3.zero;
        originalPosition = this.transform.position;
        moveSpeed = (UITargetPositon.position - originalPosition).magnitude;
        StartMovement();
    }

    public void StartMovement() {
        if (!canMove) {
            if (!isMovingBack)
            {
                isMovingBack = true;
                targetPosition = UITargetPositon.position;
                UIManager.StaticFadeOut(icon);
            }
            else
            {
                isMovingBack = false;
                targetPosition = originalPosition;
                UIManager.StaticFadeIn(icon, originalAlpha);
            }
            canMove = true;
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        while (canMove)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, moveSpeed * speedMultiplier * Time.deltaTime);
            Vector3 tempPosition = this.transform.position;
            if (tempPosition == targetPosition) {
                canMove = false;
            }
            yield return null;
        }
    }
}
