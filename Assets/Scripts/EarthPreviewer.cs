using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EarthPreviewer : MonoBehaviour
{
    public GameObject previewerOptions;
    public GameObject earthObject;

    private bool showMagneticField = false;
    private bool showCountryBorder = false;

    private readonly float rotationSpeed = 0.25f;
    private readonly float zoomSpeed = 0.005f;

    private Material earthMaterial;
    private float transitionDirection = 1;
    private readonly float transitionSpeed = 0.5f;
    private float currentAlpha = 0;
    private bool inTransition = false;

    private bool isSpawned = false;

    private bool initRotation, targetTransformReached = false;
    private Vector3 startDirection, targetDirection;
    private Quaternion startQuaternion, targetQuaternion, lastRotation, currentRotation;

    private bool ARMode = false;
    private Transform earthSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        earthMaterial = earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<Renderer>().material;
        earthSpawnPoint = earthObject.transform.parent.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //// Rotate to the current location automatically once
        //if (!initRotation)
        //{
        //    if (earthObject.transform.Find("Group_Pins").childCount != 0)
        //    {
        //        earthObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //        startDirection = (earthObject.transform.Find("Group_Pins").GetChild(0).position - earthObject.transform.position).normalized;
        //        targetDirection = (Camera.main.transform.position - earthObject.transform.position).normalized;
        //        targetQuaternion = Quaternion.FromToRotation(startDirection, targetDirection);
        //        startQuaternion = earthObject.transform.rotation;
        //        initRotation = true;
        //    }
        //}

        //if (initRotation && !targetTransformReached)
        //{
        //    earthObject.transform.rotation = Quaternion.Lerp(startQuaternion, targetQuaternion, Time.time * 0.2f);
        //    currentRotation = earthObject.transform.rotation;

        //    earthObject.transform.localScale += new Vector3(0.025f, 0.025f, 0.025f) * Time.time;

        //    if (earthObject.transform.localScale.x > 1)
        //    {
        //        earthObject.transform.localScale = new Vector3(1, 1, 1);
        //    }

        //    if (currentRotation != lastRotation)
        //    {
        //        lastRotation = currentRotation;
        //    }
        //    else if (currentRotation == lastRotation && earthObject.transform.localScale == new Vector3(1, 1, 1))
        //    {
        //        targetTransformReached = true;
        //        Debug.Log("Reached");
        //    }
        //}







        if (isSpawned)
        {
            // Rotate the Earth manually in non-AR mode
            if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                // Get the displacement delta
                Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

                // Use the parent's local axis direction as reference without messing up its own transform
                earthObject.transform.Rotate(earthObject.transform.parent.up, -deltaPosition.x * rotationSpeed, Space.World);
                earthObject.transform.Rotate(earthObject.transform.parent.right, deltaPosition.y * rotationSpeed, Space.World);
            }

            // Scale the Earth manually in non-AR mode
            if (Input.touchCount == 2)
            {
                // Get the touch
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
                Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

                float previousTouchDeltaMagnitude = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
                float currentTouchDeltaMagnitude = (firstTouch.position - secondTouch.position).magnitude;

                float touchMagnitudeDifference = currentTouchDeltaMagnitude - previousTouchDeltaMagnitude;

                earthObject.transform.parent.localScale += new Vector3(zoomSpeed* touchMagnitudeDifference, zoomSpeed* touchMagnitudeDifference, zoomSpeed* touchMagnitudeDifference);

                if (earthObject.transform.parent.localScale.x < 0.1f) {
                    earthObject.transform.parent.localScale = Vector3.one * 0.1f;
                }
            }
        }
        else
        {

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
        if (isSpawned)
        {
            earthObject.SetActive(true);
        }
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
        }

        if (showMagneticField)
        {
            layers.Find("Earth_MagneticField").gameObject.SetActive(true);
        }

        if (showCountryBorder)
        {
            layers.Find("Earth_Border").gameObject.SetActive(true);
        }
    }

    public void ToggleMagneticField() {
        if (showMagneticField)
        {
            showMagneticField = false;
        }
        else {
            showMagneticField = true;
        }
        earthObject.transform.Find("Group_Layers").Find("Earth_MagneticField").gameObject.SetActive(showMagneticField);
    }

    public void ToggleCountryBorder()
    {
        if (showCountryBorder)
        {
            showCountryBorder = false;
        }
        else
        {
            showCountryBorder = true;
        }
        earthObject.transform.Find("Group_Layers").Find("Earth_Border").gameObject.SetActive(showCountryBorder);
    }

    public void SpawnEarth() {
        if (!isSpawned)
        {
            earthObject.transform.parent.parent = null;
            earthObject.SetActive(true);
            isSpawned = true;
        }
        else
        {
            // Relocation
            earthObject.transform.parent.SetParent(earthSpawnPoint);
            earthObject.transform.parent.transform.localPosition = Vector3.zero;
            earthObject.transform.parent.transform.localScale = Vector3.one;
            earthObject.transform.parent.parent = null;
            isSpawned = true;
        }
    }
}
