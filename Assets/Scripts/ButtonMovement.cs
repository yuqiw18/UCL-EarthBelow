using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMovement : MonoBehaviour
{

    public Transform UITargetPositon;
    public int speedMultiplier = 1;

    private Image icon;
    private Vector3 targetPosition;
    private Vector3 UIOrigin;
    private bool canMove = false;
    private bool isMovingBack = false;
    private float speed;
    private float originalAlpha;

    // Start is called before the first frame update
    void Start()
    {
        icon = this.transform.Find("Image_Icon").GetComponent<Image>();
        originalAlpha = icon.color.a;
        targetPosition = Vector3.zero;
        UIOrigin = this.transform.position;
        speed = (UITargetPositon.position - UIOrigin).magnitude;
        StartMovement();
    }

    //
    public void StartMovement() {
        if (!canMove) {
            if (!isMovingBack)
            {
                isMovingBack = true;
                targetPosition = UITargetPositon.position;
                StartCoroutine(FadeToZeroAlpha());
            }
            else
            {
                isMovingBack = false;
                targetPosition = UIOrigin;
                StartCoroutine(FadeToOriginalAlpha());
            }
            canMove = true;
            StartCoroutine(Move());
        }
    }

    //
    private IEnumerator Move()
    {
        while (canMove)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, speed * speedMultiplier * Time.deltaTime);
            Vector3 tempPosition = this.transform.position;
            if (tempPosition == targetPosition) {
                canMove = false;
            }
            yield return null;
        }
    }

    private IEnumerator FadeToOriginalAlpha()
    {
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
        while (icon.color.a < originalAlpha)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, icon.color.a + Time.deltaTime * 2.0f);
            yield return null;
        }
    }

    private IEnumerator FadeToZeroAlpha()
    {
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
        while (icon.color.a > 0.0f)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, icon.color.a - Time.deltaTime * 2.0f);
            yield return null;
        }
    }
}
