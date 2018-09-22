using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour {

    public float speed = 2f;
    public float turnSpeed = 2f;
    public float sidewaysGrip = 10f;
    public float turnDamp = 1f;
    public Vector3 desiredForce;

    public Vector3 sidewaysControlForce;

    public float currentToque = 0;

    [Header("Ground")]
    public bool grounded;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundCheckRadius;

    public float maxSpeed;
    public float currentSpeed;

    private float horizontal;
    private Rigidbody rb;

    public AudioSource engineAudioSource;
    public AudioSource skidAudioSource;

    public ParticleSystem[] smokes;

    public float sidewaysDeslocation;

    public float vehicleAngle;

    public float smokeMultiplier;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        horizontal = Input.GetAxisRaw("Horizontal");

        // Quaternion desiredRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + (turnSpeed * 100f * GetHorizontalAbsolute() * Time.deltaTime), transform.rotation.eulerAngles.z);
        // transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnDamp);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, whatIsGround);
        grounded = (hitColliders.Length > 0);

        float driftValue = Vector3.Dot(rb.velocity, transform.forward);
        vehicleAngle = Mathf.Acos(driftValue);
        Debug.Log(vehicleAngle);

        SidewaysDeslocation();
        currentSpeed = rb.velocity.magnitude;

        // transform.Rotate(Vector3.up * turnSpeed * 100f * GetHorizontalAbsolute() * Time.deltaTime);

        if (currentSpeed < maxSpeed && grounded)
        {
            desiredForce = Vector3.forward * speed * 500f;
            /**if (Mathf.Abs(transform.InverseTransformDirection(rb.velocity).x) > 5f)
            {
                // desiredForce.z = 0f;
            }**/
            // desiredForce.z = desiredForce.z / Mathf.Abs(transform.InverseTransformDirection(rb.velocity).x);
            rb.AddRelativeForce(desiredForce * Time.deltaTime, ForceMode.Acceleration);
        }

        float sidewaysDeslocation = transform.InverseTransformDirection(rb.velocity).x;
        sidewaysControlForce = transform.right * -sidewaysDeslocation * sidewaysGrip;
        if (grounded)
        {
            rb.AddForce(sidewaysControlForce);
        }

        if (!grounded)
        {
            rb.AddForce(Vector3.down * 800f);
        }
        

        TurnControl();
        EngineSound();
        Smoke();
        
    }

    private void Smoke()
    {

        foreach (ParticleSystem smoke in smokes)
        {
            var emission = smoke.emission;
            if (sidewaysDeslocation > 30f && grounded)
            {
                emission.rateOverTime = sidewaysDeslocation * smokeMultiplier;
            } else
            {
                emission.rateOverTime = 0f;
            }
        }   
    }

    private void SidewaysDeslocation ()
    {
        sidewaysDeslocation = Mathf.Abs(transform.InverseTransformDirection(rb.velocity).x);
    }

    private void EngineSound()
    {
        float speedNormalized = Mathf.Clamp((currentSpeed / 50), 0, 1);
        

        float sidewaysNormalized = Mathf.Clamp(Mathf.Abs(transform.InverseTransformDirection(rb.velocity).x) / 200f, 0f, .3f);

        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, 1f + speedNormalized + sidewaysNormalized, .5f);

        // skidAudioSource.pitch = 1f * sidewaysNormalized;

        if (Mathf.Abs(transform.InverseTransformDirection(rb.velocity).x) > 10f)
        {
            skidAudioSource.pitch = Mathf.Lerp(skidAudioSource.pitch, .9f, .9f);
            skidAudioSource.volume = Mathf.Lerp(skidAudioSource.volume, 1f * sidewaysNormalized, .1f);
            // sidewaysNormalized
        } else
        {
            skidAudioSource.pitch = Mathf.Lerp(skidAudioSource.pitch, 0f, .9f);
            skidAudioSource.volume = 0f;
        }
    }

    private float GetInput()
    {
        return GetHorizontalAbsolute();
    }

    private void TurnControl()
    {
        if (grounded)
        {
            transform.Rotate(Vector3.up * (turnSpeed * 50f) * GetInput() * Time.deltaTime);

            // rb.AddTorque();
            if (GetHorizontalAbsolute() != 0)
            {
                // rb.angularVelocity = Vector3.up * turnSpeed * GetHorizontalAbsolute();
            }
            if (GetHorizontalAbsolute() == 0)
            {
                // rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, .1f);
            }
        }

    }

    private float GetHorizontalAbsolute()
    {
        if (Input.touchCount > 0)
        {
            float position = Input.GetTouch(0).position.x;
            if (position > Screen.width / 2)
            {
                return 1;
            }

            if (position < Screen.width / 2)
            {
                return -1;
            }
        }

        if (horizontal < 0)
        {
            return -1;
        } else if(horizontal > 0) {
            return 1;
        }

        return 0;
    }
}
