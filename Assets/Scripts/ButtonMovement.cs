using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMovement : MonoBehaviour
{
    public Transform UITargetPositon;
    public int speedMultiplier = 1;

    private Image icon;
    private Vector3 originalPosition;
    private bool canMove = false;
    private bool isMovingBack = false;
    private float moveSpeed, originalAlpha;

    void Start()
    {
        icon = this.transform.Find("Image_Icon").GetComponent<Image>();
        originalAlpha = icon.color.a;
        originalPosition = this.transform.position;
        moveSpeed = (UITargetPositon.position - originalPosition).magnitude;
        StartMovement();
    }

    public void StartMovement() {
        if (!canMove) {
            canMove = true;
            if (!isMovingBack)
            {
                isMovingBack = true;
                UIManager.StaticFadeOut(icon);
                StartCoroutine(Move(UITargetPositon.position));
            }
            else
            {
                isMovingBack = false;
                UIManager.StaticFadeIn(icon, originalAlpha);
                StartCoroutine(Move(originalPosition));
            }
        }
    }

    private IEnumerator Move(Vector3 targetPosition)
    {
        while (this.transform.position != targetPosition)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, moveSpeed * speedMultiplier * Time.deltaTime);
            yield return null;
        }
        canMove = false;
    }
}
