// --- Pin Definitions ---
int sensor = 2;  // LDR sensor input pin
int trigger = 3; // Trigger switch input pin
int motor = 4;   // Vibration motor output pin

// --- Timing Variables ---
int eTime = 0;   // Delay counter to control motor vibration duration
bool freeEvent;  // Unused event flag

void setup() {
  // Initialize pins
  pinMode(sensor, INPUT);
  pinMode(trigger, INPUT);
  pinMode(motor, OUTPUT);
  
  // Initialize serial communication
  Serial.begin(9600);
  while(!Serial); // Wait for serial connection to establish
}

void loop() {
  // Read inputs. Trigger is inverted (!) due to pull-up resistor logic.
  // Returns 1 when pressed, 0 when released.
  int triggerState = !digitalRead(trigger); 
  
  // Read sensor state (1 or 0)
  int sensorState = digitalRead(sensor); 
  
  // Format data as "trigger,sensor"
  char str[3];
  sprintf(str, "%d,%d", triggerState, sensorState);
  
  // Send data to Unity
  Serial.println(str);
  
  // Handle motor vibration logic
  runMotor();
  
  // Short delay for stability
  delay(1);
}

// --- Haptic Feedback Function ---
// Limits the vibration duration of the motor
void runMotor() {
  // Read inverted trigger state for motor logic
  int triggerState = !digitalRead(trigger); 
  
  // Increment timer if vibration has started
  if (eTime > 0) {
     eTime++;
  }
  
  // Start motor if trigger is pressed and timer is idle
  if ((triggerState == 1) && (eTime == 0)) {
    eTime = 1;
    digitalWrite(motor, 1); // Turn ON motor
  }
  
  // Stop motor after 50 cycles (vibration duration)
  if (eTime == 50) {  
    digitalWrite(motor, 0); // Turn OFF motor
  }
  
  // Reset timer after 80 cycles (cooldown period)
  if (eTime > 80) {  
     eTime = 0;
  }
}
