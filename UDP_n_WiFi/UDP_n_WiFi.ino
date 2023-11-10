 
#include<ESP8266WiFi.h>
#include<WiFiUdp.h>

const char* ssid = "ARU'S";
const char* passwd = "aradhya2199";

IPAddress deviceIpBroadCast(192, 168, 1, 211);

unsigned int udpRemotePort = 8888;
const int UDP_PACKET_SIZE = 28;
char udpBuffer[UDP_PACKET_SIZE];

WiFiUDP udp;

void setup() {
  // put your setup code here, to run once:

  Serial.begin(115200);
  delay(10);

  Serial.print("Connecting to :");
  Serial.println(ssid);

  WiFi.begin(ssid,passwd);
  while(WiFi.status()!=WL_CONNECTED){
      delay(300);
      Serial.print("..");
  }
  
  Serial.println("Wifi Connected");
  Serial.println("IP address");
  Serial.println(WiFi.localIP());
  Serial.println("Starting UDP");

  strcpy(udpBuffer,"Connected");
  Serial.println("Connected");

  udp.beginPacket(deviceIpBroadCast,udpRemotePort);
  udp.write(udpBuffer,sizeof(udpBuffer));
  udp.endPacket();


  //This is the PIN for potentiometer
   pinMode(A0,INPUT);

  //This is the pin for Hall Sensor

  pinMode(D4,INPUT); 
}

void sendHall(String message){
  strcpy(udpBuffer,message.c_str()); 
  udp.beginPacket(deviceIpBroadCast, udpRemotePort);
  udp.write(udpBuffer, sizeof(udpBuffer));
  udp.endPacket();
  
}

void sendPotem(String message){
  strcpy(udpBuffer,message.c_str()); 
  udp.beginPacket(deviceIpBroadCast, udpRemotePort);
  udp.write(udpBuffer, sizeof(udpBuffer));
  udp.endPacket();
}

void Hall_sensor(){
  //This is the code for Hall Sensor
  int hall = digitalRead(D4);
  Serial.println(hall);
  delay(100);
  if(hall==0){
    Serial.println("GO");
    sendHall("_GO");
  }
  
}
void Potentiometer(){
  //This is code for Potentiometer
  int potm = analogRead(A0);
  Serial.println(potm);
  if(potm>980){
    sendPotem("1");
  }
  else if(potm>850 && potm<980){
    sendPotem("0.666");
  }
  else if(potm>690 && potm<850){
    sendPotem("0.333");
  }
  else if(potm>580 && potm<690){
    sendPotem("0");
  }
  else if(potm>350 && potm<580){
    sendPotem("-0.333");
  }
  else if(potm>220 && potm<350){
    sendPotem("-0.666");
  }
  else{
    sendPotem("-1");
  }
  delay(100);
  
}
void loop() {
  // put your main code here, to run repeatedly:
  //Potentiometer();
  //Hall_sensor();
  delay(2500);
  strcpy(udpBuffer,"Worked"); 
  udp.beginPacket(deviceIpBroadCast, udpRemotePort);
  udp.write(udpBuffer, sizeof(udpBuffer));
  udp.endPacket();

}
