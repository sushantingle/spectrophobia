#define UNITY_DEBUG
#define DEBUG_BUILD

using UnityEngine;
using System.Collections;

public class CustomDebug {

    public static void Log(string text) {
        #if UNITY_DEBUG
            Debug.Log(text);
        #endif
    }

    public static void LogError(string text) {
        #if UNITY_DEBUG
             Debug.LogError(text);
        #endif
    }

    public static bool isDebugBuild() {
        #if DEBUG_BUILD
            return true;
        #elif RELEASE_BUILD
            return false;
        #endif
    }
}
