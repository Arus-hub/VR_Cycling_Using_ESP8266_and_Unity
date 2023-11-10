void setup() {
  // put your setup code here, to run once:
   Serial.begin(115200);
   pinMode(A0,INPUT);
   pinMode(LED_BUILTIN,OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
   int potm = analogRead(A0);
   Serial.println(potm);
   potm=map(potm, 0, 1023, -1, 1);
   Serial.println(potm);

   delay(500);
}
