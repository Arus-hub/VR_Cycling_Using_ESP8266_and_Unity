﻿using UnityEngine;
using System.Collections;

public class rear_suspensionAmmo : MonoBehaviour {

	public Transform target;
	public Transform ammoSpring;//spring of rear suspension to squeeze
	public Transform pendulumAngle;//rear pendulum for proper squeeze of spring
	// Use this for initialization
	//void Start () {
	//}
	
	// Update is called once per frame
	void Update () {

		transform.LookAt (target);//ammo should look at rear pendulum
		ammoSpring.localScale = new Vector3(1, 0.5f-(pendulumAngle.localRotation.x*5), 1);//change those 0.5f

	}
}
