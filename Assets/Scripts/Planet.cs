using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using BeardedManStudios.Network;

public class Planet : NetworkedMonoBehavior {

    public Vector3 initialVelocity { get; set; }

    protected float mass;
    public float distance;

    private Vector3 velocity;

    // Use this for initialization
    protected virtual void Start() {
        
        velocity = initialVelocity;
    }
	
	protected virtual void Update () {
        Vector3 acceleration = new Vector3();
        foreach (Player player in Player.players) {
            Vector3 force = new Vector3();
            force = player.transform.position - transform.position;
            distance = force.magnitude;

            if (distance > 5000) { // Planet has gone too far and won't come back so destroy it.
                Networking.Destroy(this);
            }
            force.Normalize();
            float strength = (mass * player.mass) / Mathf.Pow(distance, 2);
            force *= strength;
            acceleration += force;
        }
        velocity += acceleration;
        transform.position += velocity;
	}

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.name == "Globe") {
           Networking.Destroy(this);
        }

    }
}
