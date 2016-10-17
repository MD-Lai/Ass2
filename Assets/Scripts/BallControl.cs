using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {
    public float setSpeed = 1;
    private Vector3 origPos;
    public BoardGenerator board;
    private Rigidbody body;
    // Use this for initialization
    void Start () {
        origPos = this.transform.localPosition;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

        
        Vector3 appliedVel = Vector3.zero;
        //Vector3 ballXY = new Vector3(this.transform.localPosition.x, 0, this.transform.localPosition.z);
        //Vector3 camXY = new Vector3(Camera.main.transform.localPosition.x, 0, Camera.main.transform.localPosition.z);
        //Vector3 cameraRel = (ballXY - camXY).normalized;

        // all motions relative to camera
        // forward
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            appliedVel.z += setSpeed; 
            /*
            appliedVel.z += cameraRel.z * setSpeed;
            appliedVel.x += cameraRel.x * setSpeed;
            */
        }

        // back
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            appliedVel.z -= setSpeed;
            /*
            appliedVel.z -= cameraRel.z * setSpeed;
            appliedVel.x -= cameraRel.x * setSpeed;
            */
        }

        // right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            appliedVel.x += setSpeed;
            /*
            appliedVel.x += cameraRel.z * setSpeed;
            appliedVel.z -= cameraRel.x * setSpeed;
            */
        }

        // left
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            appliedVel.x -= setSpeed;
            /*
            appliedVel.x -= cameraRel.z * setSpeed;
            appliedVel.z += cameraRel.x * setSpeed;
            */
        }

        // jump
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 vertVel = body.velocity;
            vertVel.y = 9.81f;
            body.velocity = vertVel;
        }
        
        body.velocity += appliedVel * Time.deltaTime;
        //Debug.Log(body.velocity.magnitude);

        if (Input.GetKeyDown(KeyCode.R)) {
            respawn();
        }
        
    }

    public void respawn() {
        this.transform.localPosition = origPos;
        body.velocity = Vector3.zero;
    }
}
