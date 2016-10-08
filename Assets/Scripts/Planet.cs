using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using BeardedManStudios.Network;

public class Planet : NetworkedMonoBehavior {

    public Vector3 initialVelocity { get; set; }
    public float distance;
    public float mass;
    public ulong owner;

    // Planets can be put into the launcher temporarily for visuals only. if this is the case we don't want them updating
    public bool isTemporary = false;
    

    private Vector3 velocity;

    // Use this for initialization
    protected virtual void Start() {
        if (isTemporary) {
            GetComponent<Rigidbody>().detectCollisions = false;
        }
        else {
            velocity = initialVelocity;
            Player.planets.Add(this);
            Debug.Log("Planets " + Player.planets.Count);
        }
    }
	
	protected virtual void Update () {
        if (isTemporary)
            return;

        Vector3 acceleration = new Vector3();

        foreach (Star star in Player.stars) {
            Vector3 force = star.transform.position - transform.position;
            distance = force.magnitude;
            
            if (distance > 5000) { // Planet has gone too far and won't come back so destroy it.
                DestroyPlanet();
            }
            force.Normalize();
            float strength = (mass * star.mass) / Mathf.Pow(distance, 2);
            force *= strength;
            acceleration += force;
        }
        velocity += acceleration;
        transform.position += velocity;
	}

    void DestroyPlanet() {
        Networking.Destroy(this);
        Player.planets.Remove(this);
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.name == "Globe") {
            DestroyPlanet();
        }

    }
}
