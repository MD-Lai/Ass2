using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    public float zoomSteps = 1;
    public float rotateSpeed = 50;
    public float mouseSensitivity = 2;
    public float pinchSpeed = 0.5f;

    private Transform mainball;
    public BallControl bally;
    public BoardGenerator board;
    
    private Vector3 offSet;
    private Vector3 mainballPrev;
    private Vector3 startPos;
    private Vector3 prevTouch;

    
	// Use this for initialization
	void Start () {
        mainball = GameObject.Find("MainBall").transform;
       
        Camera.main.transform.LookAt(mainball.position);
        startPos = this.transform.localPosition;
        
    }
	
	// Update is called once per frame
	

    void Update() {

        //constrainMovement();


        if (Input.GetKeyDown(KeyCode.C)) {
            this.transform.localPosition = new Vector3(mainball.localPosition.x, this.transform.localPosition.y, mainball.localPosition.z);
        }

        if (Settings.useTablet && SystemInfo.supportsAccelerometer) {
            controlT();
        }
        else {
            controlKBM();
        }
        
        constrainMovement();

        mainballPrev = mainball.transform.localPosition;
    }

    void constrainMovement() {
        this.transform.localPosition += mainball.transform.localPosition - mainballPrev;

        Vector3 constantH = new Vector3(this.transform.localPosition.x, startPos.y, this.transform.localPosition.z);

        if (this.transform.localPosition.x < board.getBoardMinX()) {
            constantH.x = board.getBoardMinX();
        }

        if (this.transform.localPosition.x > board.getBoardMaxX()) {
            constantH.x = board.getBoardMaxX();
        }
        if (this.transform.localPosition.z < board.getBoardMaxY()) {
            constantH.z = board.getBoardMaxY();
        }

        if (this.transform.localPosition.z > board.getBoardMinY()) {
            constantH.z = board.getBoardMinY();
        }
        this.transform.localPosition = constantH;
        
        
    }

    void controlKBM() {
        

        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        float currFOV = Camera.main.fieldOfView;

        if (scrollAxis > 0) {
            currFOV = Mathf.Min(150,(currFOV + zoomSteps));
        }
        if (scrollAxis < 0) {
            currFOV = Mathf.Max(5, (currFOV - zoomSteps));
        }

        this.transform.Translate(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * mouseSensitivity * currFOV / 16);


        Camera.main.fieldOfView = currFOV;
    }

    void controlT() {
        if (Input.touchCount == 1) {
            // surface up is accel y
            // surface right is accel x
            Touch touch = Input.GetTouch(0);
            Camera camera = GetComponent<Camera>();
            if(touch.phase == TouchPhase.Began) {
                
                prevTouch.x = touch.position.x;
                prevTouch.y = touch.position.y;
                prevTouch.z = startPos.y;
            }

            Vector3 currTouch;
            currTouch.x = touch.position.x;
            currTouch.y = touch.position.y;
            currTouch.z = startPos.y;

            Vector3 touchMove = camera.ScreenToWorldPoint(currTouch) - camera.ScreenToWorldPoint(prevTouch);

            Vector3 camPos = this.transform.localPosition;

            camPos.x -= touchMove.x;

            camPos.z -= touchMove.z;

            this.transform.localPosition = camPos;

            //this.transform.Translate(-touchMove.x, touchMove.y, 0);

            prevTouch.x = currTouch.x;
            prevTouch.y = currTouch.y;
            prevTouch.z = currTouch.z;
            
        }

        if(Input.touchCount == 2) {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0Prev - touch1Prev).magnitude;
            float currrTouchDeltaMag = (touch0.position - touch1.position).magnitude;

            float touchMagDiff = prevTouchDeltaMag - currrTouchDeltaMag;


            Camera.main.fieldOfView += touchMagDiff * pinchSpeed;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 5, 150);



        }
    }
}
