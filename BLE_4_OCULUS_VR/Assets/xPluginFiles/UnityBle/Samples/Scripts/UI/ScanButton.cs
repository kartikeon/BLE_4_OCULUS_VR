using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using BlePlugin.Ble;

public class ScanButton : MonoBehaviour
{
    private const string ACTION_TEXT_NAME = "ScanText";
    private const string STATE_TEXT_NAME = "ScanStateText";

    // flag to update the UI
    private FlagStream flagStream = FlagStream.GetInstance();

    // BLE-related
    private BleScanner scanner = new BleScanner();

    public void OnClick()
    {
        if (BleScanner.isScanning) {
            scanner.StopScan();

            // scan stop
            SampleCanvas.SetText(ACTION_TEXT_NAME, "Start Scan");
            SampleCanvas.SetText(STATE_TEXT_NAME, "undiscovered");
        } else {
            scanner.StartScan();

            // scan starts
            SampleCanvas.SetText(ACTION_TEXT_NAME, "Stop Scan");
            SampleCanvas.SetText(STATE_TEXT_NAME, "scanning");

            // What to do when the desired device is found
            flagStream.OnScanFlagChanged
                .Where(value => value)
                .Subscribe(value => 
                {
                    // scan stop
                    scanner.StopScan();
                    SampleCanvas.SetText(ACTION_TEXT_NAME, "Start Scan");
                    SampleCanvas.SetText(STATE_TEXT_NAME, "discovered");
                    SampleCanvas.SetInteractive("ConnectButton", true);

                    SampleCanvas.SetText("DeviceName", BleDataCache.deviceName);
                    SampleCanvas.SetText("DeviceAddress", BleDataCache.deviceAddress);
                });
        }
    }
}
