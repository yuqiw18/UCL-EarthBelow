using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Remove overlapped pins
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);


        if (collision.gameObject.tag=="Pin") {

            Debug.Log("PIN OVERLAPPED. TRIGGER SELF-DESTRUCTION.");

            Destroy(this.gameObject);

        } 
    }
}
