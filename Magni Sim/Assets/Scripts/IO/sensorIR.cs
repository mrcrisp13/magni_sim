using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensorIR : MonoBehaviour
{
    public bool feedback = false;
    public Color sensedColor = Color.white;
    private Color targetColor = Color.black;
    private float range = 0.2f;
    private Ray sensorRay;
    private RaycastHit sensorHit;


    void Start()
    {
      
    }

    void FixedUpdate()
    {
      // sensorRay = new Ray(transform.position, transform.forward);
      if(Physics.Raycast(transform.position, -transform.up, out sensorHit, range))
      {
        sensedColor = sensorHit.transform.GetComponent<MeshRenderer>().material.color;
        if(sensedColor == targetColor)
          feedback = true;
        else
          feedback = false;
      }
      else
        feedback = false;
    }
}
