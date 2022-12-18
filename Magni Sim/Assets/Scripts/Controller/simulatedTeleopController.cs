using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simulatedTeleopController : MonoBehaviour
{
  public WheelCollider leftWheel, rightWheel;
  public float motorPower = 750f;
  public int speed = 25;
  public float deadManTimer = 2f, timeLeft;
  public float rightWheelTorque, leftWheelTorque;
  public GameObject centerOfMass;

  public enum commands
  {
    ALL_STOP,
    STRAIGHT_AHEAD,
    STRAIGHT_BACK,
    ROTATE_CW,
    ROTATE_CCW,
    CIRCLE_LEFT,
    CIRCLE_RIGHT,
    CIRCLE_BACKWARDSLEFT,
    CIRCLE_BACKWARDSRIGHT
  }
  
  private Rigidbody rigidBody;
  private float horizontalInput, verticalInput;
  public bool newCommand;
  
  private commands currentCommand, previousCommand;

  private void AnimateWheel(WheelCollider wheelCollider)
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
    switch(currentCommand)
    {
      case commands.ALL_STOP:
        Debug.Log("ALL_STOP");
        leftWheel.motorTorque = 0f;
        rightWheel.motorTorque = 0f;
        break;
      case commands.STRAIGHT_AHEAD:
        Debug.Log("STRAIGHT_AHEAD");
        leftWheel.motorTorque = motorPower * (speed/100f);
        rightWheel.motorTorque = motorPower * (speed/100f);  
        break;
      case commands.STRAIGHT_BACK:
        Debug.Log("STRAIGHT_BACK");
        leftWheel.motorTorque = -motorPower * (speed/100f);
        rightWheel.motorTorque = -motorPower * (speed/100f);  
        break;
      case commands.ROTATE_CW:
        Debug.Log("ROTATE_CW");
        leftWheel.motorTorque = motorPower * (speed/100f);
        rightWheel.motorTorque = -motorPower * (speed/100f);  
        break;
      case commands.ROTATE_CCW:
        Debug.Log("ROTATE_CCW");
        leftWheel.motorTorque = -motorPower * (speed/100f);
        rightWheel.motorTorque = motorPower * (speed/100f);  
        break;
      case commands.CIRCLE_LEFT:
        Debug.Log("CIRCLE_LEFT");
        leftWheel.motorTorque = (motorPower * (speed/100f))/4;
        rightWheel.motorTorque = motorPower * (speed/100f);  
        break;
      case commands.CIRCLE_RIGHT:
        Debug.Log("CIRCLE_RIGHT");
        leftWheel.motorTorque = motorPower * (speed/100f);
        rightWheel.motorTorque = (motorPower * (speed/100f))/4;  
        break;
      case commands.CIRCLE_BACKWARDSLEFT:
        Debug.Log("CIRCLE_BACKWARDSLEFT");
        leftWheel.motorTorque = (-motorPower * (speed/100f))/4;
        rightWheel.motorTorque = -motorPower * (speed/100f);  
        break;
      case commands.CIRCLE_BACKWARDSRIGHT:
        Debug.Log("CIRCLE_BACKWARDSRIGHT");
        leftWheel.motorTorque = -motorPower * (speed/100f);
        rightWheel.motorTorque = (-motorPower * (speed/100f))/4;  
        break;
    }

    AnimateWheel(leftWheel);
    AnimateWheel(rightWheel);

    // Report torque per wheel
    leftWheelTorque = leftWheel.motorTorque;
    rightWheelTorque = rightWheel.motorTorque;

    UpdateTime();
    FixSpeed();
    FixRotation();
  }

  private void OnGUI()
  {
    GUIStyle leftAnchor = GUI.skin.GetStyle("Label");
    leftAnchor.alignment = TextAnchor.UpperLeft;
    GUI.Label(new Rect(10, 30, 400, 20), "Command: " + currentCommand, leftAnchor);
    GUI.Label(new Rect(10, 50, 400, 20), "Speed: " + speed + "%", leftAnchor);
    GUI.Label(new Rect(10, 70, 400, 20), "Move Duration: " + timeLeft + "s", leftAnchor);
  }

  /* Movement Functions */
  private void OnAllStop()
  {
    timeLeft = 0f;
    previousCommand = currentCommand;
    currentCommand = commands.ALL_STOP;
  }

  private void OnStraightAhead()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.STRAIGHT_AHEAD;
  }

  private void OnStraightBack()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.STRAIGHT_BACK;
  }

  private void OnRotateCW()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.ROTATE_CW;
  }

  private void OnRotateCCW()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.ROTATE_CCW;
  }

  private void OnCircleLeft()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.CIRCLE_LEFT;
  }

  private void OnCircleRight()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.CIRCLE_RIGHT;
  }

  private void OnCircleBackwardsLeft()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.CIRCLE_BACKWARDSLEFT;
  }

  private void OnCircleBackwardsRight()
  {
    timeLeft = deadManTimer;
    previousCommand = currentCommand;
    currentCommand = commands.CIRCLE_BACKWARDSRIGHT;
  }

  private void OnIncreaseSpeed()
  {
    speed += 5;
  }

  private void OnDecreaseSpeed()
  {
    speed -= 5;
  }

  private void UpdateTime()
  {
    if(timeLeft <= 0)
    {
      timeLeft = 0.0f;
      currentCommand = commands.ALL_STOP;
    }
    else
      timeLeft -= Time.deltaTime;
  }

  private void FixSpeed()
  {
    speed = Mathf.Clamp(speed, 0, 100);
  }

  private void FixRotation()
  {
    Vector3 fixedRotation = new Vector3(transform.localRotation.eulerAngles.x,transform.localRotation.eulerAngles.y,0);
    transform.eulerAngles = fixedRotation;
  }

  private void ResetPosition()
  {
    Vector3 startLocation = new Vector3(0,1,0);
    transform.position = startLocation;

    Vector3 fixedRotation = new Vector3(0,0,0);
    transform.eulerAngles = fixedRotation;
  }
}
