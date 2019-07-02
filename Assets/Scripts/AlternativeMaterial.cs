using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeMaterial : MonoBehaviour
{

    public Material[] materialList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Material GetMaterial(int i) {
        return materialList[i];
    }
}
