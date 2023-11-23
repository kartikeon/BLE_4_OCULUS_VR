package com.kar98.androidunityble;

import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.Arrays;

public class UnitySender {
    public void sendMessage(String... params) {
        Log.d("UnitySender", Arrays.toString(params));
        // change [a,b,c] to "a,b,c"
        String param = String.join(",", params);

        // Send to Unity

        // UnitySendMessage("GameObject name","Function name","Argument")
      UnityPlayer.UnitySendMessage("BleReceiver", "PluginMessage", param);
      //UnityPlayer.UnitySendMessage("BleManager", "PluginMessage", param);

    }


    public void sendMessagePro(String... params) {
        Log.d("UnitySender", Arrays.toString(params));
        // change [a,b,c] to "a,b,c"
        String param = String.join(",", params);

        // Send to Unity

        // UnitySendMessage("GameObject name","Function name","Argument")
        UnityPlayer.UnitySendMessage("BleCoreManager", "PluginMessage", param);
        //UnityPlayer.UnitySendMessage("BleManager", "PluginMessage", param);

    }
}

