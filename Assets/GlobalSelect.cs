using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using HoloToolkit.Unity.InputModule;

public class GlobalSelect : MonoBehaviour, IInputClickHandler
{

    [Tooltip("Fire a global if focused game object is not one of these")]
    public LayerMask ignoreLayers = 0/*nothing*/;
    [Tooltip("The event fired on a Holo tap.")]
    public UnityEvent Tap;

    private void Start()
    {
        Debug.Log(ignoreLayers.value);
    }

    void OnEnable()
    {
        InputManager.Instance.AddGlobalListener(gameObject);
    }

    void OnDisable()
    {
        InputManager.Instance.RemoveGlobalListener(gameObject);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        // get the currently focused game object (if there is one)
        GameObject focusedObject = (InputManager.Instance.OverrideFocusedObject == null) ? GazeManager.Instance.HitObject : InputManager.Instance.OverrideFocusedObject;

        // a tap is if there is not game object focused, or if that game object is in the layer mask
        if (focusedObject == null || (ignoreLayers.value != 0 && focusedObject.layer != 0 && ((1 << focusedObject.layer) & ignoreLayers.value) != 0))
        {
            Tap.Invoke();
        }
    }


}