using System;
using System.Collections.Generic;
using UnityEngine;
using BlePlugin.Util;
using BlePlugin.Data;
using System.Linq;
using System.Net;

namespace BlePlugin.Ble {
    public class BleReceiver : MonoBehaviour
    {
        public static BleReceiver instance;

        public Action OnInitialize;
        public Action<string, string, string> OnScanCallback;
        public Action OnConnect;
        public Action OnDisconnect;
        public Action<string> OnRead;
        public Action OnWrite;
        public Action<string> OnNotify;
        public Action<string> OnError;

        public List<string> ServiceList= new List<string>();
        public List<string> CharacteristicsList = new List<string>();
        public List<string> PropertyList = new List<string>();

        public string[] ServiceUUID, CharacteristicsUUID, PropertyID;


        public static BleReceiver GetOrCreateReceiver() 
        {
            if (instance == null) 
            {
                instance = new GameObject("BleReceiver").AddComponent<BleReceiver>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }

        public void PluginMessage(string message)
        {
            var param = message.Split(',');
            if (param.Length == 0)
            {
                return;
            }

            switch (param[0])
            {
                case "InitCentral":

                    Debug.Log("InitCentral");
                    BleController.bleStatus = BleStatus.ready;
                    OnInitialize?.Invoke();
                    break;

                case "ScanCallback":

                    Debug.Log("ScanCallback");
                    OnScanCallback?.Invoke(param[1], param[2], param[3]);

                    break;

                case "ConnectCallback":

                    Debug.Log("ConnectCallback");
                    BleController.connectionStatus = ConnectionStatus.connected;
                    OnConnect?.Invoke();
                    break;

                case "DisconnectCallback":

                    Debug.Log("DisconnectCallback");
                    BleController.connectionStatus = ConnectionStatus.disconnected;
                    OnDisconnect?.Invoke();
                    break;

                case "DiscoverServiceCallback":

                    // Debug.Log("DiscoverServiceCallback");
                    // Debug.Log(param[1]+param[2]+param[3]);

                    Debug.Log("DiscoverServiceCallback - Characteristics : " + param[1] +", "+ param[2] + ", " + param[3]);

                    // Use data classes to manage discovered characteristics and serviceUuids
                    Dictionary<AttProperty, bool> properties = BleUtil.GetAttProperties(param[3]);
                   // Debug.Log("Property: " +param[3]);

                    DiscoveredCharacterisitc characterisitc = new DiscoveredCharacterisitc(
                        characteristicUuid: param[2], 
                        serviceId: param[1], 
                        isReadable: properties[AttProperty.Read], 
                        isWritableWithoutResponse: properties[AttProperty.WriteWithoutResponse],
                        isWritableWithResponse: properties[AttProperty.WriteWithResponse],
                        isNotifiable: properties[AttProperty.Notify],
                        isIndicatable: properties[AttProperty.Indicate]
                    );

                    BleController.discoveredService.setValue(param[1], characterisitc);
                    break;

                case "CharacteristicReadCallback":

                    Debug.Log("CharacteristicReadCallback");
                    OnRead?.Invoke(param[1]);
                    break;

                case "CharacteristicWriteCallback":

                    Debug.Log("CharacteristicWriteCallback");
                    Debug.Log(param[1]);
                    OnWrite?.Invoke();
                    break;

                case "CharacteristicChangedCallback":

                    Debug.Log("CharacteristicChangedCallback");
                    OnNotify?.Invoke(param[1]);
                    break;

                case "ErrorCallback":
                    // Callback at Initialize
                    // Update Status message
                    if (param[1] == "unsupported") {
                        BleController.bleStatus = BleStatus.unsupported;
                    } else if (param[1] == "unauthorized") {
                        BleController.bleStatus = BleStatus.unauthorized;
                    } else if (param[1] == "locationServicesDisabled") {
                        BleController.bleStatus = BleStatus.locationServicesDisabled;
                    }

                    OnError?.Invoke(param[1]);
                    break;
                
                default:
                    Debug.Log("Defautlt: "+param[1]);
                    break;
            }
        }
    }
}