using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil_Controller : MonoBehaviour
{

    private bool planted = false;
    private bool water = false;

    void Start()
    {

    }

    void Update()
    {

    }

    public void setWater(bool state)
    {
        water = state;
        Debug.Log("com água");
    }
}
