using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using BlePlugin.Ble;
using BlePlugin.Data;

public class BleReadButton : MonoBehaviour
{
    public void OnClick() 
    {
        if (BleController.connectionStatus == ConnectionStatus.connected) {
            // Watch the stream and change the text if it changes
            ReadValueStream readValueStream = ReadValueStream.GetInstance();
            readValueStream.OnValueChanged.Subscribe(value => 
            {
                SampleCanvas.SetText("BleDataText", value);
            });

            // if connected
            BleInteractor.ReadCharacteristic();
        } else {
            Debug.Log("Connect your device");
        }
    }
}
