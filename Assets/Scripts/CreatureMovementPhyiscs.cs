using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//equations
//buoyancy equation
//https://www.google.com/search?q=buoyancy+equation+&client=opera&hs=5HT&sxsrf=APwXEdclBRvxH6j0wba1kTdUWh5HolAvtg%3A1683645446051&ei=BmRaZKDjAqP_7_UPwrCF-AY&ved=0ahUKEwig_vSFxOj-AhWj_7sIHUJYAW8Q4dUDCA8&uact=5&oq=buoyancy+equation+&gs_lcp=Cgxnd3Mtd2l6LXNlcnAQAzIKCAAQRxDWBBCwAzIKCAAQRxDWBBCwAzIKCAAQRxDWBBCwAzIKCAAQRxDWBBCwAzIKCAAQRxDWBBCwAzIKCAAQRxDWBBCwAzIKCAAQRxDWBBCwAzIKCAAQRxDWBBCwA0oECEEYAFC2B1i2B2CsCGgCcAB4AIABAIgBAJIBAJgBAKABAqABAcgBCMABAQ&sclient=gws-wiz-serp
//drag
//https://www.google.com/search?client=opera&hs=V0n&sxsrf=APwXEde07ckRaTAR47R3Wl84HZhxgON2Hg:1683645639770&q=FLUID+DRAG+equation&spell=1&sa=X&ved=2ahUKEwi0vaTixOj-AhVm_rsIHSC2ALQQBSgAegQICBAB&biw=1880&bih=931&dpr=1

public class CreatureMovementPhyiscs : MonoBehaviour
{

    #region Public Members
    [Header("Player Controls")]
    public float moveSpeed; // Player's speed. *important affect on the obstacle (how far the obstacle will be impacted) //default 50 
    public float turnSpeed; // just player turning. just for more control "Player feeling" // starting at 0.5 (default 2)

    [Header("Modified Player Physics")]
    public float buoyancyForce;// Control over the player "Floating" //starting at 0 
    public float mass;// Player's mass. *important affect on the obstacle && velocity (how far the obstacle will be impacted) //default 1 
    public float drag;// Player's mass. *important affect on the velocity. //default 15 
    public float resistance; //  Player's water resistance *important affect on the velocity // default 0
    public float elasticity; // Player "elasticity" how much the player will bounce back after collide with rock//default 0


    [Header("Player Conditionals And Checks")]
    public float raycastDistance = 1f;
    public bool isGrounded;
    public bool isWall;
    public bool isObstacle;
    public bool canMoveForward = true;
    private bool isInStrongStream = false;
    public float strongStreamForce = 20f; //  pushback force



    [Header("Player Cosmetics for Power Ups")]
    public Material[] playerMaterials;
    public SkinnedMeshRenderer skinMeshRenderer;
    #endregion

    #region Private Members
    private float gravity = 9.81f;
    private float moveInput;
    private float turnInput;
    public Vector3 velocity;
    public float maxBuoyancyForce = 0f;
    private float buoyancyStep = 0.4f;
    private Dictionary<PowerUpType, int> powerUpMaterialIndices;
    #endregion


    private void Start()
    {
        maxBuoyancyForce = buoyancyForce;

        // Map the power-up types to the corresponding material indices
        powerUpMaterialIndices = new Dictionary<PowerUpType, int>
        {
            { PowerUpType.FrontFin, 4 },
            { PowerUpType.Tail, 3 },
            { PowerUpType.Helment, 0 },
            { PowerUpType.BackFin, 2 }

        };
    }

    private void Update()
    {
        if (canMoveForward)
            moveInput = Input.GetAxis("Vertical") * turnSpeed;
        else
            moveInput = 0;

        turnInput = Input.GetAxis("Horizontal");

        AdjustBuoyancyForce();


    }

    private void FixedUpdate()
    {
        //  buoyancy force
        float depth = transform.position.y;
        float displacedVolume = Mathf.Abs(depth) * transform.localScale.x * transform.localScale.z;
        float displacedMass = displacedVolume * mass;
        float buoyancy = displacedMass * gravity * buoyancyForce;

        //  buoyancy force
        Vector3 buoyancyVector = new Vector3(0, buoyancy, 0);
        Vector3 gravityVector = new Vector3(0, -gravity * mass, 0);
        Vector3 netForce = buoyancyVector + gravityVector;
        Vector3 acceleration = netForce / mass;
        velocity += acceleration * Time.deltaTime;

        //  drag // not sure yet if its the right approach~
        float dragMagnitude = velocity.magnitude * drag;
        Vector3 dragVector = -velocity.normalized * dragMagnitude;
        velocity += dragVector * Time.deltaTime;

        // Calculate movement force
        Vector3 movementForce = transform.forward * moveInput * moveSpeed;

        // resistance force
        Vector3 resistanceForce = -velocity * resistance;
        if (isInStrongStream)
        {
            // Calculate resistance based on the player's velocity and the strength of the stream
            resistanceForce += -transform.forward * strongStreamForce;
        }

        
        velocity += (movementForce + resistanceForce) / mass * Time.deltaTime;

        // Check if the object is grounded
        if (isGrounded)
        {
            // Disable downward movement if grounded
            velocity.y = Mathf.Max(velocity.y, 0f);
        }
        if (isWall|| isObstacle)
        {
            velocity.z = Mathf.Max(velocity.z, 0f);
        }

        // position and rotation
        transform.position += velocity * Time.deltaTime;
        transform.rotation *= Quaternion.Euler(0f, turnInput * turnSpeed, 0f);

        DetectGround();
        DetectWall();
        DetectObject();
    }

    public void DetectGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }
    public void DetectWall()
    {
        RaycastHit hit;

        Vector3 castCenter = transform.position;
        float castRadius = 2f; // 
        float castDistance = 1.5f; // 

        if (Physics.SphereCast(castCenter, castRadius, transform.forward, out hit, castDistance))
        {
            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                isWall = true;
                canMoveForward = false;
            }
        }
        else
        {
            isWall = false;
            canMoveForward = true;
        }
    }
    public void DetectObject()
    {
        RaycastHit hit;

        Vector3 castCenter = transform.position;
        float castRadius = 2f; // 
        float castDistance = 0.09f; // 

        if (Physics.SphereCast(castCenter, castRadius, transform.forward, out hit, castDistance))
        {
            if (hit.collider.gameObject.CompareTag("Rock"))
            {
                isObstacle = true;
                canMoveForward = false;
            }
        }
        else
        {
            isObstacle = false;
            canMoveForward = true;
        }
    }
    private void AdjustBuoyancyForce()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            buoyancyForce += buoyancyStep * Time.deltaTime; // Increase buoyancyForce over time
            buoyancyForce = Mathf.Clamp(buoyancyForce, 0f, maxBuoyancyForce);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            buoyancyForce -= buoyancyStep * Time.deltaTime; // Decrease buoyancyForce over time
            buoyancyForce = Mathf.Clamp(buoyancyForce, 0f, maxBuoyancyForce);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Power"))
        {
            PowerUp powerUp = other.gameObject.GetComponent<PowerUp>();

            int materialIndex;
            if (powerUpMaterialIndices.TryGetValue(powerUp.powerUpType, out materialIndex))
            {
                if (materialIndex >= 0 && materialIndex < playerMaterials.Length)
                {
                    Material[] materials = skinMeshRenderer.sharedMaterials;
                    materials[materialIndex] = playerMaterials[materialIndex];
                    skinMeshRenderer.sharedMaterials = materials;
                }
            }
            moveSpeed += powerUp.speed;
            buoyancyForce += powerUp.buoyancy;
            drag -= powerUp.drag;
            mass += powerUp.mass;
            turnSpeed += powerUp.turning;


            Destroy(other.gameObject);
            if (buoyancyForce > maxBuoyancyForce)
            {
                maxBuoyancyForce = buoyancyForce;
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle");
            Obstacle obstacle = other.gameObject.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                // Calculate impact force using player's speed, mass, and obstacle's weight
                float impactForce = (mass * moveSpeed + obstacle.weight);

                // Apply the impact force in the opposite direction of player's velocity
                ApplyImpactForce(-velocity.normalized * impactForce);
            }
        }
        else if (other.gameObject.CompareTag("StrongStreamArea"))
        {
            Debug.Log("StrongStreamArea Enter");
            resistance = 3;
            isInStrongStream = true;
        }  

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("StrongStreamArea"))
        {
            Debug.Log("StrongStreamArea Leave");
            resistance = 0;

            isInStrongStream = false;
        }
    }

    private void ApplyImpactForce(Vector3 force)
    {
        float collisionImpact = force.magnitude;     // Calculate the magnitude of the force vector, representing the strength of the collision impact.


        // elasticity = 0.5f; // Adjust this value based on the desired elasticity of the collision
        float impactReduction = Mathf.Clamp01(elasticity * collisionImpact / (mass * moveSpeed));         // Calculate the impact reduction based on the collision impact and elasticity
                                                                                                          // This ensures that impactReduction remains within the range of 0 to 1

        // Add the impact force to velocity with reduction
        velocity += force * impactReduction;
    }

}