using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manualController : MonoBehaviour
{
  public WheelCollider[] wheels;
  public float motorPower = 100;
  public float steerPower = 100;

  public GameObject centerOfMass;
  public Rigidbody rigidBody;

  public void AnimateWheels(WheelCollider wheelCollider)
  {
    if(wheelCollider.transform.childCount == 0)
      return;

    Transform visualWheel = wheelCollider.transform.GetChild(0);

    Vector3 position;
    Quaternion rotation;
    wheelCollider.GetWorldPose(out position, out rotation);

    visualWheel.transform.position = position;
    visualWheel.transform.rotation = rotation;
  }

  void Start()
  {
    rigidBody = GetComponent<Rigidbody>();
    rigidBody.centerOfMass = centerOfMass.transform.localPosition;
  }

  void FixedUpdate()
  {
    foreach(var wheel in wheels)
    {
      wheel.motorTorque = Input.GetAxis("Vertical") * motorPower;
      AnimateWheels(wheel);
    }

    for (int i = 0; i < wheels.Length; i++)
    {
      if(i < 2) // Applies steering only to first 2 wheels
      {
        wheels[i].steerAngle = Input.GetAxis("Horizontal") * steerPower;
      }
    }
  }
}
