using UnityEngine;
using System.Collections;

public class PinManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R)) {
            Destroy(this.gameObject);
        }
	}
}
