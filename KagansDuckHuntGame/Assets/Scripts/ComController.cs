using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System;

public class ComController : MonoBehaviour
{
    // --- Variables ---
    public static SerialPort spCom; // Shared serial port instance
    public Dropdown dropBoxPort;    // UI Dropdown for selecting available COM ports
    public Text lbMsg;              // UI Text for displaying connection status/errors
    public int readTimeOut = 500;   // Maximum time (ms) to wait for a response from Arduino
    
    // --- Initialization ---
    void Start()
    {
        lbMsg.text = ""; // Clear status message on startup

        // Fetch all available COM ports on the computer and populate the UI dropdown
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            dropBoxPort.options.Add(new Dropdown.OptionData(port));
        }
    }

    // --- Cleanup ---
    private void OnDestroy()
    {
        // Ensure the serial port is properly closed when the game object is destroyed
        spCom.Close();
    }

    // --- Connection Handling ---
    // Attempts to open the selected port and fires a callback action if successful
    public void CreatePortWithCallback(Action act)
    {
        // Get the currently selected COM port name from the dropdown
        string value = dropBoxPort.options[dropBoxPort.value].text;
        
        // Initialize serial port with standard parameters (9600 baud rate)
        spCom = new SerialPort(value, 9600, Parity.None, 8, StopBits.One);
        bool isOK = false;
        
        if (spCom != null)
        {
            Debug.Log("COM NOW : " + spCom.IsOpen);
            
            if (!spCom.IsOpen)
            {
                try
                {
                    spCom.Open();
                    spCom.ReadTimeout = readTimeOut; // Set the 500ms timeout
                    
                    // Check if we receive any valid data from the Arduino
                    if (spCom.ReadByte() > 0)
                    {
                        isOK = true;
                        act.Invoke(); // Trigger the callback to start the game
                        Debug.Log(value + " OPENED!");
                    }
                }
                catch
                {
                    // Catch exceptions (e.g., port already in use, device disconnected)
                    lbMsg.text = "ERROR PORT!";
                }
            }
        }

        // Display error if the connection process failed
        if (!isOK)
            lbMsg.text = "PORT IS NOT READY!";
    }
}