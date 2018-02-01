using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class BombColliderScript : NetworkBehaviour {

    public bomb realBomb;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isServer) return;
        transform.position = realBomb.transform.position;
	}

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("BOMBC Collide " + collision.gameObject.name + " " + (isServer ? "isServer" : "Not Server"));
    }
}
