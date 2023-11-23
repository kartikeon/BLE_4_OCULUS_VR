using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using BlePlugin.Ble;
using BlePlugin.Data;

public class ConnTestScript : MonoBehaviour
{
    public TMP_Text devmac, msg;

    private FlagStream flagStream = FlagStream.GetInstance();

    List<string> scanlist = new List<string>();

    // BLE関連
    private BleScanner scanner = new BleScanner();
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }



    public void StartScan() 
    {

        scanner.StartScan();

        msg.text = "";
        msg.text = "StartScan()";

     

    }
    public void StopScan() 
    {

        scanner.StopScan();

        msg.text = "";
        msg.text = "StopScan()";

    }


}
