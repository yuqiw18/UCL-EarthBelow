using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthPreviewer : MonoBehaviour
{

    public GameObject earthObject;

    public Material dayMaterial;
    public Material nightMaterial;

    private float rotationSpeed = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Parent Name:" + earthObject.transform.parent.name);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the Earth
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Moved)){
        
            // Get the displacement delta
            Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

            // Use the parent's local axis direction as reference
            earthObject.transform.Rotate(earthObject.transform.parent.transform.up, -deltaPosition.x * rotationSpeed, Space.World);
            earthObject.transform.Rotate(earthObject.transform.parent.transform.right, deltaPosition.y * rotationSpeed, Space.World);

        }

        // Scale the Earth
        if ((Input.touchCount == 2) && (Input.GetTouch(0).phase == TouchPhase.Moved)) { 
        
        
        
        }
    }

    private void OnDisable()
    {
        earthObject.SetActive(false);
    }

    private void OnEnable()
    {
        earthObject.SetActive(true);
    }
}
