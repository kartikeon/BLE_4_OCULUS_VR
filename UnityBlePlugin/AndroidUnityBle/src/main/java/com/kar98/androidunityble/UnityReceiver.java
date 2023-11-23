package com.kar98.androidunityble;

import com.kar98.androidunityble.ble.BleController;

public class UnityReceiver {
    private static BleController bleController;

    // Specify Central or Peripheral
    //
    //Initialize BLE
    public static void InitBle(String role) {
        bleController = new BleController(role);
    }

    // Call Ble Central functions
    //
    //for method with only methodName
    public static void CallCentralMethod(String methodName) {
        bleController.callCentralMethod(methodName);
    }

    // Use connect method
    public static void CallCentralMethod(String methodName, String address) {
        bleController.callCentralMethod(methodName, address);
    }

    // Use characteristic read or notification
    public static void CallCentralMethod(String methodName, String serviceUUID, String characteristicUUID) {
        bleController.callCentralMethod(methodName, serviceUUID, characteristicUUID);
    }

    // Use characteristic write
    public static  void CallCentralMethod(String methodName, String serviceUUID, String writeUUID, byte[] value) {
        bleController.callCentralMethod(methodName, serviceUUID, writeUUID, value);
    }
}
