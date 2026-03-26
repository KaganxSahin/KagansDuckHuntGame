# 🦆 Duck Hunt Custom Controller (Unity + Arduino)

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
## 🎮 How the Light Gun Mechanism Works

The project utilizes a **Sequential Light Detection** technique, the same principle used by the original 1984 NES Zapper, to achieve high-accuracy targeting with simple hardware.

### The 5-Step Detection Cycle:

1.  **Trigger Input:** When the physical trigger is pulled, the **Arduino** detects a signal on **Digital Pin 3** and immediately sends a "Fire" command to **Unity** via Serial Communication.
2.  **Black Frame Masking:** For a split second (typically 1 frame), Unity renders the entire game screen **completely black**. This happens so fast it is barely perceptible to the human eye.
3.  **Target Highlighting:** Within that same black frame, Unity draws a **solid white circle** exclusively at the target's current (x,y) coordinates.
4.  **LDR Verification:** * **MISS:** If the gun is pointed at the background, the **LDR sensor** reads "LOW" (darkness).
    * **HIT:** If the gun is pointed at the target, the **LDR sensor** detects the white flash and returns a "HIGH" signal on **Digital Pin 2**.
5.  **Validation & Feedback:** Arduino relays the light detection data back to Unity. If Unity receives both the "Trigger" and "Light Detected" signals within the same sequence, a **Hit** is registered. Simultaneously, the Arduino activates the **L9110S driver** to provide haptic recoil.

> 🌑 **Note on Environmental Lighting:** Because the **LDR (Light Dependent Resistor)** is highly sensitive to the visible spectrum, ambient light from lamps or windows can cause "False Positives." A **dark room** ensures the sensor only triggers from the high-contrast white flash on the screen.
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
