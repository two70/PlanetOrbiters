using UnityEngine;
using System.Collections;

public class NormalPlanet : Planet {

	// Use this for initialization
	protected override void Start () {
        base.Start();
        mass = 100;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}
}
