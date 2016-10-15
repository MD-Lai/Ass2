using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float zoomSteps = 1;
    public float rotateSpeed = 50;
    private GameObject mainball;
    private Vector3 offSet;
	// Use this for initialization
	void Start () {
        mainball = GameObject.Find("Sphere");
        offSet = mainball.transform.localPosition - this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");


        float currFOV = Camera.main.fieldOfView;

        if (scrollAxis < 0) {
            currFOV = (currFOV + zoomSteps) > 175 ? 175 : currFOV + zoomSteps;
        }
        if(scrollAxis > 0) {
            currFOV = (currFOV - zoomSteps) < 5 ? 5 : currFOV - zoomSteps;
        }

        Camera.main.fieldOfView = currFOV;

        this.transform.RotateAround(mainball.transform.localPosition, new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0), rotateSpeed * Time.deltaTime);
        Vector3 rotatedAngle = this.transform.localEulerAngles;
        rotatedAngle.z = 0;
        this.transform.localEulerAngles = rotatedAngle;

	}
}
