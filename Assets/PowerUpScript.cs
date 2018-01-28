using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUpScript : NetworkBehaviour {

    public string pType = "speed";
    public float pDuration = 5f;
    public float power = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
            Debug.Log("PUP Collide " + collision.gameObject.name);     
    }

}
