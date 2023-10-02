using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;

public class keyboardControls : MonoBehaviour {



	private GameObject ctrlHub;// making a link to corresponding bike's script
	private controlHub outsideControls;// making a link to corresponding bike's script

	// Use this for initialization


	/*
		IEnumerator Start () {
			ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
			outsideControls = ctrlHub.GetComponent<controlHub>();// making a link to corresponding bike's script
			yield return new WaitForSeconds (3f);
			// Create UDP client
			int receiverPort = 8888;
			UdpClient receiver = new UdpClient (receiverPort);
			receiver.BeginReceive (DataReceived, receiver);
		}

		// This is called whenever data is received
		private void DataReceived (IAsyncResult ar) {

			UdpClient c = (UdpClient)ar.AsyncState;
			IPEndPoint receivedIpEndPoint = new IPEndPoint (IPAddress.Any, 0);
			Byte [] receivedBytes = c.EndReceive (ar, ref receivedIpEndPoint);

			string packet = System.Text.Encoding.UTF8.GetString (receivedBytes, 0, 20);
			if (packet.Contains ("_")) {
				HandleVertical (packet);
			} else {
				HandleHorizontal (packet);
			}

			// Restart listening for udp data packages
			c.BeginReceive (DataReceived, ar.AsyncState);
		}

		void HandleVertical(string vert)
		{
			if (vert.Contains("_GO"))
			{
				outsideControls.Vertical = 1;
			}
		}

		void HandleHorizontal(string hori)
		{
			outsideControls.Horizontal = float.Parse(hori);


		}

	*/
	// Update is called once per frame
	void Update () {
		/*//////////////////////////////////// ACCELERATE, braking & 'full throttle - manual trick' //////////////////////////////////////////////
		if (!Input.GetKey(KeyCode.Alpha2))
		{
			outsideControls.Vertical = Input.GetAxis("Vertical") / 1.112f;//to get less than 0.9 as acceleration to prevent wheelie(wheelie begins at >0.9)
			if (Input.GetAxis("Vertical") < 0) outsideControls.Vertical = outsideControls.Vertical * 1.112f;//need to get 1(full power) for front brake
		}

		//to get less than 0.9 as acceleration to prevent wheelie(wheelie begins at >0.9)


		//////////////////////////////////// STEERING /////////////////////////////////////////////////////////////////////////
		outsideControls.Horizontal = Input.GetAxis("Horizontal");
		if (Input.GetKey(KeyCode.Alpha2)) outsideControls.Vertical = 1;

		//}
		*/
		//////////////////////////////////// Rider's mass translate ////////////////////////////////////////////////////////////
		//this strings controls pilot's mass shift along bike(vertical)
		if (Input.GetKey (KeyCode.F)) {
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift += 0.1f;
			if (outsideControls.VerticalMassShift > 1.0f) outsideControls.VerticalMassShift = 1.0f;
		}

		if (Input.GetKey(KeyCode.V)){
			outsideControls.VerticalMassShift = outsideControls.VerticalMassShift -= 0.1f;
			if (outsideControls.VerticalMassShift < -1.0f) outsideControls.VerticalMassShift = -1.0f;
		}
		if(!Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.V)) outsideControls.VerticalMassShift = 0;

		//this strings controls pilot's mass shift across bike(horizontal)
		if (Input.GetKey(KeyCode.E)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift += 0.1f;
			if (outsideControls.HorizontalMassShift >1.0f) outsideControls.HorizontalMassShift = 1.0f;
		}

		if (Input.GetKey(KeyCode.Q)){
			outsideControls.HorizontalMassShift = outsideControls.HorizontalMassShift -= 0.1f;
			if (outsideControls.HorizontalMassShift < -1.0f) outsideControls.HorizontalMassShift = -1.0f;
		}
		if(!Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q)) outsideControls.HorizontalMassShift = 0;


		//////////////////////////////////// Rear Brake ////////////////////////////////////////////////////////////////
		// Rear Brake
		if (Input.GetKey (KeyCode.X)) {
			outsideControls.rearBrakeOn = true;
		} else
			outsideControls.rearBrakeOn = false;

		//////////////////////////////////// Restart ////////////////////////////////////////////////////////////////
		// Restart & full restart
		if (Input.GetKey (KeyCode.R)) {
			outsideControls.restartBike = true;
		} else
			outsideControls.restartBike = false;

		// RightShift for full restart
		if (Input.GetKey (KeyCode.RightShift)) {
			outsideControls.fullRestartBike = true;
		} else
			outsideControls.fullRestartBike = false;

		//////////////////////////////////// Reverse ////////////////////////////////////////////////////////////////
		// Restart & full restart
		if(Input.GetKeyDown(KeyCode.C)){
				outsideControls.reverse = true;
		} else outsideControls.reverse = false;
		///
	}
}
