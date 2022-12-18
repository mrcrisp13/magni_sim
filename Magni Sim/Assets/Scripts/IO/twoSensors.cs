using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class twoSensors : MonoBehaviour
{
  public sensorIR leftSensor, rightSensor;
  public float frequency = 25f;
  
  private float lastTime;
  private ROSConnection ros;

  private string topicName;

  void Start()
  {
    ros = ROSConnection.GetOrCreateInstance();
    topicName = this.gameObject.transform.name.ToString() + "/sensors";
    ros.RegisterPublisher<Int8Msg>(topicName);
  }

  void FixedUpdate()
  {
    float timeElapsed = Time.time - lastTime;
    if(timeElapsed >= (1f/frequency))
    {
      lastTime = Time.time;
      // int[] messageArray = new int[sensors.Count];
      sbyte output = 0;

      if(leftSensor.feedback)
        output = 1;
      if(rightSensor.feedback)
        output += 2;
      
      Int8Msg outputMsg = new Int8Msg(output);
      ros.Publish(topicName, outputMsg);
    }
  }
}
