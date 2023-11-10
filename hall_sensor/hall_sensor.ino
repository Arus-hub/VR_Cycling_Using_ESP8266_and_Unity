void setup() {
  Serial.begin(115200);
  pinMode(D4,INPUT);

}

void loop() {
  // put your main code here, to run repeatedly:
   Serial.println(digitalRead(D4));
   delay(500);
}
