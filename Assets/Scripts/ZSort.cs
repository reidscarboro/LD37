using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZSort : MonoBehaviour {
	
	void Update () {
        float z;
        if (transform.localScale.x < 1.1f) {
            z = (transform.position.y) * 0.01f;
        } else {
            z = -0.25f;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, z);

    }
}
