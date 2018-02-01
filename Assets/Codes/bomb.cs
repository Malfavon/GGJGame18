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
            //firstFollow = Random.Range(0, GGJGameManager.m_Tanks.Count);
            //Debug.Log("Setting up the bomb. Player #" + firstFollow + " of " + GGJGameManager.m_Tanks.Count + " total");
        //infectedPlayer = GGJGameManager.m_Tanks[firstFollow];
        //infectedPlayer.m_Movement.isInfected = true;
        //infectingPlayer = true;
        target = FindClosestPlayer();
        isChasing = true;

        //ignore physics so bomb doesn't knock players off world - done via layers instead
        /*
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        Collider bCol = transform.Find("bombPhysics").GetComponent<Collider>();
        foreach (GameObject go in gos)
        {
            //Physics.IgnoreCollision(go.GetComponent<Collider>(), bCol);
        }
        */
    }
	
	// Update is called once per frame
	void Update () {
        if (!isServer) return;
        if (noActivePlayers) return;
        if (infectedPlayer == null)
        {
            infectingPlayer = false;
        }
        if (infectingPlayer && infectedPlayer != null)
        {
           
            transform.position =  new Vector3(infectedPlayer.m_Instance.transform.position.x, infectedPlayer.m_Instance.transform.position.y+1.2f, infectedPlayer.m_Instance.transform.position.z);
            
            infectedPlayer.m_Movement.myEnergy.value += Time.deltaTime * boomSpeed;
            if (infectedPlayer.m_Movement.myEnergy.value >= 95.0f)
            {
                Debug.Log("Player " + infectedPlayer.m_Movement.m_PlayerNumber + " GO BOOM");
                infectedPlayer.isDead = true;
                infectedPlayer.m_Movement.GoBOOM();
                GGJGameManager.s_Instance.RemoveTank(infectedPlayer.m_Movement.transform.gameObject);

                infectedPlayer = null;
                infectingPlayer = false;
                isChasing = true;
                
            }
            
        } else if(isChasing  )
        {
            float curDistance = 0f;
            if (target != null && target.activeSelf)
            {
                Vector3 diff = target.transform.position - transform.position;
                curDistance = diff.sqrMagnitude;
            }
            if (target == null || !target.activeSelf ||  curDistance > maxChaseDist)
            {
                target = FindClosestPlayer();
                if (!target)
                {
                    noActivePlayers = true;
                    Debug.Log("No more active players to chase");
                    return;
                }
            } else if(curDistance < 1 )
            {
                return;
                Debug.Log("BOOM");
                infectingPlayer = true;
                isChasing = false;
                infectedPlayer = GGJGameManager.FindPlayer(target.GetComponent<movement>().m_PlayerNumber); //GGJGameManager.m_Tanks[target.GetComponent<movement>().m_PlayerNumber];
                if (infectedPlayer == null)
                {
                    target = null;
                }
                else
                {
                    target.GetComponent<movement>().RpcHitBomb(transform.gameObject);
                    return;
                }
            }

            if (target != null)
            {
                float step = chaseSpeed * Time.deltaTime;
                Vector3 tPos = target.transform.position;
                tPos.y = 1.3f; //otherwise will chase players feet
                transform.position = Vector3.MoveTowards(transform.position, tPos, step);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log("BOMB Collide " + collision.gameObject.name + " " + (isServer?"isServer":"Not Server") );
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
