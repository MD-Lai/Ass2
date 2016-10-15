using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {
    public float setSpeed = 1;
    private Vector3 origPos;
	// Use this for initialization
	void Start () {
        origPos = this.transform.localPosition;
	}

    // Update is called once per frame
    void Update() {
        Rigidbody body = GetComponent<Rigidbody>();
        Vector3 appliedVel = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            appliedVel.z += setSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            appliedVel.z -= setSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            appliedVel.x += setSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            appliedVel.x -= setSpeed;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 vertVel = body.velocity;
            vertVel.y = 9.81f;
            body.velocity = vertVel;
        }
        
        body.velocity += appliedVel * Time.deltaTime;
        //Debug.Log(body.velocity.magnitude);

        if (Input.GetKeyDown(KeyCode.R)) {
            this.transform.localPosition = origPos;
            body.velocity = Vector3.zero;
        }
    }

}
