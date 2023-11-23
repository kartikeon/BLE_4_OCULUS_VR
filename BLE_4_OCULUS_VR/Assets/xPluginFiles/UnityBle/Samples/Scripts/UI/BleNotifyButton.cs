using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using BlePlugin.Ble;
using BlePlugin.Data;

public class BleNotifyButton : MonoBehaviour
{
    public void OnClick() 
    {
        if (BleController.connectionStatus == ConnectionStatus.connected) {
            // if connected
            if (BleController.characteristicStatus == CharacteristicStatus.notify) {
                SampleCanvas.SetInteractive("BleReadButton", true);

                // Unset if Notify
                BleInteractor.StopNotification();
            } else {
                // If Notify is not currently set
                // Watch the stream and change text in response to notifications
                NotifyValueStream notifyValueStream = NotifyValueStream.GetInstance();
                IDisposable iDisposable = notifyValueStream.OnValueChanged.Subscribe(value => 
                {
                   // SampleCanvas.SetText("BleDataText", value);
                });
                NotifyValueStream.iDisposable = iDisposable;

                SampleCanvas.SetInteractive("BleReadButton", false);

                // Configure notifications
                BleInteractor.StartNotification();
            }
        } else {
            Debug.Log("Connect your device");
        }
    }
}
