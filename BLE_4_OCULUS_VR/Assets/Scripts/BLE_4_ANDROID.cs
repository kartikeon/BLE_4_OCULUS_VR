using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Android;
using BlePlugin.Ble;
using BlePlugin.Util;
using BlePlugin.Data;
using UniRx;
using TMPro;




public class BLE_4_ANDROID : MonoBehaviour
{

    public GameObject GenContainer,PermissionTab,
        DeviceTab, ServiceTab, CharacsTab, LogTab, PropTab;


    //Permission Tab
    public TMP_Text PermissionStatus;
    private bool canStartBle = false;

    //LOG
    public TMP_Text PluginCheckLog, BleLog, ErrorLog,
                    ServiceLog, SerCharPropsLog, DeviceLog, DeviceIncObj;



    //Scene 1 Elements
    private bool isDeviceScanningDone;
    private bool isUnLoadDone = false;
    public TMP_Text ScanBtnTxt, ConnectBtnTxt, LoadBtnTxt, ScanStatusTxt;
    public GameObject DeviceResultListPrefab;
    public Transform DeviceCloneParent;
    private string selectedDeviceName, selectedDeviceMac;
    public Color deviceSelectColour;


    private List<string> NameList = new List<string>();
    private List<string> MacList = new List<string>();
    private List<string> RssiList = new List<string>();
    private List<GameObject> DeviceListDestory = new List<GameObject>();




    //Scene 2 Elements
    public TMP_Text serviceDeviceName, servcieDeviceMac;
    public GameObject ServiceResultListPrefab;
    public Transform ServiceCloneParent;
    private string selectedServiceUUID;
    public Color servcieSelectColour;


    private List<string> ServicesList = new List<string>();
    private List<GameObject> ServiceListDestory = new List<GameObject>();




    //Scene 3 Elements
    public TMP_Text serviceCharUUID;
    public GameObject CharacteristicsResultListPrefab;
    public Transform CharacteristicsCloneParent;
    private string selectedCharacteristicsUUID, propertyNames;
    public Color characteristicsSelectColour;
   // public Button ReadButton, WriteButton, NotifyButton, IndicateButton;

    private List<string> ServiceRefList = new List<string>();
    private List<string> CharacteristicsRefList = new List<string>();
    private List<string> PropertyRefList = new List<string>();
    private List<GameObject> CharacteristicsListDestory = new List<GameObject>();



    //Scene 4 Elements
    private bool isNotify;
    public TMP_Text StartStopNotifyBtnTxt;
    public TMP_Text TempValue;
    private string Val;
    



    private FlagStream flagStream = FlagStream.GetInstance();
    private BleScanner scanner = new BleScanner();
    private BleConnector connector = new BleConnector();
    private BleReceiver receiver = new BleReceiver();
    private DiscoveredService discoveredService = new DiscoveredService();


    private const string PluginPath = "com.kar98.androidunityble.UnityReceiver";

    



    void Start()
    {

        pluginChecker();

        PermissionUtil.RequestLocation();
        
        BleController.Initialize(OnInitialize, OnError);

    }


    void Update()
    {

    }

    void Awake()
    {      
        DontDestroyOnLoad(gameObject);      
    }


    private void OnInitialize()
    {
        ErrorLog.text = "Plugin Initialized";
        Debug.Log("Plugin Initialized");
    }
    private void OnError(string errorMessage)
    {
        ErrorLog.text = errorMessage;
        Debug.Log("Ble Initialized Error: " + errorMessage);
    }

   


    void pluginChecker()
    {
        using var javaClass = new AndroidJavaClass(PluginPath);

        if (javaClass == null)
        {
            PluginCheckLog.text = "Plugin Error";
        }
        else
        {
            PluginCheckLog.text = "Plugin Working!!!";
        }

    }





    ////////////////////////////Tabs Functions///////////////////////////////////////////////////////

    //GenContainer, DeviceTab, ServiceTab, CharacsTab, PropTab, LogTab, 
    //PropContainer
    public void Gen_DeviceTab_Button()
    {
        PermissionTab.SetActive(false);
        
        DeviceTab.SetActive(true);
        ServiceTab.SetActive(false);
        CharacsTab.SetActive(false);
        PropTab.SetActive(false);
        LogTab.SetActive(false);

    }

    public void Gen_ServiceTab_Button()
    {
        PermissionTab.SetActive(false);

        DeviceTab.SetActive(false);
        ServiceTab.SetActive(true);
        CharacsTab.SetActive(false);
        PropTab.SetActive(false);
        LogTab.SetActive(false);
    }

    public void Gen_CharacsTab_Button()
    {
        PermissionTab.SetActive(false);

        DeviceTab.SetActive(false);
        ServiceTab.SetActive(false);
        CharacsTab.SetActive(true);
        PropTab.SetActive(false);
        LogTab.SetActive(false);
       
    }

    public void Gen_PropertyTab_Button()
    {
        PermissionTab.SetActive(false);

        DeviceTab.SetActive(false);
        ServiceTab.SetActive(false);
        CharacsTab.SetActive(false);
        PropTab.SetActive(true);
        LogTab.SetActive(false);
    }


    public void Gen_LogTab_Button()
    {
        PermissionTab.SetActive(false);

        DeviceTab.SetActive(false);
        ServiceTab.SetActive(false);
        CharacsTab.SetActive(false);
        PropTab.SetActive(false);
        LogTab.SetActive(true);  
    }

   







    ////////////////////////////Buttons Functions///////////////////////////////////////////////////////


    public void PermissionStart_Button() 
    {
        if(canStartBle) 
        {
           Gen_DeviceTab_Button();
        }
    
    }


    public void StartStopScan_Button()
    {
        if (BleScanner.isScanning)
        {
            StopScan_UI();
        }
        else
        {
            if (!isUnLoadDone)
            {
                StartScan_UI();

                Invoke(nameof(StopScan_UI), 10f);
            }

        }
    }
    
    public void LoadUnLoadDevicesList_Button()
    {

        if(isDeviceScanningDone)
        {   
            LoadBtnTxt.text = "UnLoad";

            DeviceResultUpdate();
            
            isDeviceScanningDone = false;
            isUnLoadDone = true;
        }
        else
        {   
            LoadBtnTxt.text = "Load";

            DeviceResultDestory();
            ServiceResultDestory();
            
            serviceDeviceName.text = "";
            servcieDeviceMac.text = "";

            isUnLoadDone = false;
        }   
    }

    public void ConnectDisconnectDevice_Button() 
    {
        ConnectDisconnectDevice();
    }


    public void EnterServiceList_Button() 
    {
        
        ServiceResultUpdate();
        Gen_ServiceTab_Button();
    }

    public void EnterCharacteristicsList_Button()
    {
        CharacteristicsGameObjDestory();
        CharacteristicsResultUpdate();
        Gen_CharacsTab_Button();
    }


    public void ClearLog_Button()
    {
        PluginCheckLog.text = "";
        BleLog.text = "";
        ErrorLog.text = "";
        DeviceLog.text = "";
        DeviceIncObj.text = "";
        ServiceLog.text = "";
        SerCharPropsLog.text = "";
        
    }

    public void Restart_Button()
    {

        ScanBtnTxt.text = "Scan";
        LoadBtnTxt.text = "Load";
        ConnectBtnTxt.text = "Connect";
        ScanStatusTxt.text = "";
        serviceDeviceName.text = "";
        servcieDeviceMac.text = "";
        serviceCharUUID.text = "";
        selectedServiceUUID = null;
        selectedCharacteristicsUUID = null;
        StartStopNotifyBtnTxt.text = "Start Notify";
        TempValue.text = "";


        isUnLoadDone = false;


        DeviceResultDestory();//Clears Device List
        ServiceResultDestory();//Clears Service List
        CharacteristicsResultDestory();//Clears Characteristics List
        connector.Disconnect();//Disconnects Current Device
        StopNotification();

        BleController.Close(OnError);//Close the GATT Connection
        SceneManager.LoadScene(0);
        Start();// Restart

    }

    public void CharacsOk_Button()
    {
        Gen_PropertyTab_Button();
    }


    public void StartStopNotify_Button() 
    {
        StartStopNotify();
    }






    ////////////////////////////////////////////////////////////////////////////////////////////////////
  




    void StartScan_UI()
    {
        scanner.StartScan();

        ScanBtnTxt.text = "Stop Scan";

        ScanStatusTxt.text = "Scanning...";

    }

    void StopScan_UI()
    {
        scanner.StopScan();

        ScanBtnTxt.text = "Scan";

        ScanStatusTxt.text = "Scan Finished";

        isDeviceScanningDone = true;
    }


    void DeviceResultUpdate()
    {

        NameList = scanner.DeviceName;
        MacList = scanner.DeviceMac;
        RssiList = scanner.DeviceRssi;

        DeviceLog.text = "Device : " + MacList.Count.ToString() + ",\n" + MacList[0]
                                              + ",\n" + MacList[1] + ",\n" + MacList[2]
                                              + ",\n" + MacList[3];


        for (int i = 0; i < MacList.Count; i++)
        {

            var Dev = Instantiate(DeviceResultListPrefab); 
            
            Dev.transform.SetParent(DeviceCloneParent);
            Dev.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = NameList[i];
            Dev.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = MacList[i];
            Dev.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = RssiList[i];
            Dev.transform.localScale = new Vector3(1,1,1);
            Dev.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            
            Dev.GetComponent<Button>().onClick.AddListener(SelectedDevice);
            DeviceListDestory.Add(Dev);


            DeviceIncObj.text = Dev.transform.localScale.ToString() + ",\n" +
                                DeviceResultListPrefab.transform.localScale.ToString();
                               
                    
        }
        

    }


    void DeviceResultDestory()
    {
        for (int i = 0; i < DeviceListDestory.Count; i++)
        {
            Destroy(DeviceListDestory[i]);
            NameList.Clear();
            MacList.Clear();
            RssiList.Clear();
        }

        DeviceListDestory = new List<GameObject>();
    }


    void SelectedDevice()
    {

        for (int i = 0; i < DeviceListDestory.Count; i++)
        {
            DeviceListDestory[i].GetComponent<Image>().color = Color.white;
        }

        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = deviceSelectColour;

        selectedDeviceName = EventSystem.current.currentSelectedGameObject
            .transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        selectedDeviceMac = EventSystem.current.currentSelectedGameObject
            .transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        // Using EventSystem by Clicked Device's Mac Text Get passed to a string


        Debug.Log("SelectedDevice() : " + selectedDeviceName +", "+ selectedDeviceMac);
        

    }



    //////////////////////////////Connection Part//////////////////////////////////////////////////////////

     void ConnectDisconnectDevice()
    {


        if (BleController.connectionStatus == ConnectionStatus.connected)
        {
            ConnectBtnTxt.text = "Connect";

            connector.Disconnect();

            serviceDeviceName.text = "";
            servcieDeviceMac.text = "";
            ServiceResultDestory(); // Clearing Current Service List While New List Arrives

            serviceCharUUID.text = "";
            CharacteristicsResultDestory();
            
            StopNotification();
            TempValue.text = "";


            BleLog.text = "Disconnected : " + selectedDeviceName + "\n" + selectedDeviceMac;

            Debug.Log("Disconnected : " + selectedDeviceName + "\n" + selectedDeviceMac);


            
            


        }
        else
        {
            ConnectBtnTxt.text = "Disconnect";

            connector.Connect(selectedDeviceMac);


            serviceDeviceName.text = selectedDeviceName;
            servcieDeviceMac.text = selectedDeviceMac;
            ServiceResultDestory(); // Clearing Current Service List While New List Arrives

            serviceCharUUID.text = "";
            CharacteristicsResultDestory();

            BleLog.text = "Connected : "+ "\n" + selectedDeviceName + "\n" + selectedDeviceMac;

            Debug.Log("Connected : " + selectedDeviceName +" | "+ selectedDeviceMac);

        }
    }

    //////////////////////////////Service Part//////////////////////////////////////////////////////////


   

    public void PluginMessage(string message)
    {
        var param = message.Split(',');
        if (param.Length == 0)
        {
            return;
        }

        switch (param[0])
        {
            case "BleEnable":

                if (param[1] == "false")
                {
                    canStartBle = false;

                    PermissionStatus.text = "Turn on Bluetooth And Location. Restart the App";
                }

                if (param[1] == "true")
                {
                    canStartBle = true;

                    PermissionStatus.text = "Ready!!! Please Start";
                }

            break;




            case "ServiceList":

                ServicesList.Add(param[1]);
               

                ServiceLog.text = "Service : " + ServicesList.Count.ToString() + ",\n" + ServicesList[0] 
                                              + ",\n" + ServicesList[1] + ",\n" + ServicesList[2] 
                                              + ",\n" + ServicesList[3];
               
                Debug.Log("BLE_4_ANDROID - ServiceList - Services : " + "Service : " + ServicesList.Count.ToString() + ",\n" + ServicesList[0]
                                              + ",\n" + ServicesList[1] + ",\n" + ServicesList[2]
                                              + ",\n" + ServicesList[3]);


                break;


            case "SerCharPropsList":

                ServiceRefList.Add(param[1]);
                CharacteristicsRefList.Add(param[2]);
                PropertyRefList.Add(param[3]);

                SerCharPropsLog.text = "Length : " + ServiceRefList.Count.ToString()
                                           + " | " + CharacteristicsRefList.Count.ToString()
                                           + " | " + PropertyRefList.Count.ToString()
                                           + " ,\n " + ServiceRefList[0]
                                           + " # " + CharacteristicsRefList[0]
                                           + " # " + PropertyRefList[0];




                Debug.Log("BLE_4_ANDROID - SerCharPropsList : " + "Length : " + ServiceRefList.Count.ToString()
                                                           + " | " + CharacteristicsRefList.Count.ToString()
                                                           + " | " + PropertyRefList.Count.ToString()
                                                           + " ,\n " + ServiceRefList[0]
                                                           + " # " + CharacteristicsRefList[0]
                                                           + " # " + PropertyRefList[0]);


                break;


            case "Notify":

                //TempValue.text = param[1];

                TempValue.text = param[1] +" | "+param[2]+" | "+param[3];

                break;

        }

        

    }

    void ServiceResultUpdate()
    {

        for (int i = 0; i < ServicesList.Count; i++)
        {

            var SER = Instantiate(ServiceResultListPrefab);
            SER.transform.SetParent(ServiceCloneParent);
            SER.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Service " + i;
            SER.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ServicesList[i];
            SER.transform.localScale = new Vector3(1, 1, 1);
            SER.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

            SER.GetComponent<Button>().onClick.AddListener(SelectedServcie);
            ServiceListDestory.Add(SER);
        }

    }

    void ServiceResultDestory()
    {
        for (int i = 0; i < ServiceListDestory.Count; i++)
        {
            Destroy(ServiceListDestory[i]);
            ServicesList.Clear(); 
            
        }

        ServiceListDestory = new List<GameObject>();
    }


    void SelectedServcie()
    {

        for (int i = 0; i < ServiceListDestory.Count; i++)
        {
            ServiceListDestory[i].GetComponent<Image>().color = Color.white;
        }

        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = servcieSelectColour;


        selectedServiceUUID = EventSystem.current.currentSelectedGameObject
            .transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        // Using EventSystem by Clicked Device's Service Text Get passed to a string


        Debug.Log("SelectedServcie() : " + selectedServiceUUID);


    }

    //////////////////////////////Characteristics Part//////////////////////////////////////////////////////////



    void CharacteristicsResultUpdate()
    {
        //ServiceRefList,CharacteristicsRefList,PropertyRefList
        serviceCharUUID.text = selectedServiceUUID;

        for (int i = 0; i < CharacteristicsRefList.Count; i++)
        {

            if(selectedServiceUUID == ServiceRefList[i])
            {               
                var CHAR = Instantiate(CharacteristicsResultListPrefab);
                CHAR.transform.SetParent(CharacteristicsCloneParent);               
                CHAR.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Characteristics " + i;        
                CHAR.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = CharacteristicsRefList[i];  
                PropertyCheckText(PropertyRefList[i]);
                CHAR.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = propertyNames;

                CHAR.transform.localScale = new Vector3(1, 1, 1);
                CHAR.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);


                CHAR.GetComponent<Button>().onClick.AddListener(SelectedCharacteristics);
                CharacteristicsListDestory.Add(CHAR);

               /* This is For Normal Ble Property Read Function
                  Button readButton = CHAR.transform.GetChild(2).GetComponent<Button>();
                  Button writeButton = CHAR.transform.GetChild(3).GetComponent<Button>();
                  Button notifyButton = CHAR.transform.GetChild(4).GetComponent<Button>();
                  Button indicateButton = CHAR.transform.GetChild(5).GetComponent<Button>();

                  readButton.onClick.AddListener();

                  PropertyCheckButton(Int32.Parse(PropertyRefList[i]), readButton,writeButton,notifyButton,indicateButton);
                  CHAR.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = PropertyRefList[i];
               */

            }
        }

    }

    void CharacteristicsResultDestory()
    {
        serviceCharUUID.text = "";

        for (int i = 0; i < CharacteristicsListDestory.Count; i++)
        {
            Destroy(CharacteristicsListDestory[i]);
            ServiceRefList.Clear();
            CharacteristicsRefList.Clear();
            PropertyRefList.Clear();
        }

        CharacteristicsListDestory = new List<GameObject>();
    }

    void CharacteristicsGameObjDestory()
    {
        for (int i = 0; i < CharacteristicsListDestory.Count; i++)
        {
            Destroy(CharacteristicsListDestory[i]);
        }

        CharacteristicsListDestory = new List<GameObject>();
    }





    void SelectedCharacteristics()
    {

        for (int i = 0; i < CharacteristicsListDestory.Count; i++)
        {
            CharacteristicsListDestory[i].GetComponent<Image>().color = Color.white;
        }

        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = characteristicsSelectColour;


        selectedCharacteristicsUUID = EventSystem.current.currentSelectedGameObject
            .transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        // Using EventSystem by Clicked Device's Characteristics Text Get passed to a string


        Debug.Log("SelectedCharacteristics() : " + selectedCharacteristicsUUID);


    }


    /* This is For Normal Ble Property Read Function
      void PropertyCheckButton(int property, Button ReadButton, Button WriteButton, Button NotifyButton, Button IndicateButton) 
     {

         if (property - 2 == 0 || (property - 2) % 4 == 0)
         {       
            //Read
            ReadButton.interactable = true;
         }
         if (property % 4 == 0 || (property - 2) % 4 == 0)
         {
             //WriteWithoutResponse
             WriteButton.interactable = true;
         }
         if ((8 <= property && property <= 14) || (24 <= property && property <= 30)
                 || (40 <= property && property <= 46) || (56 <= property && property <= 62))
         {  
             //WriteWithResponse
             WriteButton.interactable = true;
         }
         if ((16 <= property && property <= 30) || (48 <= property && property <= 62))
         {
             //Notify
             NotifyButton.interactable = true;
         }
         if (32 <= property && property <= 62)
         {
             //Indicate
             IndicateButton.interactable = true;
         }

     }*/

    void PropertyCheckText(string property) 
    {
        switch (property)
        {
            case "2": propertyNames = "Read"; break;
            case "4": propertyNames = "WriteWOR"; break; 
            case "6": propertyNames = "Read | WriteWOR"; break;
            case "8": propertyNames = "WriteWR"; break;
            case "10": propertyNames = "Read | WriteWR"; break;
            case "12": propertyNames = "WriteWOR | WriteWR"; break;
            case "14": propertyNames = "Read | WriteWOR | WriteWR"; break;

            case "16":propertyNames = "Notify"; isNotify = true; break;

            case "18": propertyNames = "Read | Notify"; break;
            case "20": propertyNames = "WriteWOR | Notify"; break;
            case "22": propertyNames = "Read | WriteWOR | Notify"; break;
            case "24": propertyNames = "WriteWR | Notify"; break;
            case "26": propertyNames = "Read | WriteWR | Notify"; break;
            case "28": propertyNames = "WriteWOR | WriteWR | Notify"; break;
            case "30": propertyNames = "Read | WriteWOR | WriteWR | Notify"; break;

            case "32": propertyNames = "Indicate"; break;
            case "34": propertyNames = "Read | Indicate"; break;
            case "36": propertyNames = "WriteWOR | Indicate"; break;
            case "38": propertyNames = "Read | WriteWOR | Indicate"; break;
            case "40": propertyNames = "WriteWR | Indicate"; break;

            case "42": propertyNames = "Read | WriteWR | Indicate"; break;
            case "44": propertyNames = "WriteWOR | WriteWR | Indicate"; break;
            case "46": propertyNames = "Read | WriteWOR | WriteWR | Indicate"; break;
            case "48": propertyNames = "Notify | Indicate"; break;
            case "50": propertyNames = "Read | Notify | Indicate"; break;
            case "52": propertyNames = "WriteWOR | Notify | Indicate"; break;
            case "54": propertyNames = "Read | WriteWOR | Notify | Indicate"; break;
            case "56": propertyNames = "WriteWR | Notify | Indicate"; break;
            case "58": propertyNames = "Read | WriteWR | Notify | Indicate"; break;
            case "60": propertyNames = "WriteWOR | WriteWR | Notify | Indicate"; break;
            case "62": propertyNames = "Read | WriteWOR | WriteWR | Notify | Indicate"; break;

        }

    
    
    }


    void StartNotification() 
    {

        JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "startNotification", selectedServiceUUID, selectedCharacteristicsUUID);

    }

    void StopNotification() 
    {
        JavaClassUtil.CallStaticWithoutResponse("CallCentralMethod", "stopNotification", selectedServiceUUID, selectedCharacteristicsUUID);

    }



    void StartStopNotify() 
    {

        if(isNotify)
        {
          StartNotification();
          StartStopNotifyBtnTxt.text = "Stop Notify"; 

          isNotify = false;
        }
        else
        {      
          StopNotification();
          StartStopNotifyBtnTxt.text = "Start Notify";
          TempValue.text = null;

          isNotify = true; 
        }
          

    }









}