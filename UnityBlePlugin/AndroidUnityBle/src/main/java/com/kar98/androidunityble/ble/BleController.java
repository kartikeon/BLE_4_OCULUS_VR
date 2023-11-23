package com.kar98.androidunityble.ble;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothManager;
import android.content.Context;
import android.content.pm.PackageManager;
import android.util.Log;
import android.widget.Toast;

import com.kar98.androidunityble.UnitySender;
import com.unity3d.player.UnityPlayer;

public class BleController {
    private static BleCentral central = null;

    public BleController(String role) {
        UnitySender unitySender = new UnitySender();

        if (!UnityPlayer.currentActivity.getPackageManager().hasSystemFeature(PackageManager.FEATURE_BLUETOOTH_LE)) {
            // If your device does not support BLE
            Toast.makeText(UnityPlayer.currentActivity, "Your Device Doesn't Support BLE", Toast.LENGTH_SHORT).show();
            unitySender.sendMessage("ErrorCallback", "unsupported");

        }

        BluetoothManager manager = (BluetoothManager) UnityPlayer.currentActivity.getSystemService(Context.BLUETOOTH_SERVICE);
        BluetoothAdapter adapter = manager.getAdapter();

        if (adapter == null || !adapter.isEnabled()) {
            // If your device does not allow BLE
            Toast.makeText(UnityPlayer.currentActivity, "Turn on Bluetooth and Location", Toast.LENGTH_SHORT).show();
            unitySender.sendMessage("ErrorCallback", "unauthorized");
            unitySender.sendMessagePro("BleEnable", "false");
            return;
        }
        else
        {
            unitySender.sendMessagePro("BleEnable", "true");
        }



        switch (role) {
            case "Central":
                central = new BleCentral(adapter);
                break;
            case "Peripheral":
                // not yet written
                break;
        }
    }

    public void callCentralMethod(String methodName) {
        switch (methodName) {
            case "startScan":
                central.startScan();
                break;

            case "stopScan":
                central.stopScan();
                break;

            case "disconnect":
                central.disconnect();
                break;

            case "discoverService":
                central.discoverService();
                break;

            case "close":
                central.close();
                break;

            default:
                Log.e("BleController", "Specified function not found");
                break;
        }
    }

    public void callCentralMethod(
            String methodName,
            String serviceUUID,
            String characteristicUUID
    ) {
        switch (methodName) {
            case "readCharacteristic":
                central.readCharacteristic(serviceUUID, characteristicUUID);
                break;

            case "startNotification":
                central.startNotification(serviceUUID, characteristicUUID);
                break;

            case "stopNotification":
                central.stopNotification(serviceUUID, characteristicUUID);
                break;

            case "startIndication":
                central.startIndication(serviceUUID, characteristicUUID);
                break;

            default:
                Log.e("BleController", "Specified function not found");
                break;
        }
    }

    public void callCentralMethod(
            String methodName,
            String address
    ) {
        if ("connect".equals(methodName)) {
            central.connect(address);
        } else {
            Log.e("BleController", "Specified function not found");
        }
    }
    public void callCentralMethod(
            String methodName,
            String serviceUUID,
            String writeUUID,
            byte[] value
    ) {
        if ("writeCharacteristic".equals(methodName)) {
            central.writeCharacteristicResponse(serviceUUID, writeUUID, value);
        } else {
            Log.e("BleController", "Specified function not found");
        }
    }
}
