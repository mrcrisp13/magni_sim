using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class teleopController : MonoBehaviour
{
  [field: Header("Magni ROS Settings")]
  [field: SerializeField] public string TopicName { get; private set; } = "cmd_vel";
  public bool overrideTopicName = false;
  public bool enableTimeout = true;
  public float timeout = 2f;
  
  [field: Header("Magni Parameters")]
  [field: SerializeField] public string getLinearSpeed;
  [field: SerializeField] public string getAngularSpeed;
  [field: SerializeField] public string getTimeLeft;

  [field: Header("Magni Components")]
  [field: SerializeField] public WheelCollider leftWheel;
  [field: SerializeField] public WheelCollider rightWheel;
  [field: SerializeField] public WheelCollider leftCaster;
  [field: SerializeField] public WheelCollider rightCaster;

  private float angularSpeed  = 0f,
                cmdTime       = 0f,
                differential  = 1.26f,
                linearSpeed   = 0f,
                maxAngle      = 0.5f,
                maxSpeed      = 1f,
                motorPower    = 17f,
                timeLeft      = 0f,
                wheelRadius   = 0.1f;  
  private GameObject centerOfMass;
  private Rigidbody rigidBody;
  private ROSConnection ros;

  
  void Start()
  {
    // Set name
    if(!overrideTopicName)
      TopicName = transform.name.ToString() + "/cmd_vel";

    // Get center of mass
    centerOfMass = this.transform.GetChild(3).gameObject;
    rigidBody = GetComponent<Rigidbody>();
    rigidBody.centerOfMass = centerOfMass.transform.localPosition;

    // Establish ROS connection and subscribe to the specified topic
    ros = ROSConnection.GetOrCreateInstance();
    ros.Subscribe<TwistMsg>(TopicName, ReceiveROSCmd);
  }

  void FixedUpdate()
  {
    if(enableTimeout && IsTimeout())
    {
      linearSpeed = 0f;
      angularSpeed = 0f;
    }

    float leftWheel_torque = Mathf.Clamp(linearSpeed, -maxSpeed, maxSpeed) / wheelRadius;
    float rightWheel_torque = leftWheel_torque;
    float differentialModifier = (Mathf.Clamp(angularSpeed, -maxAngle, maxAngle) * differential) / wheelRadius;

    leftWheel_torque = (angularSpeed != 0) ? (leftWheel_torque + differentialModifier) : leftWheel_torque;
    rightWheel_torque = (angularSpeed != 0) ? (rightWheel_torque - differentialModifier) : rightWheel_torque;

    AnimateWheel(leftWheel);
    AnimateWheel(rightWheel);
    GetParameters();
    SetDriveSpeed(leftWheel,leftWheel_torque);
    SetDriveSpeed(rightWheel,rightWheel_torque);
    SetDriveSpeed(leftCaster,leftWheel_torque);
    SetDriveSpeed(rightCaster,rightWheel_torque);
  }

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

  private void FixRotation()
  {
    Vector3 fixedRotation = new Vector3(transform.localRotation.eulerAngles.x,transform.localRotation.eulerAngles.y,0);
    transform.eulerAngles = fixedRotation;
  }

  private void GetParameters()
  {
    getLinearSpeed = Mathf.Clamp(linearSpeed, -maxSpeed, maxSpeed).ToString("#.00") + " m/s";
    getAngularSpeed = Mathf.Clamp(angularSpeed, -maxAngle, maxAngle).ToString("#.00") + " rad/s";
    getTimeLeft = timeLeft.ToString("#.00") + " s";
  }

  private bool IsTimeout()
  {
    float timeElapsed = Time.time - cmdTime;
    timeLeft = timeout - timeElapsed;

    if(timeLeft <= 0)
    {
      timeLeft = 0;
      return true;
    }
    
    return false;
  }

  private void OnGUI()
  {
    // Disabled, only used for debug
    // GUIStyle leftAnchor = GUI.skin.GetStyle("Label");
    // leftAnchor.alignment = TextAnchor.UpperLeft;
    // GUI.Label(new Rect(10, 30, 400, 20), "Linear Speed: " + Mathf.Clamp(linearSpeed, -maxSpeed, maxSpeed).ToString("#.00") + " m/s", leftAnchor); 
    // GUI.Label(new Rect(10, 50, 400, 20), "Angular Speed: " + Mathf.Clamp(angularSpeed, -maxAngle, maxAngle).ToString("#.00") + " rad/s", leftAnchor);
    // if(enableTimeout)
    //   GUI.Label(new Rect(10, 70, 400, 20), "Move Duration: " + timeLeft.ToString("#.00") + "s", leftAnchor);
  }

  private void ReceiveROSCmd(TwistMsg velocityCmd)
  {
    linearSpeed = (float)velocityCmd.linear.x;
    angularSpeed = -(float)velocityCmd.angular.z;
    cmdTime = Time.time;
  }

  private void SetDriveSpeed(WheelCollider wheel, float torque = float.NaN)
  {
    wheel.motorTorque = float.IsNaN(torque) ? ((2 * maxSpeed) / wheelRadius) * motorPower : torque * motorPower;
  }
}
