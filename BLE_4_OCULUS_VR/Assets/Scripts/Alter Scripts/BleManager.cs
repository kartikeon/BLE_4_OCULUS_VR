using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class BleManager : MonoBehaviour
{
    public bool isScanningDevices = false;
    public bool isScanningServices = false;
    public bool isScanningCharacteristics = false;
    public bool isSubscribed = false;
    public TMP_Text deviceScanButtonText, deviceScanStatusText, serviceScanStatusText, 
    characteristicScanStatusText, subcribeText, errorText, THPDataText;
    public GameObject deviceScanResultProto;
    public Button deviceScanButton,serviceScanButton, characteristicScanButton, subscribeButton;
    public TMP_Dropdown serviceDropdown, characteristicDropdown;
    public Button writeButton;
    public InputField writeInput;


    public Transform scanResultRoot;
    public string selectedDeviceId;
    public string selectedServiceId;
    Dictionary<string, string> characteristicNames = new Dictionary<string, string>();
    public string selectedCharacteristicId;
    Dictionary<string, Dictionary<string, string>> devices = new Dictionary<string, Dictionary<string, string>>();
    string lastError;

    Thread readingThread;

    public short temp, humi, pres;

    public int uTemp, uHumi, uPres;

    // int -2,147,483,648 to 2,147,483,647
    // short -32,768 to 32,767







    // Start is called before the first frame update
    void Start()
    {
        scanResultRoot = deviceScanResultProto.transform.parent;
        deviceScanResultProto.transform.SetParent(null);
        readingThread = new Thread(ReadBleData);
    }

    // Update is called once per frame

    void Update() 
    {
        BLE_Init();

    }

    void BLE_Init()
    {
        //////////////////////////////////////

        uTemp = temp; 
        uHumi = humi; 
        uPres = pres;

        THPDataText.text = "Temperature : " + uTemp + " C ," + " Humidity : " + uHumi + " % , " + "Pressure : " + uPres + " kPa";
        
        //////////////////////////////////////

        BleApi.ScanStatus status;
        if (isScanningDevices)
        {
            BleApi.DeviceUpdate res = new BleApi.DeviceUpdate();
            do
            {
                status = BleApi.PollDevice(ref res, false);
                if (status == BleApi.ScanStatus.AVAILABLE)
                {
                    if (!devices.ContainsKey(res.id))
                        devices[res.id] = new Dictionary<string, string>() {
                            { "name", "" },
                            { "isConnectable", "False" }
                        };
                    if (res.nameUpdated)
                        devices[res.id]["name"] = res.name;
                    if (res.isConnectableUpdated)
                        devices[res.id]["isConnectable"] = res.isConnectable.ToString();
                    // consider only devices which have a name and which are connectable
                    if (devices[res.id]["name"] != "" && devices[res.id]["isConnectable"] == "True")
                    {
                        // add new device to list
                        if (scanResultRoot !=null) {
                            GameObject g = Instantiate(deviceScanResultProto, scanResultRoot);

                            g.name = res.id;
                            g.transform.GetChild(0).GetComponent<Text>().text = devices[res.id]["name"];
                            g.transform.GetChild(1).GetComponent<Text>().text = res.id;
                            g.transform.localScale = new Vector3(1, 1, 1);
                            // g.transform.position = new Vector3(0, 0, -64999.5f);
                        }
                    }
                }
                else if (status == BleApi.ScanStatus.FINISHED)
                {
                    isScanningDevices = false;
                    deviceScanButtonText.text = "Scan devices";
                    deviceScanStatusText.text = "Finished";
                  
                }
            } while (status == BleApi.ScanStatus.AVAILABLE);
        }
        if (isScanningServices)
        {
            BleApi.Service res = new BleApi.Service();
            do
            {
                status = BleApi.PollService(out res, false);
                if (status == BleApi.ScanStatus.AVAILABLE)
                {
                    serviceDropdown.AddOptions(new List<string> { res.uuid });
                    // first option gets selected
                    if (serviceDropdown.options.Count == 1)
                        SelectService(serviceDropdown.gameObject);
                }
                else if (status == BleApi.ScanStatus.FINISHED)
                {
                    isScanningServices = false;
                    serviceScanButton.interactable = true;
                    serviceScanStatusText.text = "Finished";
                }
            } while (status == BleApi.ScanStatus.AVAILABLE);
        }
        if (isScanningCharacteristics)
        {
            BleApi.Characteristic res = new BleApi.Characteristic();
            do
            {
                status = BleApi.PollCharacteristic(out res, false);
                if (status == BleApi.ScanStatus.AVAILABLE)
                {
                    string name = res.userDescription != "no description available" ? res.userDescription : res.uuid;
                    characteristicNames[name] = res.uuid;
                    characteristicDropdown.AddOptions(new List<string> { name });
                    // first option gets selected
                    if (characteristicDropdown.options.Count == 1)
                        SelectCharacteristic(characteristicDropdown.gameObject);
                }
                else if (status == BleApi.ScanStatus.FINISHED)
                {
                    isScanningCharacteristics = false;
                    characteristicScanButton.interactable = true;
                    characteristicScanStatusText.text = "Finished";
                }
            } while (status == BleApi.ScanStatus.AVAILABLE);
        }
        if (isSubscribed)
        {
           /* BleApi.BLEData res = new BleApi.BLEData();
            while (BleApi.PollData(out res, false))
            {
              
                //subcribeText.text = BitConverter.ToString(res.buf, 0, res.size);
               //subcribeText.text = Encoding.ASCII.GetString(res.buf, 0, res.size);
               
            }*/


            if (!readingThread.IsAlive)
                {
                    readingThread = new Thread(ReadBleData);
                    readingThread.Start();
                }

            subcribeText.text = "T: " + uTemp + ", H: " + uHumi + ", P: " + uPres;

            Debug.Log("T: " + uTemp + ", H: " + uHumi + ", P: " + uPres);
            


        }

        {
            // log potential errors
            BleApi.ErrorMessage res = new BleApi.ErrorMessage();
           // BleApi.GetError(out res);
            if (lastError != res.msg)
            {
                Debug.Log(res.msg);
                errorText.text = res.msg;
                lastError = res.msg;
            }
        }
    }

    public void ReadBleData(object obj)
    {
      short[] packageReceived = new short[3];
      
     packageReceived = BleApi.BleWan.ReadShorts();///BLE.ReadBytes();
  
       foreach (short p in packageReceived)
        {
            if (p == 0)
            {

                return;
            }

            else
            {
                temp = packageReceived[0];

                humi = packageReceived[1];

                pres = packageReceived[2];
            }
        }
    }


    /*public static short[] ReadShorts()
    {
        BleApi.BLEData packageReceived;
        bool result = BleApi.PollData(out packageReceived, false);

        if (result)
        {
            Debug.Log("Size: " + packageReceived.size);
            Debug.Log("From: " + packageReceived.deviceId);

            if (packageReceived.size > 512)
                throw new ArgumentOutOfRangeException("Package too large.");

            return packageReceived.buf;
        } else
        {
            return new short[] { 0x0 };
        }
    }*/


    /* public static byte[] ReadBytes()
   {
       BleApi.BLEData packageReceived;
       bool result = BleApi.PollData(out packageReceived, true);

       if (result)
       {
           Debug.Log("Size: " + packageReceived.size);
           Debug.Log("From: " + packageReceived.deviceId);

           if (packageReceived.size > 512)
               throw new ArgumentOutOfRangeException("Package too large.");

           return packageReceived.buf;
       } else
       {
           return new byte[] { 0x0 };
       }
   }*/

  
    private void OnApplicationQuit()
    {
        BleApi.Quit();
    }

    public void StartStopDeviceScan()
    {

        if (!isScanningDevices)
        {
            // start new scan
            if (scanResultRoot != null)
            {

                for (int i = scanResultRoot.childCount - 1; i >= 0; i--)
                    Destroy(scanResultRoot.GetChild(i).gameObject);
            }
            BleApi.StartDeviceScan();
            isScanningDevices = true;
            deviceScanButtonText.text = "Stop scan";
            deviceScanStatusText.text = "scanning";
        }
        else
        {
            // stop scan
            isScanningDevices = false;
            BleApi.StopDeviceScan();
            deviceScanButtonText.text = "Start scan";
            deviceScanStatusText.text = "stopped";
        }
    }

    public void SelectDevice(GameObject data)
    {
        if (scanResultRoot != null)
        {
            for (int i = 0; i < scanResultRoot.transform.childCount; i++)
            {
                var child = scanResultRoot.transform.GetChild(i).gameObject;
                child.transform.GetChild(0).GetComponent<Text>().color = child == data ? Color.red :
                    deviceScanResultProto.transform.GetChild(0).GetComponent<Text>().color;
            }
        }
        selectedDeviceId = data.name;
        serviceScanButton.interactable = true;
    }

    public void StartServiceScan()
    {
        if (!isScanningServices)
        {
            // start new scan
            serviceDropdown.ClearOptions();
            BleApi.ScanServices(selectedDeviceId);
            isScanningServices = true;
            serviceScanStatusText.text = "scanning";
            serviceScanButton.interactable = false;
        }
    }

    public void SelectService(GameObject data)
    {
        selectedServiceId = serviceDropdown.options[serviceDropdown.value].text;
        characteristicScanButton.interactable = true;
    }


        public void StartCharacteristicScan()
        {
            if (!isScanningCharacteristics)
            {
                // start new scan
                characteristicDropdown.ClearOptions();
                BleApi.ScanCharacteristics(selectedDeviceId, selectedServiceId);
                isScanningCharacteristics = true;
                characteristicScanStatusText.text = "scanning";
                characteristicScanButton.interactable = false;
            }
        }

    



    public void SelectCharacteristic(GameObject data)
    {
        string name = characteristicDropdown.options[characteristicDropdown.value].text;
        selectedCharacteristicId = characteristicNames[name];
        subscribeButton.interactable = true;
       // writeButton.interactable = true;
        
    }

    public void Subscribe()
    {
        // no error code available in non-blocking mode
        BleApi.SubscribeCharacteristic(selectedDeviceId, selectedServiceId, selectedCharacteristicId, false);
        isSubscribed = true;
       
    }

    /*public void Write()
    {
        byte[] payload = Encoding.ASCII.GetBytes(writeInput.text);
        BleApi.BLEData data = new BleApi.BLEData();
        data.buf = new short[512];
        data.size = (short)payload.Length;
        data.deviceId = selectedDeviceId;
        data.serviceUuid = selectedServiceId;
        data.characteristicUuid = selectedCharacteristicId;
        for (int i = 0; i < payload.Length; i++)
            data.buf[i] = payload[i];
        // no error code available in non-blocking mode
        BleApi.SendData(in data, false);
    }*/
       


    public void EulerData()
    {
            if (!readingThread.IsAlive)
                {
                    readingThread = new Thread(ReadBleData);
                    readingThread.Start();
                }

                uTemp = temp;
                uHumi = humi;
                uPres = pres;



    }



    


     void Awake()
    {
        // GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestoryOnLoad");

        // if (objs.Length > 1)
        // {
        //     Destroy(this.gameObject);
        // }
      
        DontDestroyOnLoad(this.gameObject);

        


    }




}
