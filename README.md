# VR_Cycling_Using_ESP8266_and_Unity

By Aradhya Talawar

## Testing of the fitness game
https://youtu.be/WDciMWy8f8w

## Installation
### Hardware Requirements:
* Potentiometer
* Hall Effect Sensor
* VR Headset
* Cycle Stand
* ESP8266 (NodeMCU)

### Software Requirements:
* Unity 3D Personal
* Visual Studio Code
* Android Studio
* Arduino IDE

## Flowchart
![Screenshot](Img/start%20working.jpg)

## E-R Diagram
![Screenshot](Img/ER_Digram.png)

## Software Development

### Game Development in Unity:

A game was created in Unity, a popular game engine, and a virtual bicycle (Game Object) was added, programmed to move within the game environment.

### Terrain Creation:

A virtual terrain or map was designed in Unity, providing a simulated landscape for cycling activities.
![Screenshot](Img/21.jfif)
![Screenshot](Img/1.jfif)
![Screenshot](Img/3.jfif)
Scripting in C#:
C# scripts were attached to the virtual bicycle in Unity. These scripts define the bicycle's functionalities, such as its movement on the terrain and control mechanisms.

## Hardware Integration
### ESP8266 WiFi Module:

The WiFi-enabled ESP8266 module, integrated into a programmable NodeMCU board, was employed for hardware-software communication.

### Sensor Attachment:

Various sensors were connected to the NodeMCU board using jumper wires. These sensors serve as input devices for the virtual bicycle in the game.

The two main sensors used are:

* Hall Sensor: Acts as a switch in the presence of a magnetic field, used to measure the RPM (Revolutions Per Minute) of the bicycle's rear tire.
* Potentiometer: Functions as a voltage regulator, determining the direction in which the virtual bicycle is moving.
![Screenshot](Img/hardware.jfif)
### Data Transmission Using UDP:

After the sensors gather data, the ESP8266 module transmits this information to the Unity-based game using UDP (User Datagram Protocol).
The NodeMCU sends data through UDP packets, which the game then receives and translates into meaningful inputs for the virtual cycling experience.

#### Architecture
![Screenshot](Img/Architecture.png)

## Final Assembly

### Building the VR Game for Android Smartphones:
The last step involves compiling and building the VR game for use on Android smartphones. Unity simplifies this process, allowing for straightforward compilation and deployment of the VR application to a mobile platform.

#### Screenshot of game being tested
![Screenshot](Img/vr%20test%20app.jpg)

#### Screenshot of game working proverly.
![Screenshot](Img/implemented%20game.jfif)

This comprehensive approach to project implementation combines innovative software solutions with practical hardware integrations, leading to an immersive VR cycling experience. If there are more specific aspects of the implementation you'd like to know about, feel free to ask!
