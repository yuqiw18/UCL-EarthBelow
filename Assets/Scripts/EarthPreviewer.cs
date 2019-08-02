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

    private bool isSpawned = false;
    private bool canSpawn = false;
    private float currentSpawnProgress = 1.0f;
    private readonly float spawnSpeed = 0.5f;

    private bool initRotation = false;
    private bool targetTransformReached = false;
    private Vector3 startDirection, targetDirection;
    private Quaternion startQuaternion, targetQuaternion, lastRotation, currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        earthTransformPoint = earthObject.transform.parent;
        earthSpawnPoint = earthObject.transform.parent.parent;
       
        pinGroup = earthObject.transform.Find("Group_Pins").gameObject;
        earthRenderer = earthObject.transform.Find("Group_Layers").Find("Earth_Surface").GetComponent<Renderer>();
        SetMaterial(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn animation control
        if (canSpawn)
        {
            if (currentSpawnProgress > 0)
            {
                currentSpawnProgress -= spawnSpeed * Time.deltaTime;
                if (currentSpawnProgress < 0)
                {
                    currentSpawnProgress = 0;

                    earthRenderer.material.SetFloat("_Dissolve", currentSpawnProgress);
                    canSpawn = false;
                    pinGroup.SetActive(true);
                    SetMaterial(1);
                }
                else
                {
                    earthRenderer.material.SetFloat("_Dissolve", currentSpawnProgress);
                }
            }
        }

        //if (earthMaterial)
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

        // Gesture control
        if (isSpawned)
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
        if (isSpawned)
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
                if (Mathf.RoundToInt(GetMaterialAlpha()) == 0)
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
                if (Mathf.RoundToInt(GetMaterialAlpha()) == 0 )
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

    private IEnumerator AlphaBlendToZero()
    {
        SetMaterialAlpha(1);
        while (GetMaterialAlpha() > 0.0f)
        {
            SetMaterialAlpha(GetMaterialAlpha() - Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaBlendToOne()
    {
        SetMaterialAlpha(0);
        while (GetMaterialAlpha() < 1.0f)
        {
            SetMaterialAlpha(GetMaterialAlpha() + Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    private IEnumerator AlphaBlendOneToOne(int index)
    {
        // Fade to zero
        SetMaterialAlpha(1);
        while (GetMaterialAlpha() > 0.0f)
        {
            SetMaterialAlpha(GetMaterialAlpha() - Time.deltaTime * 0.5f);
            yield return null;
        }

        // Switch to the target material
        SetMaterial(index);

        // Fade to one again
        SetMaterialAlpha(0);
        while (GetMaterialAlpha() < 1.0f)
        {
            SetMaterialAlpha(GetMaterialAlpha() + Time.deltaTime * 0.5f);
            yield return null;
        }
        inTransition = false;
    }

    public void SetMaterial(int index)
    {
        earthRenderer.material = earthMaterialList[index];
        currentMaterialIndex = index;
    }

    public void SetMaterialAlpha(float alpha)
    {
        earthRenderer.material.SetFloat("_AlphaBlending", alpha);
    }

    public float GetMaterialAlpha()
    {
        return earthRenderer.material.GetFloat("_AlphaBlending");
    }

    public Material GetMaterial()
    {
        return earthRenderer.material;
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
        isSpawned = true;
        canSpawn = true;
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
        currentSpawnProgress = 1.0f;
        SetMaterialAlpha(currentSpawnProgress);
        earthObject.SetActive(false);
        isSpawned = false;
        canSpawn = false;
    }
}
