using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class GPIO : MonoBehaviour
{
  public List<sensorIR> sensors;
  public float frequency = 1f;
  
  private float lastTime;
  private ROSConnection ros;
  private List<string> topicNames;  

  void Start()
  {
    ros = ROSConnection.GetOrCreateInstance();

    topicNames = new List<string>();
    for(int i = 0; i < sensors.Count; i++)
    {
      topicNames.Add(this.gameObject.transform.name.ToString() + "/" + sensors[i].GetType().ToString() + i.ToString());
      ros.RegisterPublisher<BoolMsg>(topicNames[i]);
    }
  }

  void FixedUpdate()
  {
    float timeElapsed = Time.time - lastTime;
    if(timeElapsed >= (1f/frequency))
    {
      lastTime = Time.time;
      for(int i = 0; i < sensors.Count; i++)
      {
        BoolMsg output = new BoolMsg(sensors[i].feedback);
        ros.Publish(topicNames[i], output);
      }
    }
  }
}
