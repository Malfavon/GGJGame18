using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class movement : NetworkBehaviour {

    public float speed;
	public float MaxSpeed;
    public Rigidbody m_Rigidbody;
    public static Vector3 charPosition;  //variable para enviar a otro codigo la posicion del personaje
    public static bool mine; //variable para ver si tu personaje tiene la bomba
    public bool isInfected = false;
    private float energy; //variable para almacenar energia acumulada del personaje
    public Slider myEnergy; //slider que mostrara cuanta energia has acumulado
    public float energyLvl = 0f;
    public static GameObject thisObject;

    public int m_PlayerNumber = 1;                // Used to identify which player this object belongs to
    public int m_LocalID = 1;

    private string m_MovementAxis;              // The name of the input axis for moving forward and back.
    private string m_TurnAxis;                  // The name of the input axis for turning.
    private float m_MovementInput;              // The current value of the movement input.
    private float m_TurnInput;                  // The current value of the turn input.
    public float m_Speed = 12f;
    public float m_SpeedInfected = 15f;

    private bool hasSpeedPower = false;
    private float pUpRemaining = 0f;
    private float pUpStrength = 0f;

    public float m_TurnSpeed = 180f;

    public bool isDead = false;

    private CameraControl myCam;

    // Use this for initialization
    void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
        //myEnergy.value = 48;
        energyLvl = 0;
        if (isLocalPlayer)
        {
            Debug.Log("Setting up local player ("+m_PlayerNumber+") energy bar and camera");
            myEnergy = GameObject.Find("Energy Slider").GetComponent<Slider>();
            myCam = transform.Find("CameraRig").GetComponent<CameraControl>();
            myCam.gameObject.SetActive(true);
            //myCam.gameObject.transform.parent = this.gameObject.transform;
            myCam.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            myCam.SetTarget(this.gameObject);

            
        } 
    }

    

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        float curSpeed = (isInfected) ? m_SpeedInfected : m_Speed;
        if (hasSpeedPower)
        {
            curSpeed += pUpStrength;
            pUpRemaining -= Time.deltaTime;
            if(pUpRemaining <= 0 )
            {
                hasSpeedPower = false;
                pUpRemaining = 0f;
            }
        }
        Vector3 movement = transform.forward * m_MovementInput * curSpeed * Time.deltaTime;
        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        float turn = m_TurnInput * m_TurnSpeed * Time.deltaTime;
        // Make this into a rotation in the y axis.
        Quaternion inputRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
       m_Rigidbody.MoveRotation(m_Rigidbody.rotation * inputRotation);

    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer )
        {
            return;
        }

        /** do this in bomb on server side
        if (isInfected && myEnergy )
        {
            myEnergy.value += Time.deltaTime * 5 ;
            if(myEnergy.value >= 100.0f)
            {

                GGJGameManager.s_Instance.RemoveTank(transform.gameObject);
            }
        }
        **/

        m_TurnInput = Input.GetAxis("Horizontal");
        m_MovementInput = Input.GetAxis("Vertical");


    }

	void LimitVelocity() {
		Vector2 xzVel = new Vector2(m_Rigidbody.velocity.x, m_Rigidbody.velocity.z);
		if (xzVel.magnitude > MaxSpeed) {
			xzVel = xzVel.normalized * MaxSpeed;
			m_Rigidbody.velocity = new Vector3(xzVel.x, m_Rigidbody.velocity.y, xzVel.y);
		}
	}

    public void SetDefaults()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        
    }

    [ClientRpc]
    public void RpcBombTicker( int pNum, float boomTick)
    {
        if (!isInfected) return;
        if (isDead) return;
        if (!isLocalPlayer) return;
        energyLvl += boomTick;
        if (isLocalPlayer) myEnergy.value = energyLvl;
        if ( energyLvl>= 95.0f)
        {
            Debug.Log("Player " + m_PlayerNumber + " GO BOOM");
            isInfected = false;
            GoBOOM();
            isDead = true;
        }
    }

    public void GoBOOM()
    {
        //move player camera to scene so it doesnt get deleted
        Transform cam = transform.Find("CameraRig");
        if (!cam) return;
        if (isLocalPlayer)
        {
            cam.gameObject.name = "DeadCamera";
            cam.parent = null;
            cam.position = new Vector3(0.0f, 10.0f, 0.0f);
            cam.Find("Main Camera").LookAt(transform);
        }
        this.gameObject.SetActive(false);
        //Destroy(this.gameObject, 1);
        GameObject.Find("Canvas").GetComponent<AudioSource>().Stop();
        GameObject.Find("Canvas").GetComponent<AudioSource>().clip = GGJGameManager.s_Instance.youLoseClip;
        GameObject.Find("Canvas").GetComponent<AudioSource>().loop = false;
        GameObject.Find("Canvas").GetComponent<AudioSource>().Play();
        GGJGameManager.s_Instance.m_EndRoundScreen.alpha = 1.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isInfected == false && collision.gameObject.name == "bomb")
        {
            Debug.Log("Collided");
            //hitBomb(collision.gameObject);
        }
        else if (collision.gameObject.tag == "PowerUp")
        {
            Debug.Log("POwer UP");
            PowerUpScript pUp = collision.gameObject.GetComponent<PowerUpScript>();
            hasSpeedPower = true;
            pUpRemaining = pUp.pDuration;
            pUpStrength = pUp.power;
            Destroy(collision.gameObject);
        }
        else if (isInfected && collision.gameObject.tag == "Player")
        {
            Debug.Log("Pass the bomb from player " + m_PlayerNumber + " to player " + collision.gameObject.GetComponent<movement>().m_PlayerNumber);
            GameObject.Find("bomb").GetComponent<bomb>().PassTheBomb(collision.gameObject.GetComponent<movement>());
            isInfected = false;
        }
        else
        {
            Debug.Log("Collide " + collision.gameObject.name);
        }
    }

    [ClientRpc]
    public void RpcHitBomb( int pNum, GameObject b)
    {
        /* this needs to be handled by server controlling bomb
        if (b.GetComponent<bomb>().infectingPlayer)
        {
            b.GetComponent<bomb>().infectedPlayer.m_Movement.isInfected = false;
            b.GetComponent<bomb>().infectedPlayer.m_Movement.myEnergy.value = 0;
        }
        */
        Debug.Log("RPC Hit Bomb");
        if (this.m_PlayerNumber != pNum) return;
        //b.GetComponent<bomb>().infectedPlayer = GGJGameManager.FindPlayer(m_PlayerNumber); //GGJGameManager.m_Tanks[m_PlayerNumber];
        //if (b.GetComponent<bomb>().infectedPlayer == null) return;
        isInfected = true;
        //b.GetComponent<bomb>().infectingPlayer = true;
    }

}
