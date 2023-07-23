    using System.Collections;
    using System.Collections.Generic;
    using Cinemachine;
    using UnityEngine;
    public enum PowerUpType
    {
        FrontFin,
        Tail,
        Helment,
        BackFin,
    }

    public class PowerUp : MonoBehaviour
    {
        public PowerUpType powerUpType;

        // what will be affect the player.
        public float speed;
        public float buoyancy;
        public float mass;
        public float drag;
        public float turning;

    }
