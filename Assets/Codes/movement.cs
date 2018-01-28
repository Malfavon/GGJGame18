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
    public float m_Speed = 12f;

    public float m_TurnSpeed = 180f;

    private CameraControl myCam;

    // Use this for initialization
    void Start () {
        m_Rigidbody = GetComponent<Rigidbody>();
        //myEnergy.value = 48;
        if (isLocalPlayer)
        {
            Debug.Log("Setting up local player energy bar and camera");
            myEnergy = GameObject.Find("Energy Slider").GetComponent<Slider>();
            myCam = GameObject.Find("CameraRig").GetComponent<CameraControl>();
            myCam.gameObject.transform.parent = this.gameObject.transform;
            myCam.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            myCam.SetTarget(this.gameObject);

            
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        Vector3 movement = transform.forward * m_MovementInput * m_Speed * Time.deltaTime;
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
        if (!isLocalPlayer)
        {
            return;
        }
        if (mine && myEnergy )
        {
            myEnergy.value += Time.deltaTime * 10;
        }

        m_TurnInput = Input.GetAxis("Horizontal");
        m_MovementInput = Input.GetAxis("Vertical")*-1;


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
            Debug.Log("Collided");
            thisObject=this.gameObject;
            charPosition = thisObject.transform.position;
            mine = !mine;
        }
    }
}
