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
    private float energy; //variable para almacenar energia acumulada del personaje
    public Slider myEnergy; //slider que mostrara cuanta energia has acumulado
    public static GameObject thisObject;

    public int m_PlayerNumber = 1;                // Used to identify which player this object belongs to
    public int m_LocalID = 1;

    private string m_MovementAxis;              // The name of the input axis for moving forward and back.
    private string m_TurnAxis;                  // The name of the input axis for turning.
    private float m_MovementInput;              // The current value of the movement input.
    private float m_TurnInput;                  // The current value of the turn input.

    // Use this for initialization
    void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
        //myEnergy.value = 48;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        //charPosition = transform.position;  //toma la posicion actual del personaje
		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        


		m_Rigidbody.velocity = movement * speed;

		LimitVelocity ();
    }

    // Update is called once per frame
    void Update () {

        charPosition = thisObject.transform.position;
        if (mine)
        {
            myEnergy.value += Time.time;
        }
        if (!isLocalPlayer)
        {
            return;
        }
       
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

    private void OnCollisionEnter(Collision collision)
    {
        if (mine == false)
        {
            thisObject=this.gameObject;
            
            mine = !mine;
        }
    }
}
