using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthPreviewer : MonoBehaviour
{

    public GameObject switchButton;

    public GameObject earthObject;

    public Material dayMaterial;
    public Material nightMaterial;

    private readonly float rotationSpeed = 0.25f;
    private readonly float zoomSpeed = 0.005f;

    private Renderer earthMaterialRenderer;
    private Material targetMaterial;
    private Material beginMaterial;
    private readonly float transitionDuration = 5.0f;
    private readonly float transitionSpeed = 0.1f;
    private bool transitionDayNight = false;
    

    // Start is called before the first frame update
    void Start()
    {
        earthMaterialRenderer = earthObject.gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the Earth
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Moved)){
        
            // Get the displacement delta
            Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

            // Use the parent's local axis direction as reference without messing up its own transform
            earthObject.transform.Rotate(earthObject.transform.parent.transform.up, -deltaPosition.x * rotationSpeed, Space.World);
            earthObject.transform.Rotate(earthObject.transform.parent.transform.right, deltaPosition.y * rotationSpeed, Space.World);

        }

        // Scale the Earth
        if (Input.touchCount == 2) {

            // Get the touch
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
            Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

            float previousTouchDeltaMagnitude = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
            float currentTouchDeltaMagnitude = (firstTouch.position - secondTouch.position).magnitude;

            float touchMagnitudeDifference = previousTouchDeltaMagnitude - currentTouchDeltaMagnitude;

            earthObject.transform.parent.gameObject.transform.localPosition += new Vector3(0, 0, zoomSpeed * touchMagnitudeDifference);
        }

        // Transition between day and night (smoothing the material change)
        if (transitionDayNight) {
            float lerp = Mathf.PingPong(Time.time, transitionDuration) / transitionDuration;
            earthMaterialRenderer.material.Lerp(beginMaterial, targetMaterial, lerp);

            if (earthMaterialRenderer.sharedMaterial = targetMaterial) {
                transitionDayNight = false;
            }
        }

        // Transition between day and night using shader (smoothing the material change)
        //if (transitionDayNight)
        //{
        //    earthMaterialRenderer.material.SetFloat("BlendAlpha", transitionSpeed * Time.deltaTime);
        //}

    }

    private void OnDisable()
    {
        earthObject.SetActive(false);
        switchButton.SetActive(false);
    }

    private void OnEnable()
    {
        earthObject.SetActive(true);
        switchButton.SetActive(true);
    }

    public void SwitchDayNight() { 

        // Only begin transition when it is not already in the process of transition
        if (!transitionDayNight) {
            if (earthMaterialRenderer.sharedMaterial == dayMaterial)
            {
                beginMaterial = dayMaterial;
                targetMaterial = nightMaterial;
            }
            else {
                beginMaterial = nightMaterial;
                targetMaterial = dayMaterial;
            }
            transitionDayNight = true;
        }
    
    }
}
