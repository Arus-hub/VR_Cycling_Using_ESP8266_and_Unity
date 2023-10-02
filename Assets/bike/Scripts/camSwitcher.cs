using UnityEngine;
using System.Collections;

public class camSwitcher : MonoBehaviour
{

	public Camera backCamera;
	
	public Transform cameraTarget;
	private Camera currentCamera;
	//////////////////// for back Camera 
	float dist = 0.0f;
	float height = 1.5f;
	

	//new camera behaviour
	private float currentTargetAngle;
	
	private GameObject ctrlHub;// gameobject with script control variables 
	private controlHub outsideControls;// making a link to corresponding bike's script
	
	// Use this for initialization
	void Start ()
	{
		ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
		outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

		backCamera.enabled = true;
		
		currentCamera = backCamera;
		
		if (GetComponent<Rigidbody> ()) GetComponent<Rigidbody> ().freezeRotation = true;
	
		currentTargetAngle = cameraTarget.transform.eulerAngles.z;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
#if UNITY_STANDALONE || UNITY_WEBPLAYER// turn camera rotaion ONLY for mobile for free touch screen anywhere
		if (Input.GetMouseButton (1)) {

			backCamera.enabled = false;
			aroundCamera.enabled = true;
			backCamera.gameObject.SetActive (false);
			aroundCamera.gameObject.SetActive (true);
			currentCamera = aroundCamera;
			
			
			x += Input.GetAxis ("Mouse X") * xSpeed;
			y -= Input.GetAxis ("Mouse Y") * ySpeed;
			
			y = Mathf.Clamp (y, yMinLimit, yMaxLimit);
			
			
			
			xSmooth = Mathf.SmoothDamp (xSmooth, x, ref xVelocity, smoothTime);
			ySmooth = Mathf.SmoothDamp (ySmooth, y, ref yVelocity, smoothTime);
			
			
			distance = Mathf.Clamp (distance + Input.GetAxis ("Mouse ScrollWheel") * distance, distanceMin, distanceMax);
			
			
			currentCamera.transform.localRotation = Quaternion.Euler (ySmooth, xSmooth, 0);
			currentCamera.transform.position = currentCamera.transform.rotation * new Vector3 (0.0f, 0.0f, -distance) + cameraTarget.position;


		} else {
#endif
			backCamera.enabled = true;
			
			backCamera.gameObject.SetActive (true);
			
			currentCamera = backCamera;
			
			//////////////////// code for back Camera
			backCamera.fieldOfView = backCamera.fieldOfView + outsideControls.Vertical * 20f * Time.deltaTime;
			if (backCamera.fieldOfView > 85) {
				backCamera.fieldOfView = 85;
			}
			if (backCamera.fieldOfView < 50) {
				backCamera.fieldOfView = 50;
			}
			if (backCamera.fieldOfView < 60) {
				backCamera.fieldOfView = backCamera.fieldOfView += 10f * Time.deltaTime;
			}
			if (backCamera.fieldOfView > 60) {
				backCamera.fieldOfView = backCamera.fieldOfView -= 10f * Time.deltaTime;
			}
			
			float wantedRotationAngle = cameraTarget.eulerAngles.y;
			float wantedHeight = cameraTarget.position.y + height;
			float currentRotationAngle = currentCamera.transform.eulerAngles.y;
			float currentHeight = currentCamera.transform.position.y;
			
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, 3 * Time.deltaTime);
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, 2 * Time.deltaTime);
			
			Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
			currentCamera.transform.position = cameraTarget.position;
			currentCamera.transform.position -= currentRotation * Vector3.forward * dist;
			currentCamera.transform.position = new Vector3 (currentCamera.transform.position.x, currentHeight, currentCamera.transform.position.z);
			currentCamera.transform.LookAt (cameraTarget);


			// rotate camera according with bike leaning
			if (cameraTarget.transform.eulerAngles.z >0 && cameraTarget.transform.eulerAngles.z < 180) {
				currentTargetAngle = cameraTarget.transform.eulerAngles.z/10;
			}
			if (cameraTarget.transform.eulerAngles.z >180){
				currentTargetAngle = -(360-cameraTarget.transform.eulerAngles.z)/10;
			}
			currentCamera.transform.rotation = Quaternion.Euler (height*10, currentRotationAngle, currentTargetAngle);
			//to this -------------------------------------------------------------------------
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
		}
		#endif
	}
}