using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using BlePlugin.Ble;
using BlePlugin.Data;

public class ConnectButton : MonoBehaviour
{
    private const string ACTION_TEXT_NAME = "ConnectText";
    private const string STATE_TEXT_NAME = "ConnectStateText";

    // flag to update the UI
    private FlagStream flagStream = FlagStream.GetInstance();

    private BleConnector connector = new BleConnector();

    void OnDestroy()
    {
        // Disconnect when exiting the app
        if (BleController.connectionStatus == ConnectionStatus.connected) {
            connector.Disconnect();
        }
    }

    public void OnClick() 
    {
        if (BleController.connectionStatus == ConnectionStatus.connected) {
            // Cutting process
            if (BleController.characteristicStatus == CharacteristicStatus.notify) {
                // If Notify
                // unsubscribe from stream
                SampleCanvas.SetInteractive("BleReadButton", true);
                BleInteractor.StopNotification();
                NotifyValueStream.GetInstance().OnDispose();
            }
            
            connector.Disconnect();

            SampleCanvas.SetText(ACTION_TEXT_NAME, "Connect");
            SampleCanvas.SetText(STATE_TEXT_NAME, "disconnected");
            SampleCanvas.SetText("BleDataText", "No Data");
        } else {
            // If you find the device you want
            BleDevice device = BleDataCache.GetBleDevice();
            Debug.Log(device.address);
            connector.Connect(device.address);
            SampleCanvas.SetText(STATE_TEXT_NAME, "connecting");

            // When a connection is established with the device
            flagStream.OnConnectFlagChanged
                .Where(value => value)
                .Subscribe(value => 
                {
                    // UI change
                    SampleCanvas.SetText(ACTION_TEXT_NAME, "Disconnect");
                    SampleCanvas.SetText(STATE_TEXT_NAME, "connected");
                });
        }
    }
}
