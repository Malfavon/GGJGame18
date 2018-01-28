using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class bomb : NetworkBehaviour {

    private int firstFollow;
    public PlayerManager infectedPlayer; //saber que jugador tiene la bomba
    public bool infectingPlayer = false;

    public float boomSpeed = 10.0f;

	// Use this for initialization
	void Start () {

       
        //firstPick();
		
	}

    public void Setup()
    {
            firstFollow = Random.Range(0, GGJGameManager.m_Tanks.Count);
            Debug.Log("Setting up the bomb. Player #" + firstFollow + " of " + GGJGameManager.m_Tanks.Count + " total");
            infectedPlayer = GGJGameManager.m_Tanks[firstFollow];
            infectedPlayer.m_Movement.isInfected = true;
            infectingPlayer = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (infectingPlayer)
        {
            transform.position =  new Vector3(infectedPlayer.m_Instance.transform.position.x, infectedPlayer.m_Instance.transform.position.y+1.2f, infectedPlayer.m_Instance.transform.position.z);
            
            infectedPlayer.m_Movement.myEnergy.value += Time.deltaTime * boomSpeed;
            if (infectedPlayer.m_Movement.myEnergy.value >= 95.0f)
            {
                Debug.Log("Player " + infectedPlayer.m_Movement.m_PlayerNumber + " GO BOOM");
                GGJGameManager.s_Instance.RemoveTank(infectedPlayer.m_Movement.transform.gameObject);
                infectedPlayer.isDead = true;
                infectedPlayer.m_Movement.GoBOOM();
                
            }
            
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
