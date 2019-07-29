using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMovement : MonoBehaviour
{

    public Transform UITargetPositon;
    public int speedMultiplier = 1;

    private Vector3 UIOrigin;
    private bool canMove = false;
    private bool isMovingTo = false;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        UIOrigin = this.gameObject.transform.position;
        speed = (UITargetPositon.position - UIOrigin).magnitude;
        StartMovement();
    }

    public void StartMovement() {
        if (!canMove) {
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        Vector3 targetPosition = Vector3.zero;
        canMove = true;

        if (!isMovingTo)
        {
            isMovingTo = true;
            targetPosition = UITargetPositon.position;
        }
        else {
            isMovingTo = false;
            targetPosition = UIOrigin;
        }

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
}
