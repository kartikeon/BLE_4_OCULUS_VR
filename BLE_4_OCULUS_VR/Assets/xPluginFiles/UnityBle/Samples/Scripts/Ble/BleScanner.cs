using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlePlugin.Ble;
using BlePlugin.Data;
using TMPro;

public class BleScanner : Callbacks {
    // State management
    public static bool isScanning = false;
    private FlagStream flagStream = FlagStream.GetInstance();

    private static List<BleDevice> discoveredDevices = new List<BleDevice>();

    private BleReceiver receiver = new BleReceiver();

    public List<string> DevListUpdate = new List<string>();

    
    public List<string> DeviceName = new List<string>();
    public List<string> DeviceMac = new List<string>();
    public List<string> DeviceRssi = new List<string>();




    // Used when searching for the desired device - for configuration
    private string deviceName = "Nano Sense";
    private string deviceAddress = "D0:D1:A3:08:1C:2F";

    //  Scan your device
    public void StartScan()
    {
        discoveredDevices.Clear();
        flagStream.SetScanFlag(false);

        isScanning = BleController.StartScan(OnDeviceScan, OnError);

        Debug.Log("start scan: scanning -> "+isScanning);
    }
    // stop scanning
    public void StopScan()
    {
        if (isScanning)
        {
            BleController.StopScan(OnError);
            isScanning = false;
        }
    }

   
    // callbacks
    public void OnDeviceScan(string name, string address, string rssi)
    {
        // If you stock discovered devices and the user chooses
        // check for duplicates
        bool duplication = false;
        
        for (int i=0; i<discoveredDevices.Count; i++) 
        {
            if (discoveredDevices[i].address == address) 
            {
                duplication = true;
            }
        }
        
        // If it is a newly discovered device, add it to the array
        if (!duplication)
        {
            Debug.Log("scan result: " + name + " , " + address);

            discoveredDevices.Add(
                new BleDevice(
                    name: name,
                    address: address

                )
            );

           

               string Name, Mac, Rssi;

                Name = name;
                DeviceName.Add(Name);

                Mac = address;
                DeviceMac.Add(Mac);

                Rssi = rssi;
                DeviceRssi.Add(Rssi);

            
                


            // If you have the desired device
            if (deviceName == name || deviceAddress == address)
            {
                BleDataCache.deviceName = name;
                BleDataCache.deviceAddress = address;
                flagStream.SetScanFlag(true);
            }
        }



    }
  

    private void OnError(string message)
    {
        Debug.Log(message);
    }
}