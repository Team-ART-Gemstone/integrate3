using System;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiffMatchPatch;
using HoloToolkit.Unity.Receivers;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

#if UNITY_UWP
using SpellLibrary;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Security;
#endif

public class temp : InteractionReceiver
{

    protected override void InputDown(GameObject obj, InputEventData eventData)
    {
        Debug.Log("This");
        switch (obj.name)
        {
            case "Button":
                Debug.Log("This Worked");
                break;
            case "third":
                break;
            case "fourth":
                break;
            case "fifth":
                break;
            case "sixth":
                break;
            case "FreeMode":
                break;
        }
    }

}