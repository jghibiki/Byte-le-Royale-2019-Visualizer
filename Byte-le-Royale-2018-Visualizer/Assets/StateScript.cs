using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateScript : MonoBehaviour {
    List<GameObject> ShipList;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject x in ShipList)
        {
            Rigidbody y = x.GetComponent<Rigidbody>();
            y.MovePosition(new Vector3(y.position.x, y.position.y, y.position.z + 10));
        }
	}
}
