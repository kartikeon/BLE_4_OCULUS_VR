using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlePlugin.Ble {
    interface Callbacks 
    {
        void OnInitialize() {}

        void OnDeviceScan(string deviceName, string address, string rssi) {}

        void OnConnect() {}

        void OnDisconnect() {}
    
        void OnRead(string value) {}

        void OnWrite() {}

        void OnNotify(string value) {}

        void OnError(string message) {}
    }
}