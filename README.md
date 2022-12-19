# Magni Simulation
Unity project for Magni simulation developed by Matthew Crisp for SFSU ENGR 697.

https://user-images.githubusercontent.com/49082093/208378989-70bf6ead-d267-4c82-a8a3-1dcaa72df9fa.mp4

## Setup
In order to run everything, you'll have to do a little bit of setup work.

### Unity
The simulation of the Magni robot was built in Unity. It allows for the control of a digital recreation of the Magni via ROS Twist messages and provides sensor feedback for two IR sensors on the front of the robot.

![image](https://user-images.githubusercontent.com/49082093/208372404-68c7232d-1bf8-4019-b965-a09a59d8759f.png)

For the Unity end of the setup, you'll need to ensure that you have a 2020 version of the editor installed. The one used specifically for this project is 2020.3.38f1. Once the correct version of the engine is installed, clone this repo to your hard drive.

With the project open, update the ROS IP settings to reflect the IP address of the remote host you intend to run the ROS instance on. If you intend to run the ROS instance locally, keep it how it is!

![image](https://user-images.githubusercontent.com/49082093/208374827-5b2220fb-a3e4-4298-a3d8-a99e475bcd3b.png)


### ROS
The ROS infrastructure handles all of the backend automation for the simulation, getting sensor feedback from the robots inside Unity, determining 

In order to run the ROS side, it's necessary to have Docker installed. Then, in your terminal, run the following command:
```
docker run --magni_sim -p 6080:80 -p 10000:10000 mcrisp/magni_sim:magni_sim
```
This will create a Docker container from which you can launch the Linux instance where ROS Noetic and all of the ROS end of the project are already installed.
![image](https://user-images.githubusercontent.com/49082093/208370792-2911f937-2872-452b-830f-612559887538.png)

Once the container is running, browse to http://127.0.0.1:6080/ to see the VNC interface with the instance.

## Teleop Simulation
A basic test of the simulation is to control the robot via teleop controls in ROS. To do this, it's ideal to go to the Arena scene. There are already 3 Magni robots in this scene and by default they will all subscribe to their own Twist messages (for example, the robot magni1 will receive control commands via 'magni1/cmd_vel'). You can override these values if you wish. In this example, though, we only need to drive one. Select the first Magni and override the topic name so the robot will receive control commands via 'cmd_vel'.
![image](https://user-images.githubusercontent.com/49082093/208376584-e35f8e0a-b6d0-4b59-9482-01ac5b5d09bb.png)

When ready, press Play in the Unity editor to launch the simulation.

Then, on the ROS end, open two terminals. In one, enter the following command:
```
roslaunch ros_tcp_endpoint endpoint.launch
```

In the other, enter the following command:
```
rosrun teleop_twist_keyboard teleop_twist_keyboard.py
```

This will enable teleoperational control of the robot in your simulation via the ROS instance. If you'd like to see the ROS network diagram, open another terminal and type ```rqt```.

Your screen on the Linux instance should look something like this:

![image](https://user-images.githubusercontent.com/49082093/208377551-9f5d232a-6322-4cd2-9a22-400c2e80e352.png)

Now, something funny you can do is override all of the Magnis' control topics to 'cmd_vel' and play a little synchronized demolition derby.

![image](https://user-images.githubusercontent.com/49082093/208377925-1acc6d95-63a5-490e-a0e0-c17ace19ab39.png)

## Line Following Simulation
Here's the big demonstration: 4 Magnis running simultaneously, each with their own course to follow, all being orchestrated from a remote host using the same algorithm. You saw the recording of the simulation in action (and sped up 30x), now try it for yourself. Open up the 'PathFindingExample' scene. Everything is already set for the simulation. Press Play in the Untiy editor to launch the simulation.

Then, on the ROS end, open a terminal and enter the following command:
```
roslaunch magni_sim four_follow.launch
```

This will start automated the robots in the simulation. 

![image](https://user-images.githubusercontent.com/49082093/208380931-b730f570-6514-4f9e-be1b-b95e192fb609.png)

If you'd like to build your own courses, you can create a new scene and use any of the prefab blocks to do so. When you're ready, drag one or more Magni robots into the scene and place them on your courses, making sure to name them with the shown convention (ie 'magni1'). Then in ROS launch the magni_sim launch file for the appropriate number of robots in the scene, up to four (for example, ```one_follow.launch``` will control one robot while ```three_follow``` will control three).

![image](https://user-images.githubusercontent.com/49082093/208381263-47d1d347-1736-4dd1-8311-04cc8285b2d2.png)
