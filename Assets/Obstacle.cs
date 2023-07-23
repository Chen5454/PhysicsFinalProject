using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float weight = 1f;
    public float drag = 0.5f;
    public bool isGrounded;
    public float raycastDistance = 1f; // Distance to cast the ray for ground detection
    public float ImpactDevided;
    public Vector3 velocity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit obstacle");
            CreatureMovementPhyiscs creatureMovement = other.gameObject.GetComponent<CreatureMovementPhyiscs>();

            float impactForce = (creatureMovement.moveSpeed * creatureMovement.mass)/ ImpactDevided;

            Vector3 impactDirection = transform.position - other.transform.position;
            Vector3 impactForceVector = impactDirection.normalized * impactForce;
            ApplyImpactForce(impactForceVector);
        }
    }
    private void Update()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Max(velocity.y, 0f);
            velocity.x = Mathf.Lerp(velocity.x, 0f, drag * Time.deltaTime); 
            velocity.z = Mathf.Lerp(velocity.z, 0f, drag * Time.deltaTime); 
        }
        else
        {
            velocity -= velocity * drag * Time.deltaTime;
            velocity += Vector3.down * weight * Time.deltaTime;
        }

        transform.position += velocity * Time.deltaTime;
    }


    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }
    public void ApplyImpactForce(Vector3 force)
    {
        velocity += force / weight;
    }
}
