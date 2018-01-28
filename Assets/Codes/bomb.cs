using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour {

    private int firstFollow;
    public static PlayerManager infectedPlayer; //saber que jugador tiene la bomba

	// Use this for initialization
	void Start () {

        firstFollow = Random.Range(0, GGJGameManager.m_Tanks.Count);
        infectedPlayer = GGJGameManager.m_Tanks[firstFollow];
        //firstPick();
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = new Vector3 (movement.charPosition.x - 1, movement.charPosition.y, movement.charPosition.z);

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
