using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoxResizer : MonoBehaviour {

    public Transform boxTransform;

	// Use this for initialization
	void Start () {
        float relativeZ = boxTransform.position.z - gameObject.transform.position.z;
        float frustumHeight = 2.0f * relativeZ * Mathf.Tan(GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * GetComponent<Camera>().aspect;
        Vector3 scale = boxTransform.localScale;
        scale.Set(frustumWidth, frustumHeight, 1.0f);
        boxTransform.localScale = scale;


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
