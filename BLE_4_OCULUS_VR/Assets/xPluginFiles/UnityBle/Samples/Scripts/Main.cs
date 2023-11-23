using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using BlePlugin.Ble;
using BlePlugin.Util;
using BlePlugin.Data;

public class Main : MonoBehaviour, Callbacks
{
    // called when class is called
    void Start()
    {
        // Request location permissions
        PermissionUtil.RequestLocation();

        // BLE initialization
        BleController.Initialize(OnInitialize, OnError);
    }

    // Called when the app ends
    void OnDestroy()
    {
        // Perform BLE termination processing
        BleController.Close(OnError);
    }

    // Called on every frame update
    void Update() { }

    // callbacks
    private void OnInitialize()
    {
        // Implement processing after initialization
        Debug.Log("initialize");
    }
    private void OnError(string errorMessage)
    {
        // The reason why BLE cannot be initialized is returned with an error message
        // Initialize again or implement other processing
        Debug.Log("Ble Initialized Error: "+errorMessage);
    }
}