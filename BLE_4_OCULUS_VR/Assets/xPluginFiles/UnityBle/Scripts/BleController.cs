using System;
using UnityEngine;
using UnityEngine.Android;
using BlePlugin.Data;
using BlePlugin.Util;

namespace BlePlugin.Ble {
    public class BleController : MonoBehaviour
    {
        private static BleReceiver receiver;

        public static BleStatus bleStatus;

        public static ConnectionStatus connectionStatus;

        public static DiscoveredService discoveredService;

        public static CharacteristicStatus characteristicStatus;

        // static constructor
         static BleController() 
        {
            // Initialize the ble receiver
            receiver = BleReceiver.GetOrCreateReceiver();

            bleStatus = BleStatus.unknown;
            connectionStatus = ConnectionStatus.disconnected;
            discoveredService = new DiscoveredService();
            characteristicStatus = CharacteristicStatus.unknown;
        }

        // Check permissions and initialize necessary variables
        public static void Initialize(Action onInitialize, Action<string> onError) 
        {
            if (bleStatus != BleStatus.ready) {
                receiver.OnInitialize = onInitialize;
                receiver.OnError = onError;

                JavaClassUtil.CallStaticWithoutResponse("InitBle", "Central");
            }
        }

        // Scan Peripherals
        public static bool StartScan(Action<string, string, string> onScanCallback, Action<string> onError) 
        {
            if (bleStatus == BleStatus.ready) {
                receiver.OnScanCallback = onScanCallback;
                receiver.OnError = onError;

                JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "startScan");

                // to update the status
                return true;
            } else {
                return false;
            }
        }

        // Stop Scan
        public static void StopScan(Action<string> onError) 
        {
            if (bleStatus != BleStatus.ready) return;
            receiver.OnError = onError;

            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "stopScan");
        }

        // Connect to device
        public static void Connect(string address, Action onConnect, Action onDisconnect, Action<string> onError) 
        {
            // change state
            BleController.connectionStatus = ConnectionStatus.connecting;

            receiver.OnConnect = onConnect;
            receiver.OnDisconnect = onDisconnect;
            receiver.OnError = onError;
            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "connect", address);
        }

        //disconnect
        public static void Disconnect(Action<string> onError) 
        {
            // change state
            BleController.connectionStatus = ConnectionStatus.disconnecting;

            receiver.OnError = onError;
            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "disconnect");
        }

        // Search for connected device services => Callback called when found
        public static void DiscoverService(Action<string> onError) 
        {
            receiver.OnError = onError;
        
            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "discoverService");
        }

        

        // Characteristic read
        public static void ReadCharacteristic(string serviceUUID, string characteristicUUID, Action<string> onRead, Action<string> onError) 
        {
            receiver.OnRead = onRead;
            receiver.OnError = onError;

            // set the state of the characteristic
            characteristicStatus = CharacteristicStatus.read;
            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "readCharacteristic", serviceUUID, characteristicUUID);
        }

        // Characteristic write + response
        public static void WriteCharacteristic(string serviceUUID, string charcteristicUUID, byte[] message, Action onWrite, Action<string> onError)
        {
            receiver.OnWrite = onWrite;
            receiver.OnError = onError;

            characteristicStatus = CharacteristicStatus.write;
            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "writeCharacteristic", serviceUUID, charcteristicUUID, message);
        }

        // Allow and set to receive notify
        public static void StartNotification(string serviceUUID, string charcteristicUUID, Action<string> onNotify, Action<string> onError)
        {
            receiver.OnNotify = onNotify;
            receiver.OnError = onError;

            characteristicStatus = CharacteristicStatus.notify;
            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "startNotification", serviceUUID, charcteristicUUID);
        }

        // stop notify
        public static void StopNotification(string serviceUUID, string characteristicUUID, Action<string> onError)
        {
            receiver.OnError = onError;

            characteristicStatus = CharacteristicStatus.notify;//.Unknown
            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "stopNotification", serviceUUID, characteristicUUID);
        }

        // exit ble
        public static void Close(Action<string> onError)
        {
            receiver.OnError = onError;

            JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "close");
            bleStatus = BleStatus.poweredOff;
        }
    }
}