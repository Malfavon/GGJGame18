using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class bomb : NetworkBehaviour {

    private int firstFollow;
    public PlayerManager infectedPlayer; //saber que jugador tiene la bomba
    private bool infectingPlayer = false;

	// Use this for initialization
	void Start () {

       
        //firstPick();
		
	}

    public void Setup()
    {
            firstFollow = Random.Range(0, GGJGameManager.m_Tanks.Count);
            Debug.Log("Setting up the bomb. Player #" + firstFollow + " of " + GGJGameManager.m_Tanks.Count + " total");
            infectedPlayer = GGJGameManager.m_Tanks[firstFollow];
            infectingPlayer = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (infectingPlayer)
        {
            transform.position = infectedPlayer.m_Instance.transform.position;
        }

    }

    /*void firstPick()
    {
        switch (firstFollow)
        {
            case 1:
                print("Siguiendo al jugador 1");
                break;
            case 2:
                print("Siguiendo al jugador 2");
                break;
            case 3:
                print("Siguiendo al jugador 3");
                break;
            case 4:
                print("Siguiendo al jugador 4");
                break;
        }
    }*/
}
