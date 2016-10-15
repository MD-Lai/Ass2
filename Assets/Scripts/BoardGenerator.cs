using UnityEngine;
using System.Collections;

public class BoardGenerator : MonoBehaviour {

    private GameObject pinPrefab;
    

    // Use this for initialization
    void Start() {
        pinPrefab = (GameObject)Resources.Load("prefabs/Pin", typeof(GameObject));

        //if (pins != null) {
        for (int i = 0; i < 10; i++) {
            GameObject pins = GameObject.Instantiate<GameObject>(pinPrefab);
            pins.transform.localPosition = new Vector3((Random.value - 0.5f) * 20, 0.75f, (Random.value - 0.5f) * 20);
        }
        //}
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R)) {
            for (int i = 0; i < 10; i++) {
                GameObject pins = GameObject.Instantiate<GameObject>(pinPrefab);
                pins.transform.localPosition = new Vector3((Random.value - 0.5f) * 20, 0.75f, (Random.value - 0.5f) * 20);
            }
        }
    }
}
