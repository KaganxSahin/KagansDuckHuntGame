# 🦆 Duck Hunt Custom Controller (Unity + Arduino)

![Project Status](https://img.shields.io/badge/Status-Completed-success)
![Platform](https://img.shields.io/badge/Platform-Unity_2D-black)
![Hardware](https://img.shields.io/badge/Hardware-Arduino_Uno-blue)

A hardware-in-the-loop interactive game project that recreates the classic "Duck Hunt" experience using a custom-built physical light gun. This project seamlessly integrates Embedded C++ (Arduino) for hardware processing with C# (Unity) for game logic and UI.

## 🌟 Key Features
* **Hardware-Software Integration:** Real-time serial communication between an Arduino microcontroller and the Unity Game Engine.
* **Classic Light Detection Logic:** Utilizes an LDR (Light Dependent Resistor) to detect white targets on a black screen flash, perfectly mimicking the original NES Zapper mechanics.
* **Haptic Feedback:** Integrates an L9110S motor driver and a micro vibration motor to provide instantaneous physical recoil when the trigger is pulled.
* **Custom Hardware Design:** Built with a switch module featuring a built-in 10K pull-up resistor, ensuring stable logic levels (`!digitalRead`) and a clean, reliable circuit.

---

## 🔌 Hardware Architecture & Wiring

The custom controller is built using the following components wired to an **Arduino Uno**:

| Component | Arduino Pin | Description / Purpose |
| :--- | :--- | :--- |
| **LDR Sensor (DO)** | `Pin 2` | Detects the white target on the screen during the flash sequence. |
| **Switch Module (S)** | `Pin 3` | The trigger mechanism. Uses pull-up logic (Normally HIGH, goes LOW when pressed). |
| **L9110S Motor Driver (A-1A)** | `Pin 4` | Receives the signal to activate the micro vibration motor. |
| **L9110S Motor Driver (A-1B)**| `GND` | Ground connection for the motor driver signal. |
| **All VCC / V+ Pins** | `5V` | Shared power line for the sensor, switch, and motor driver. |
| **All GND / G Pins** | `GND` | Shared ground line for all components. |

> **Note on Haptic Setup:** Connect the micro vibration motor directly to the `Motor A` terminals on the L9110S driver. 

---

## 💻 Software Structure

The project is divided into two main environments:
1. **`Arduino_Code/DuckHunt_v2.ino`**: The embedded code responsible for reading the LDR, listening to the pull-up trigger, controlling the vibration motor, and sending data packages `[trigger_state, sensor_state]` to Unity via Serial (9600 baud rate).
2. **`Unity_Project/`**: The game engine environment containing the C# scripts:
   * `ComController.cs`: Manages the COM port connection.
   * `GameController.cs`: The core logic, timer, and screen flash mechanism.
   * `Duck.cs`: Target AI and animation states.
   * `ScoreObject.cs`: UI visual feedback.

---

## 🚀 How to Run the Project

1. **Hardware Setup:** Wire the components according to the table above.
2. **Flash the Arduino:** Open `Arduino_Code/DuckHunt_v2.ino` in the Arduino IDE and upload it to your board. Note the COM port number.
3. **Launch Unity:** Open the `Unity_Project` folder in Unity Hub.
4. **Play:** Press the Play button in the Unity Editor. Select your Arduino's COM port from the dropdown menu and start shooting!
