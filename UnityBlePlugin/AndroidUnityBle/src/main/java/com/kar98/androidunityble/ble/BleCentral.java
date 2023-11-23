package com.kar98.androidunityble.ble;



import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothGatt;
import android.bluetooth.BluetoothGattCallback;
import android.bluetooth.BluetoothGattCharacteristic;
import android.bluetooth.BluetoothGattDescriptor;
import android.bluetooth.BluetoothGattService;
import android.bluetooth.BluetoothProfile;
import android.bluetooth.le.BluetoothLeScanner;
import android.bluetooth.le.ScanCallback;
import android.bluetooth.le.ScanResult;
import android.bluetooth.le.ScanSettings;
import android.widget.Toast;

import com.kar98.androidunityble.UnitySender;
import com.unity3d.player.UnityPlayer;

import java.util.List;
import java.util.UUID;

public class BleCentral{

    private final UnitySender unitySender;

    private final BluetoothAdapter adapter;
    private final BluetoothLeScanner scanner;
    private BluetoothGatt gatt;

    private final UUID CLIENT_CHARACTERISTIC_CONFIG = UUID.fromString("00002902-0000-1000-8000-00805f9b34fb");


    // constructor
    public BleCentral(BluetoothAdapter adapter) {
        this.adapter = adapter;

        scanner = this.adapter.getBluetoothLeScanner();

        unitySender = new UnitySender();
        unitySender.sendMessage("InitCentral");
    }

    // Search for devices
    @SuppressLint("MissingPermission")
    public void startScan() {
        Toast.makeText(UnityPlayer.currentActivity, "startScan() - AndroidUnityBle ", Toast.LENGTH_SHORT).show();
        ScanSettings.Builder scanSettings = new ScanSettings.Builder();
        scanSettings.setScanMode(ScanSettings.SCAN_MODE_LOW_LATENCY);
        ScanSettings settings = scanSettings.build();

        scanner.startScan(null, settings, scanCallback);
        unitySender.sendMessage("default", "native start scan");
    }

    // Stop scanning
    @SuppressLint("MissingPermission")
    public void stopScan() {
        Toast.makeText(UnityPlayer.currentActivity, "stopScan() - AndroidUnityBle ", Toast.LENGTH_SHORT).show();
        scanner.stopScan(scanCallback);
        unitySender.sendMessage("default", "native stop scan");
    }

    // leScannerのCallback
    private final ScanCallback scanCallback = new ScanCallback() {
        @Override
        public void onScanResult(int callbackType, ScanResult result) {
            super.onScanResult(callbackType, result);
            // Called each time a device is found during scanning

           // Toast.makeText(UnityPlayer.currentActivity, "onScanResult() - AndroidUnityBle ", Toast.LENGTH_SHORT).show();

            unitySender.sendMessage("default", "discover ble device");
            if(result.getDevice() == null) return;

            @SuppressLint("MissingPermission")
            String deviceName = result.getDevice().getName();
            String address = result.getDevice().getAddress();
                  int _rssi = result.getRssi();
             String rssi = Integer.toString(_rssi);
            unitySender.sendMessage("ScanCallback", deviceName, address, rssi);
           // Toast.makeText(UnityPlayer.currentActivity, "Device : "+deviceName+", Address : "+address+", Rssi : "+rssi, Toast.LENGTH_SHORT).show();
        }

        @Override
        public void onBatchScanResults(List<ScanResult> results) {
            super.onBatchScanResults(results);

        }

        @Override
        public void onScanFailed(int errorCode) {
            super.onScanFailed(errorCode);
        }
    };

    // Connect to device
    @SuppressLint("MissingPermission")
    public void connect(String address) {
        BluetoothDevice device = adapter.getRemoteDevice(address);
        if (device == null) {
            return;
        }

        if (gatt != null) {
            gatt.disconnect();
        }
        gatt = device.connectGatt(UnityPlayer.currentActivity, false, gattCallback);
        String deviceName = device.getName();
        Toast.makeText(UnityPlayer.currentActivity, "Connected!!! to Device : "+deviceName+", Address : "+address, Toast.LENGTH_SHORT).show();
    }

    // disconnect from device
    @SuppressLint("MissingPermission")
    public void disconnect() {
        gatt.disconnect();
        gatt = null;

        Toast.makeText(UnityPlayer.currentActivity, "Disconnected!!!" , Toast.LENGTH_SHORT).show();
    }

    // discover services
    @SuppressLint("MissingPermission")
    public void discoverService() {
        gatt.discoverServices();
    }





    // Ask the device to send and read the characteristics
    @SuppressLint("MissingPermission")
    public void readCharacteristic(String serviceUUID, String readUUID) {
        BluetoothGattService service = gatt.getService(UUID.fromString(serviceUUID));
        BluetoothGattCharacteristic characteristic
                = service.getCharacteristic(UUID.fromString(readUUID));

        gatt.readCharacteristic(characteristic);
    }

    // Send data to device
    @SuppressLint("MissingPermission")
    public void writeCharacteristicResponse(String serviceUUID, String writeUUID, byte[] value) {
        BluetoothGattService service = gatt.getService(UUID.fromString(serviceUUID));
        BluetoothGattCharacteristic characteristic
                = service.getCharacteristic(UUID.fromString(writeUUID));

        characteristic.setValue(value);
        gatt.writeCharacteristic(characteristic);
    }

    // Allow reading and set the notify
    @SuppressLint("MissingPermission")
    public void startNotification(String serviceUUID, String notificationUUID) {
        BluetoothGattService service = gatt.getService(UUID.fromString(serviceUUID));
        BluetoothGattCharacteristic characteristic = service.getCharacteristic(UUID.fromString(notificationUUID));

        gatt.setCharacteristicNotification(characteristic, true);
        BluetoothGattDescriptor descriptor = characteristic.getDescriptor(CLIENT_CHARACTERISTIC_CONFIG);
        descriptor.setValue(BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE);
        gatt.writeDescriptor(descriptor);
    }

    // stop reading notify
    @SuppressLint("MissingPermission")
    public void stopNotification(String serviceUUID, String notificationUUID) {
        BluetoothGattService service = gatt.getService(UUID.fromString(serviceUUID));
        BluetoothGattCharacteristic characteristic = service.getCharacteristic(UUID.fromString(notificationUUID));

        gatt.setCharacteristicNotification(characteristic, false);
        BluetoothGattDescriptor descriptor = characteristic.getDescriptor(CLIENT_CHARACTERISTIC_CONFIG);
        descriptor.setValue(BluetoothGattDescriptor.DISABLE_NOTIFICATION_VALUE);
        gatt.writeDescriptor(descriptor);
    }


    @SuppressLint("MissingPermission")
    public void startIndication(String serviceUUID, String notificationUUID) {
        BluetoothGattService service = gatt.getService(UUID.fromString(serviceUUID));
        BluetoothGattCharacteristic characteristic = service.getCharacteristic(UUID.fromString(notificationUUID));

        gatt.setCharacteristicNotification(characteristic, true);
        BluetoothGattDescriptor descriptor = characteristic.getDescriptor(CLIENT_CHARACTERISTIC_CONFIG);
        descriptor.setValue(BluetoothGattDescriptor.ENABLE_INDICATION_VALUE);
        gatt.writeDescriptor(descriptor);
    }




    private final BluetoothGattCallback gattCallback = new BluetoothGattCallback() {
        @Override
        public void onConnectionStateChange(BluetoothGatt gatt, int status, int newState) {
            super.onConnectionStateChange(gatt, status, newState);
            // Called when the connection state with the peripheral changes

            if (newState == BluetoothProfile.STATE_CONNECTED) {
                // connect
                unitySender.sendMessage("ConnectCallback");

                discoverService();
            }
            else if (newState == BluetoothProfile.STATE_DISCONNECTED) {
                // Disconnect
                unitySender.sendMessage("DisconnectCallback");
            }
             else if (newState == BluetoothProfile.STATE_CONNECTING) {
                // Connecting
                unitySender.sendMessage("ConnectingCallback");
            }
        }



       /* @Override
        public void onServicesDiscovered(BluetoothGatt gatt, int status) {
            super.onServicesDiscovered(gatt, status);
            // Called when a Peripheral service is found

            if (status == BluetoothGatt.GATT_SUCCESS) {
                // Notification of detected services and characteristics
                for (BluetoothGattService service : gatt.getServices()) {
                    for (BluetoothGattCharacteristic characteristic : service.getCharacteristics()) {
                        unitySender.sendMessage(
                                "DiscoverServiceCallback",
                                service.getUuid().toString(),
                                characteristic.getUuid().toString(),
                                String.valueOf(characteristic.getProperties())

                        );
                    }
                }
            }

        }*/




        @Override
        public void onServicesDiscovered(BluetoothGatt gatt, int status) {
            super.onServicesDiscovered(gatt, status);
            // Called when a Peripheral service is found

            if (status == BluetoothGatt.GATT_SUCCESS) {

                for (BluetoothGattService service : gatt.getServices()) {


                    unitySender.sendMessagePro(
                            "ServiceList",
                            service.getUuid().toString() );

                }



                // Notification of detected services and characteristics
                for (BluetoothGattService service : gatt.getServices()) {
                    for (BluetoothGattCharacteristic characteristic : service.getCharacteristics()) {


                            unitySender.sendMessagePro(
                                "SerCharPropsList",
                                    service.getUuid().toString(),
                                    characteristic.getUuid().toString(),
                                    String.valueOf(characteristic.getProperties())
                                    //+" # "+characteristic.getUuid().toString() +" # "+
                                    //String.valueOf(characteristic.getProperties())

                            );


                            unitySender.sendMessage(
                                "DiscoverServiceCallback",
                                 service.getUuid().toString(),
                                 characteristic.getUuid().toString(),
                                 String.valueOf(characteristic.getProperties()));


                    }
                }
            }

        }







        @Override
        public void onCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
            super.onCharacteristicRead(gatt, characteristic, status);
            // Called when read results are available

            //data acquisition
            int value = characteristic.getIntValue(BluetoothGattCharacteristic.FORMAT_UINT32, 0);
            //byte[] value = characteristic.getValue(); Arrays.toString(value);
            unitySender.sendMessage("CharacteristicReadCallback", String.valueOf(value));
        }

        @Override
        public void onCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, int status) {
            super.onCharacteristicWrite(gatt, characteristic, status);
            // write callback

            unitySender.sendMessage("CharacteristicWriteCallback", "Writing completion");
        }

        @Override
        public void onCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic) {
            super.onCharacteristicChanged(gatt, characteristic);
            // Called when the notify result is obtained

            // data acquisition

             //int value = characteristic.getIntValue(BluetoothGattCharacteristic.FORMAT_UINT32, 0);
            byte[] byteValue = new byte[3];
            byteValue   = characteristic.getValue();

            //byte[] value = characteristic.getValue(); Arrays.toString(value);
            // value.contentToString() == [12, 41, 43, 7]
            //unitySender.sendMessage("CharacteristicChangedCallback", String.valueOf(value));
            unitySender.sendMessagePro("Notify", String.valueOf(byteValue[0]), String.valueOf(byteValue[1]), String.valueOf(byteValue[2]) );
        }
    };

    // call when the app exits
    @SuppressLint("MissingPermission")
    public void close() {
        gatt.close();
        gatt = null;
    }

}
