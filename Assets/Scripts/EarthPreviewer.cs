using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EarthPreviewer : MonoBehaviour
{
    public GameObject previewerOptions;
    public GameObject earthObject;
    public Material[] earthMaterialList;

    private GameObject pinGroup;
    private bool showMagneticField = false;
    private bool showCountryBorder = false;

    private Transform earthTransformPoint;
    private Transform earthSpawnPoint;
    private readonly float rotationSpeed = 0.25f;
    private readonly float scaleSpeed = 0.005f;

    private Renderer earthRenderer;
    private bool inTransition = false;
    private int currentMaterialIndex;

    private bool spawned = false;

    void Start()
    {
        earthTransformPoint = earthObject.transform.parent;
        earthSpawnPoint = earthObject.transform.parent.parent;
        pinGroup = earthObject.transform.Find("Group_Pins").gameObject;
        earthRenderer = earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<Renderer>();
        SetMaterial(0);
    }

    void Update()
    {
        // Gesture control
        if (spawned)
        {
            // Rotate the Earth manually 
            if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                // Only when the earth is visible
                if (earthObject.GetComponent<Renderer>().isVisible)
                {
                    // Get the displacement delta
                    Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

                    // Use the parent's local axis direction as reference without messing up its own transform
                    earthObject.transform.Rotate(earthObject.transform.parent.up, -deltaPosition.x * rotationSpeed, Space.World);
                    earthObject.transform.Rotate(earthObject.transform.parent.right, deltaPosition.y * rotationSpeed, Space.World);
                }
            }
            else if (Input.touchCount == 2)
            {
                // Only when the earth is visible
                if (earthObject.GetComponent<Renderer>().isVisible)
                {
                    // Get the touch
                    Touch firstTouch = Input.GetTouch(0);
                    Touch secondTouch = Input.GetTouch(1);

                    Vector2 firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
                    Vector2 secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

                    float previousTouchDeltaMagnitude = (firstTouchPreviousPosition - secondTouchPreviousPosition).magnitude;
                    float currentTouchDeltaMagnitude = (firstTouch.position - secondTouch.position).magnitude;

                    float touchMagnitudeDifference = currentTouchDeltaMagnitude - previousTouchDeltaMagnitude;

                    earthObject.transform.parent.localScale += new Vector3(scaleSpeed * touchMagnitudeDifference, scaleSpeed * touchMagnitudeDifference, scaleSpeed * touchMagnitudeDifference);

                    // Limit the scale to 0.1f so that it will not disappear
                    if (earthObject.transform.parent.localScale.x < 0.1f)
                    {
                        earthObject.transform.parent.localScale = Vector3.one * 0.1f;
                    }
                }
            }
            else
            {
                // Update the tranform parent
                earthObject.transform.parent = null;
                earthTransformPoint.transform.LookAt(Camera.main.transform);
                earthTransformPoint.transform.Rotate(new Vector3(0, 180, 0));
                earthObject.transform.SetParent(earthTransformPoint);
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
        if (spawned)
        {
            earthObject.SetActive(true);
        }
        previewerOptions.SetActive(true);
    }

    public void TransitionMaterial(int index)
    {
        // Only begin transition when it is not in the process of transition
        if (!inTransition)
        {
            inTransition = true;
            if (currentMaterialIndex != index)
            {
                if (Mathf.RoundToInt(GetMaterialValue("_AlphaBlending")) == 0)
                {
                    // Replace the material and start transition
                    SetMaterial(index);
                    StartCoroutine(AlphaBlendToOne());
                }
                else
                {
                    // Transition from 1 to 0 for the first material and then 0 to 1 for the second material
                    StartCoroutine(AlphaBlendOneToOne(index));
                }
            }
            else
            {
                // Transition between 0 and 1 for the same material
                if (Mathf.RoundToInt(GetMaterialValue("_AlphaBlending")) == 0 )
                {
                    StartCoroutine(AlphaBlendToOne());
                }
                else
                {
                    StartCoroutine(AlphaBlendToZero());
                }
            } 
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

    public void ToggleMagneticField()
    {
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

    // Spawn a new earth
    public void SpawnEarth() {

        // Attach to the camera-related spawn point
        earthObject.transform.parent.SetParent(earthSpawnPoint);

        // Reset transform
        earthObject.transform.parent.transform.localPosition = Vector3.zero;
        earthObject.transform.parent.transform.localScale = Vector3.one;

        // Detach from the camera-related spawn point
        earthObject.transform.parent.parent = null;

        pinGroup.SetActive(false);
        earthObject.SetActive(true);

        StartCoroutine(AlphaClipToZero());
    }

    // Reset earth
    public void ResetEarth()
    {
        SwitchLayer(0);
        earthObject.transform.Find("Group_Layers").Find("Earth_MagneticField").gameObject.SetActive(false);
        earthObject.transform.Find("Group_Layers").Find("Earth_Border").gameObject.SetActive(false);
        showMagneticField = false;
        showCountryBorder = false;
        SetMaterial(0);
        earthObject.SetActive(false);
        spawned = false;
    }

    // Material manipulation
    private IEnumerator AlphaBlendToZero()
    {
        SetMaterialValue("_AlphaBlending", 1);
        while (GetMaterialValue("_AlphaBlending") > 0.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") - Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaBlendToOne()
    {
        SetMaterialValue("_AlphaBlending", 0);
        while (GetMaterialValue("_AlphaBlending") < 1.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") + Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaBlendOneToOne(int index)
    {
        // Fade to zero
        SetMaterialValue("_AlphaBlending", 1);
        while (GetMaterialValue("_AlphaBlending") > 0.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") - Time.deltaTime * 0.5f);
            yield return null;
        }

        // Switch to the target material
        SetMaterial(index);

        // Fade to one again
        SetMaterialValue("_AlphaBlending", 0);
        while (GetMaterialValue("_AlphaBlending") < 1.0f)
        {
            SetMaterialValue("_AlphaBlending", GetMaterialValue("_AlphaBlending") + Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaClipToZero()
    {
        SetMaterialValue("_Dissolve", 1);
        while (GetMaterialValue("_Dissolve") > 0)
        {
            SetMaterialValue("_Dissolve", GetMaterialValue("_Dissolve") - Time.deltaTime * 0.5f);
            yield return null;
        }
        pinGroup.SetActive(true);
        SetMaterial(1);
        spawned = true;
    }

    public Material GetMaterial()
    {
        return earthRenderer.material;
    }

    public void SetMaterial(int index)
    {
        earthRenderer.material = earthMaterialList[index];
        currentMaterialIndex = index;
    }

    public void SetMaterialValue(string property, float value)
    {
        earthRenderer.material.SetFloat(property, value);
    }

    public float GetMaterialValue(string property)
    {
        return earthRenderer.material.GetFloat(property);
    }

}
