using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarthPreviewer : MonoBehaviour
{

    public GameObject previewerOptions;
    public GameObject earthObject;

    private readonly float rotationSpeed = 0.25f;
    private readonly float zoomSpeed = 0.005f;

    private Material earthMaterial;
    private float transitionDirection = 1;
    private readonly float transitionSpeed = 0.5f;
    private float currentAlpha = 0;
    private bool inTransition = false;

    // Start is called before the first frame update
    void Start()
    {
        earthMaterial = earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the Earth
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            // Get the displacement delta
            Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

            // Use the parent's local axis direction as reference without messing up its own transform
            earthObject.transform.Rotate(earthObject.transform.parent.transform.up, -deltaPosition.x * rotationSpeed, Space.World);
            earthObject.transform.Rotate(earthObject.transform.parent.transform.right, deltaPosition.y * rotationSpeed, Space.World);
        }

        // Scale the Earth
        if (Input.touchCount == 2)
        {
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
        if (inTransition)
        {
            currentAlpha += transitionDirection * transitionSpeed * Time.deltaTime;

            if (currentAlpha < 0)
            {
                earthMaterial.SetFloat("_AlphaBlending", 0);
                inTransition = false;
                currentAlpha = 0;
            }
            else if (currentAlpha > 1)
            {
                earthMaterial.SetFloat("_AlphaBlending", 1);
                inTransition = false;
                currentAlpha = 1;
            }
            else
            {
                earthMaterial.SetFloat("_AlphaBlending", currentAlpha);
            }
        }
    }

    private void OnDisable()
    {
        earthObject.SetActive(false);
        previewerOptions.SetActive(false);
    }

    private void OnEnable()
    {
        earthObject.SetActive(true);
        previewerOptions.SetActive(true);
    }

    public void ChangeMaterial(int index)
    {
        earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<Renderer>().material = earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<AlternativeMaterial>().materialList[index];
        earthMaterial = earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<Renderer>().material;
        earthMaterial.SetFloat("_AlphaBlending", currentAlpha);
    }

    public void TransitionMaterial()
    {
        // Only begin transition when it is not already in the process of transition
        if (!inTransition)
        {
            if (Mathf.RoundToInt(currentAlpha) == 1)
            {
                transitionDirection = -1;
            }
            else
            {
                transitionDirection = 1;
            }
            inTransition = true;
        }
    }

    public void SwitchLayer(int i)
    {
        Transform layers = earthObject.transform.Find("Group_Layers");

        foreach (Transform l in layers)
        {
            l.gameObject.SetActive(false);
        }
        layers.GetChild(i).gameObject.SetActive(true);

        if (layers.GetChild(i).localScale != layers.Find("Earth_Grid").localScale)
        {
            layers.Find("Earth_Grid").gameObject.SetActive(true);
            layers.Find("Earth_Border").gameObject.SetActive(true);
        }
    }
}
