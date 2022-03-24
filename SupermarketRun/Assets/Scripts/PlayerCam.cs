using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour
{

	private Camera cam;
	
	// The target we are following
	public Transform playerCar;
	public Transform _playerCar{get{return playerCar;}set{playerCar = value;	GetPlayerCar();}}
	private Rigidbody playerRigid;

	// The distance in the x-z plane to the target
	public float distance = 10.0f;

	// the height we want the camera to be above the target
	public float height = 5.0f;
	
	public float heightOffset = .75f;
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	
	public float minimumFOV = 50f;
	public float maximumFOV = 70f;
	
	public float maximumTilt = 15f;
	public float tiltAngle = 0f;

	public bool reverseCam = true;
	public bool highwayCam = false;

	void Awake(){

		cam = GetComponentInChildren<Camera>();

	}

	void Start(){

		// Early out if we don't have a target
		if (!playerCar)
			return;

		playerRigid = playerCar.GetComponent<Rigidbody>();

	}

	void GetPlayerCar(){

		if(!playerCar)
			return;

		playerRigid = playerCar.GetComponent<Rigidbody>();

		transform.position = playerCar.transform.position;

		if(!highwayCam)
			transform.rotation = playerCar.transform.rotation * Quaternion.Euler(10f, 0f, 0f);

		

	}

	public void SetPlayerCar(GameObject player){

		_playerCar = player.transform;

	}
	
	void Update(){
		
		// Early out if we don't have a target
		if (!playerCar)
			return;
		
		if(playerRigid != playerCar.GetComponent<Rigidbody>())
			playerRigid = playerCar.GetComponent<Rigidbody>();
		
		//Tilt Angle Calculation.
		tiltAngle = Mathf.Lerp (tiltAngle, (Mathf.Clamp (playerCar.InverseTransformDirection(playerRigid.velocity).x * 2f, -maximumTilt, maximumTilt)), Time.deltaTime * 5f);
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp (minimumFOV, maximumFOV, (playerRigid.velocity.magnitude * 3.6f) / 200f), Time.deltaTime * 5f);
		
	}
	
	void LateUpdate (){
		
		// Early out if we don't have a target
		if (!playerCar || !playerRigid)
			return;
		
		float speed = (playerRigid.transform.InverseTransformDirection(playerRigid.velocity).z) * 3.6f;
		
		// Calculate the current rotation angles.
		float wantedRotationAngle = playerCar.eulerAngles.y;
		float wantedHeight = playerCar.position.y + height;
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;
		
		if(speed < -2 && reverseCam)
			wantedRotationAngle = playerCar.eulerAngles.y + 180;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = playerCar.position;
		transform.position -= currentRotation * (Vector3.forward * distance);
		
		// Set the height of the camera
		if(!highwayCam)
			transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
		else
			transform.position = new Vector3(0, currentHeight, transform.position.z);
		
		// Always look at the target
		if(!highwayCam){
			transform.LookAt (new Vector3(playerCar.position.x, playerCar.position.y + heightOffset, playerCar.position.z));
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y, Mathf.Clamp(tiltAngle, -10f, 10f));
		}

	}  
	   
	
}