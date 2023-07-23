using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAnimation : MonoBehaviour
{
    public float height = 0.5f;   // height 
    public float speed = 1f;         // speed 
    public float offset = 0f;        // starting position of the wave

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        float yPos = height * Mathf.Sin(Time.time * speed + offset);
        transform.position = initialPosition + new Vector3(0f, yPos, 0f);
    }
}
