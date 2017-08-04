using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCamera : MonoBehaviour {

    private GameObject mainCamera;

	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}

    // Update is called once per frame
    void Update()
    {

        Transform cameraTransform = mainCamera.transform;
        Vector3 eulerAngles = mainCamera.transform.rotation.eulerAngles;
        eulerAngles = new Vector3(-270, eulerAngles.y, -90);
        transform.rotation = Quaternion.Euler(eulerAngles);

    }
}
