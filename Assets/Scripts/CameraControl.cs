using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float zoomSteps = 1;
    public float rotateSpeed = 50;
    public float mouseSensitivity = 2;
    private GameObject mainball;
    private Vector3 offSet;
    private Vector3 mainballPrev;
	// Use this for initialization
	void Start () {
        mainball = GameObject.Find("Sphere");
        Camera.main.transform.LookAt(mainball.transform.position);
        offSet = this.transform.localPosition - mainball.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        this.transform.localPosition += mainball.transform.localPosition - mainballPrev;

        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        float currFOV = Camera.main.fieldOfView;

        if (scrollAxis < 0) {
            currFOV = (currFOV + zoomSteps) > 175 ? 175 : currFOV + zoomSteps;
        }
        if(scrollAxis > 0) {
            currFOV = (currFOV - zoomSteps) < 5 ? 5 : currFOV - zoomSteps;
        }

        Camera.main.fieldOfView = currFOV;

        this.transform.RotateAround(mainball.transform.position, new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0), rotateSpeed * mouseSensitivity * Time.deltaTime);

        if(this.transform.localPosition.y <=  0) {
            Vector3 manipulator = this.transform.localPosition;
            manipulator.y = 0;
            this.transform.localPosition = manipulator;
            Camera.main.transform.LookAt(mainball.transform.position);
        }

        Vector3 rotatedAngle = this.transform.localEulerAngles;
        rotatedAngle.z = 0;
        this.transform.localEulerAngles = rotatedAngle;

        mainballPrev = mainball.transform.localPosition;
    }
}
