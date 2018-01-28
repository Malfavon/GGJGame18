using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class bomb : NetworkBehaviour {

    private int firstFollow;
    public PlayerManager infectedPlayer; //saber que jugador tiene la bomba
    public bool infectingPlayer = false;
    public bool isChasing = false;

    public float boomSpeed = 6.0f;
    public float chaseSpeed = 10.0f;
    public float maxChaseDist = 5f;

    public bool noActivePlayers = false;

    public GameObject target;

	// Use this for initialization
	void Start () {

       
        //firstPick();
		
	}

    public void Setup()
    {
            firstFollow = Random.Range(0, GGJGameManager.m_Tanks.Count);
            Debug.Log("Setting up the bomb. Player #" + firstFollow + " of " + GGJGameManager.m_Tanks.Count + " total");
        //infectedPlayer = GGJGameManager.m_Tanks[firstFollow];
        //infectedPlayer.m_Movement.isInfected = true;
        //infectingPlayer = true;
        target = FindClosestPlayer();
        isChasing = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (noActivePlayers) return;
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
                infectedPlayer = null;
                infectingPlayer = false;
                isChasing = true;
                
            }
            
        } else if(isChasing )
        {
            Vector3 diff = target.transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance > maxChaseDist)
            {
                target = FindClosestPlayer();
                if (!target)
                {
                    noActivePlayers = true;
                    Debug.Log("No more active players to chase");
                }
                Debug.Log("Chasing " + target.name);
            } else if(curDistance < 1 )
            {
                Debug.Log("BOOM");
                infectingPlayer = true;
                infectedPlayer = GGJGameManager.m_Tanks[target.GetComponent<movement>().m_PlayerNumber];
                target.GetComponent<movement>().hitBomb(transform.gameObject);
                return;
            }
            float step = chaseSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        }

    }

    public GameObject FindClosestPlayer()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            if (!go.activeSelf) continue;
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
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
