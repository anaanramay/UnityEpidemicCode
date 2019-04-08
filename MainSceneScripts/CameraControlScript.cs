using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour {

    // State of this camera, camera becomes inactive if false
    bool camActive = true;

    // Camera movement speed
    public float speed = 2f;
    public float zoomSpeed = 120f;

    // Index macros for key down array
    const int W_KEY  = 0;
    const int A_KEY  = 1;
    const int S_KEY  = 2;
    const int D_KEY  = 3;
    const int LSHIFT = 4;

    // Camera component
    Camera cam;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start() {
        cam = GetComponent<Camera>();
        cam.orthographicSize += 100;

	}
	
	// Update is called once per frame
	void Update() {
        if (camActive) {
            
            // Get the keys that are down
            int[] keys = GetKeysDown();

            // Set the direction
            Vector3 direction = new Vector3();
            direction += new Vector3(0, keys[W_KEY] * speed, 0);
            direction += new Vector3(0, keys[S_KEY] * -speed, 0);
            direction += new Vector3(keys[A_KEY] * -speed, 0, 0);
            direction += new Vector3(keys[D_KEY] * speed, 0, 0);

            // Triple the speed if lshift is down
            direction *= (keys[LSHIFT] == 1) ? 3 : 1;

            // Scale the speed to the zoom
            direction *= cam.orthographicSize / 200;

            transform.position += direction;

            // Set the zoom
            float zoom = 0;
            zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

            if (cam.orthographicSize + zoom <= 2000 && cam.orthographicSize + zoom >= 20) {
                cam.orthographicSize += zoom;
            }
        }
	}

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Returns an array of ints representing which keys are down, 0 = up, 1 = down
    // Indices: 0 = w, 1 = a, 2 = s, 3 = d, 4 = lshift
    int[] GetKeysDown() {
        int[] keys = {
            Input.GetKey("w") ? 1 : 0,
            Input.GetKey("a") ? 1 : 0,
            Input.GetKey("s") ? 1 : 0,
            Input.GetKey("d") ? 1 : 0,
            Input.GetKey("left shift") ? 1 : 0
        };

        return keys;
    }

    // Sets the activeness of the scene
    void ToggleCameraActive() {
        camActive = !camActive;
    }
}
