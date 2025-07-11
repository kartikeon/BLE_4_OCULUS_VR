﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class BleApi : MonoBehaviour
{
    // dll calls

    public enum ScanStatus { PROCESSING, AVAILABLE, FINISHED };


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DeviceUpdate
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string id;
        [MarshalAs(UnmanagedType.I1)]
        public bool isConnectable;
        [MarshalAs(UnmanagedType.I1)]
        public bool isConnectableUpdated;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string name;
        [MarshalAs(UnmanagedType.I1)]
        public bool nameUpdated;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Service
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string uuid;
        };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
     public struct Characteristic
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string uuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string userDescription;
        };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct BLEData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public short[] buf ;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public short size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string deviceId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string serviceUuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string characteristicUuid;
        };   

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ErrorMessage
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string msg;
        };







    [DllImport("BleWinrtDll.dll", EntryPoint = "StartDeviceScan")]
    public static extern void StartDeviceScan();

    [DllImport("BleWinrtDll.dll", EntryPoint = "PollDevice")]
    public static extern ScanStatus PollDevice(ref DeviceUpdate device, bool block);

    [DllImport("BleWinrtDll.dll", EntryPoint = "StopDeviceScan")]
    public static extern void StopDeviceScan();

    [DllImport("BleWinrtDll.dll", EntryPoint = "ScanServices", CharSet = CharSet.Unicode)]
    public static extern void ScanServices(string deviceId);

    [DllImport("BleWinrtDll.dll", EntryPoint = "PollService")]
    public static extern ScanStatus PollService(out Service service, bool block);

    [DllImport("BleWinrtDll.dll", EntryPoint = "ScanCharacteristics", CharSet = CharSet.Unicode)]
    public static extern void ScanCharacteristics(string deviceId, string serviceId);

    [DllImport("BleWinrtDll.dll", EntryPoint = "PollCharacteristic")]
    public static extern ScanStatus PollCharacteristic(out Characteristic characteristic, bool block);

    [DllImport("BleWinrtDll.dll", EntryPoint = "SubscribeCharacteristic", CharSet = CharSet.Unicode)]
    public static extern bool SubscribeCharacteristic(string deviceId, string serviceId, string characteristicId, bool block);

    [DllImport("BleWinrtDll.dll", EntryPoint = "PollData")]
    public static extern bool PollData(out BLEData data, bool block);

    [DllImport("BleWinrtDll.dll", EntryPoint = "SendData")]
    public static extern bool SendData(in BLEData data, bool block);

    [DllImport("BleWinrtDll.dll", EntryPoint = "Quit")]
    public static extern void Quit();

    [DllImport("BleWinrtDll.dll", EntryPoint = "GetError")]
    public static extern void GetError(out ErrorMessage buf);







    public class BleWan
    {
        public static short[] ReadShorts()   
        {
            BleApi.BLEData packageReceived;
            bool result = BleApi.PollData(out packageReceived, true);

            if (result)
            {
             Debug.Log("Size: " + packageReceived.size);
             Debug.Log("From: " + packageReceived.deviceId);

              if (packageReceived.size > 512)
               throw new ArgumentOutOfRangeException("Package too large.");

                return packageReceived.buf;
            } 
            else
            {
                return new short[] { 0x0 };
            }
        }    

    }




 
}





 

 












 







