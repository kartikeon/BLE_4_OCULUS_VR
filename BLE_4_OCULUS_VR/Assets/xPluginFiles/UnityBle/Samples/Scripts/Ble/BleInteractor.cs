using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using BlePlugin.Ble;
using BlePlugin.Data;

// responsible for communication with ble
public class BleInteractor : Callbacks {

    private static string serviceUUID = "19b10000-1000-537e-4f6c-d104768a1214";
    private static string notifyCharacteristic = "19b10000-1001-537e-4f6c-d104768a1214";
    private static string readCharacteristic = "19b10000-1001-537e-4f6c-d104768a1214";
    private static string writeCharacteristic = "write characteristic";
    

    // read data from characteristic
    public static void ReadCharacteristic()  
    {
        if (BleController.connectionStatus != ConnectionStatus.connected) return;
        BleController.ReadCharacteristic(serviceUUID, readCharacteristic, OnRead, OnError);
    }

    // Write data to characteristic
    public static void WriteWithCharacteristic(byte[] writeValue) 
    {
        if (BleController.connectionStatus != ConnectionStatus.connected) return;
        BleController.WriteCharacteristic(serviceUUID, writeCharacteristic, writeValue, OnWrite, OnError);
    }

    // Permission and setting to receive notifications
    public static void StartNotification() 
    {
        if (BleController.connectionStatus != ConnectionStatus.connected) return;
        BleController.StartNotification(serviceUUID, notifyCharacteristic, OnNotify, OnError);
    }

    // stop notify
    public static void StopNotification()
    {
        if (BleController.connectionStatus != ConnectionStatus.connected) return;
        BleController.StopNotification(serviceUUID, notifyCharacteristic, OnError);
    }

    // callbacks
    private static void OnRead(string value)
    {
        Debug.Log("Value: "+value);
        // Change stream data and emit events
        ReadValueStream readValueStream = ReadValueStream.GetInstance();
        readValueStream.SetValue(value);
    }
    private static void OnWrite()
    {
        // Called when writing is complete
        Debug.Log("Write result: True");
    }
    private static void OnNotify(string value)
    {
        Debug.Log("Value: "+value);
        // Change stream data and emit events
        NotifyValueStream notifyValueStream = NotifyValueStream.GetInstance();
        notifyValueStream.SetValue(value);
    }
    private static void OnError(string message)
    {
        Debug.Log(message);
    }
}