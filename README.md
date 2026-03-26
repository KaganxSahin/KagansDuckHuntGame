<div align="center">
  <img src="https://upload.wikimedia.org/wikipedia/commons/c/c4/Unity_2021_Logo.svg" alt="Unity Logo" width="250" />
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <img src="https://upload.wikimedia.org/wikipedia/commons/8/87/Arduino_Logo.svg" alt="Arduino Logo" width="200" />
</div>

<br>

# 🦆 Duck Hunt Custom Controller (Unity + Arduino)

![Project Status](https://img.shields.io/badge/Status-Completed-success)
![Platform](https://img.shields.io/badge/Platform-Unity_2D-black)
![Hardware](https://img.shields.io/badge/Hardware-Arduino_Uno-blue)

A hardware-in-the-loop interactive game project that recreates the classic "Duck Hunt" experience using a custom-built physical light gun. This project seamlessly integrates Embedded C++ (Arduino) for hardware processing with C# (Unity) for game logic and UI.

> ⚠️ **IMPORTANT ENVIRONMENTAL REQUIREMENT:** > For the Light Dependent Resistor (LDR) to accurately detect the white screen flashes without interference, **this game must be played in a dark or dimly lit room.** High ambient lighting or sunlight will cause false sensor readings and unstable targeting logic.

## 🌟 Key Features
* **Hardware-Software Integration:** Real-time serial communication between an Arduino microcontroller and the Unity Game Engine.
* **Classic Light Detection Logic:** Utilizes an LDR (Light Dependent Resistor) to detect white targets on a black screen flash.
* **Haptic Feedback:** Integrates an L9110S motor driver and a micro vibration motor to provide instantaneous physical recoil.
* **Custom Hardware Design:** Built with a switch module featuring a built-in 10K pull-up resistor for stable logic.

---

## 🔌 Hardware Architecture & Wiring

All sensor and trigger inputs are connected to **Digital Pins** on the Arduino Uno to ensure precise binary state detection (HIGH/LOW).

| Component | Arduino Pin | Pin Type | Description / Purpose |
| :--- | :--- | :--- | :--- |
| **LDR Sensor (DO)** | `D2` | **Digital Input** | Detects the white target on the screen. |
| **Switch Module (S)** | `D3` | **Digital Input** | The trigger mechanism (Pull-up logic). |
| **L9110S Driver (A-1A)** | `D4` | **Digital Output** | Controls the vibration motor activation. |
| **L9110S Driver (A-1B)**| `GND` | Ground | Reference ground for the driver signal. |
| **All VCC / V+ Pins** | `5V` | Power | Shared 5V power line for all modules. |
| **All GND / G Pins** | `GND` | Ground | Shared ground line for the entire circuit. |

> **Note on Haptic Setup:** Connect the micro vibration motor directly to the `Motor A` terminals on the L9110S driver. 

---

## 💻 Software Structure

The project is divided into two main environments:
1. **`Arduino_Code/DuckGun.ino`**: Embedded code reading **Digital Pins 2 & 3**, controlling the vibration on **Digital Pin 4**, and sending data via Serial.
2. **`Unity_Project/`**: The game engine environment containing the C# scripts:
   * `ComController.cs`: Manages the COM port connection.
   * `GameController.cs`: Core logic and screen flash mechanism.
   * `Duck.cs`: Target AI and animation states.
   * `ScoreObject.cs`: UI visual feedback.

---
