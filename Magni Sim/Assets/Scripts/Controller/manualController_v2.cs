using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manualController_v2 : MonoBehaviour
{
  public WheelCollider leftWheel, rightWheel;
  public float motorPower = 750;
  public float throttle = 0.25f;
  // public float steerPower = 100;
  public float rightWheelTorque, leftWheelTorque;

  public GameObject centerOfMass;
  
  
  private Rigidbody rigidBody;
  private float horizontalInput, verticalInput;

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
    throttle = Mathf.Clamp(throttle, 0, 1);

    verticalInput = Input.GetAxis("Vertical");
    horizontalInput = Input.GetAxis("Horizontal");

    leftWheel.motorTorque = (verticalInput + horizontalInput) * motorPower * throttle;
    rightWheel.motorTorque = (verticalInput + (horizontalInput * -1)) * motorPower * throttle;

    // }
    // else if(horizontalInput < 0)
    // {
    //   leftWheel.motorTorque = horizontalInput * motorPower * -1;
    //   rightWheel.motorTorque = horizontalInput * motorPower;
    // }

  //   foreach(var wheel in wheels)
  //   {
  //     wheel.motorTorque = Input.GetAxis("Vertical") * motorPower;
  //     AnimateWheels(wheel);
  //   }

  //   for (int i = 0; i < wheels.Length; i++)
  //   {
  //     if(i < 2) // Applies steering only to first 2 wheels
  //     {
  //       wheels[i].steerAngle = Input.GetAxis("Horizontal") * steerPower;
  //     }
  //   }
    AnimateWheels(leftWheel);
    AnimateWheels(rightWheel);

    leftWheelTorque = leftWheel.motorTorque;
    rightWheelTorque = rightWheel.motorTorque;
  }
}
