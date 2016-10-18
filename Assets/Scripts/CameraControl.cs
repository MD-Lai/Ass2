using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float zoomSteps = 1;
    public float rotateSpeed = 50;
    public float mouseSensitivity = 2;
    
    private Transform mainball;
    
    private Vector3 offSet;
    private Vector3 mainballPrev;
    private Vector3 startPos;
	// Use this for initialization
	void Start () {
        mainball = GameObject.Find("MainBall").transform;
       
        Camera.main.transform.LookAt(mainball.position);
        startPos = this.transform.localPosition;
        
    }
	
	// Update is called once per frame
	void Update () {

    }

    void LateUpdate() {


        if (Input.GetKeyDown(KeyCode.C)) {
            this.transform.localPosition = new Vector3(mainball.localPosition.x, this.transform.localPosition.y, mainball.localPosition.z);
        }
        
        this.transform.localPosition += mainball.transform.localPosition - mainballPrev;

        Vector3 constantH = new Vector3(this.transform.localPosition.x, startPos.y, this.transform.localPosition.z);

        this.transform.localPosition = constantH;

        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        float currFOV = Camera.main.fieldOfView;

        if (scrollAxis < 0) {
            currFOV = (currFOV + zoomSteps) > 150 ? 150 : currFOV + zoomSteps;
        }
        if(scrollAxis > 0) {
            currFOV = (currFOV - zoomSteps) < 5 ? 5 : currFOV - zoomSteps;
        }

        

        Camera.main.fieldOfView = currFOV;

        this.transform.Translate(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * mouseSensitivity * currFOV / 6);
        //this.transform.RotateAround(mainball.position, new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0), rotateSpeed * mouseSensitivity * Time.deltaTime);

        //Vector3 rotatedAngle = this.transform.localEulerAngles;
        //rotatedAngle.z = 0;
        //this.transform.localEulerAngles = rotatedAngle;

        mainballPrev = mainball.transform.localPosition;
    }
}
