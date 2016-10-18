using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour {
    public float setSpeed = 1;
    public Shader shader;
    public PointLight pointLight;

    private Vector3 origPos;
    public BoardGenerator board;
    private Rigidbody body;

    private int score = 0;
    

    // Use this for initialization
    void Start () {
        origPos = this.transform.localPosition;
        body = GetComponent<Rigidbody>();
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
        renderer.material.shader = shader;
    }

    // Update is called once per frame
    void Update() {

        // Get renderer component (in order to pass params to shader)
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // Pass updated light positions to shader
        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
        renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());


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
            vertVel.y = 5.0f;
            body.velocity = vertVel;
        }
        
        body.velocity += appliedVel * Time.deltaTime;
        //Debug.Log(body.velocity.magnitude);

        if (Input.GetKeyDown(KeyCode.R) || this.transform.localPosition.y <= -25) {
            score--;
            respawn();
        }
        
    }

    public void respawn() {
        this.transform.localPosition = origPos;
        body.velocity = Vector3.zero;
        
    }

    public void addScore(int toAdd) {
        score += toAdd;
    }

    public int getScore() {
        return score;
    }
}
