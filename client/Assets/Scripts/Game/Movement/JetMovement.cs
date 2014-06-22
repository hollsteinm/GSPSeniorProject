using UnityEngine;
using System.Collections;

public class JetMovement : MonoBehaviour {
	
	private float rotationX = 0.0f;
    private float rotationY = 0.0f;
	private float rotationZ = 0.0f;

    public float lookSpeed = 0.5f;

    private float forwardAcceleration = 0.0f;
    public float maxForwardAcceleration = 200.0f;
	public float forwardAccelerationReaction = 7.5f;

    private float forwardVelocity;
	public float defaultForwardVelocity = 200.0f;
	public float forwardVelocityDeviation = 100.0f;
	
	private float horizontalAcceleration = 0.0f;
	public float maxHorizontalAcceleration = 30.0f;
	public float horizontalAccelerationReaction = 15.0f;
	
	private float horizontalVelocity = 0.0f;
	public float horizontalVelocityDeviation = 20.0f;
	
	//Boundary for the level
	public int boundary = 8000;
	
	//Used for aerial maneuvers
	private bool usingManeuver = false;
	private bool hitBoundary = false;
	private AerialManeuvers.ManeuverType currentManeuver;
    private AerialManeuvers maneuver = new AerialManeuvers();
	
	//Energy is used up during boost, brake, and maneuvers
	public float maxEnergy = 100;
	public float currentEnergy = 100;
	public GUIStyle EnergyHUDStyle;
	private bool setEnergyTimer = false;
	private float energyTimer = 0;
	
	private bool infEnergy = false;
	private float infEnTimer;
	
	private Texture2D infEnTex, mainGUITex, g_OutlineGUITex, g_BackGUITex, g_BlueGUITex;
	private Texture2D g_GreenGUITex, g_RedGUITex, g_YellowGUITex;
	
	//Change controls for the ship
	private bool controlChange = false;

    public float maxVelocity
    {
        set
        {
            defaultForwardVelocity = value;
        }
    }
	
	// Use this for initialization
	void Start () {
		forwardVelocity = defaultForwardVelocity;
		rigidbody.freezeRotation = true;
		infEnTex = Resources.Load("Textures/Energy") as Texture2D;
		mainGUITex = Resources.Load("GUI/GUI") as Texture2D;
		g_OutlineGUITex = Resources.Load("GUI/GaugeOutline") as Texture2D;
		g_BackGUITex = Resources.Load("GUI/GaugeBack") as Texture2D;
		g_BlueGUITex = Resources.Load("GUI/GaugeBlue") as Texture2D;
		g_GreenGUITex = Resources.Load("GUI/GaugeGreen") as Texture2D;
		g_RedGUITex = Resources.Load("GUI/GaugeRed") as Texture2D;
		g_YellowGUITex = Resources.Load("GUI/GaugeYellow") as Texture2D;
	}
	
	void OnGUI() {
		//Initialize information
		Gun shipsGun = GetComponent<Gun>();
		ShipHull shipsHealth = GetComponent<ShipHull>();
		
		//Main GUI
		GUI.DrawTexture(new Rect(10,Screen.height - 60,
								 (Screen.width - Screen.width/4.0f) * 0.75f,100 * 0.55f), mainGUITex, ScaleMode.StretchToFill, true, 0);
		
		//Health GUI
		GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 47,
								 ((Screen.width * 5.0f)/9.0f) * 0.75f,25 * 0.55f), g_BackGUITex, ScaleMode.StretchToFill, true, 0);
		if (shipsHealth.Health > (shipsHealth.MaxHealth / 10.0f * 6.0f))
		{
			GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 47,
								 (((Screen.width * 5.0f)/9.0f) * (shipsHealth.Health/shipsHealth.MaxHealth)) * 0.75f,25 * 0.55f), g_GreenGUITex, ScaleMode.StretchToFill, true, 0);
		}
		else if (shipsHealth.Health > (shipsHealth.MaxHealth / 10.0f * 3.0f))
		{
			GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 47,
								 (((Screen.width * 5.0f)/9.0f) * (shipsHealth.Health/shipsHealth.MaxHealth)) * 0.75f,25 * 0.55f), g_YellowGUITex, ScaleMode.StretchToFill, true, 0);
		}
		else
		{
			GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 47,
								 (((Screen.width * 5.0f)/9.0f) * (shipsHealth.Health/shipsHealth.MaxHealth)) * 0.75f,25 * 0.55f), g_RedGUITex, ScaleMode.StretchToFill, true, 0);
		}
		GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 47,
								 ((Screen.width * 5.0f)/9.0f) * 0.75f,25 * 0.55f), g_OutlineGUITex, ScaleMode.StretchToFill, true, 0);
		GUI.Label(new Rect(Screen.width/11.0f - 2,Screen.height - 55, 128, 32), "Shield", EnergyHUDStyle);
		
		//Energy GUI
		GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 25,
								 ((Screen.width * 5.0f)/9.0f) * 0.75f,25 * 0.55f), g_BackGUITex, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 25,
							 (((Screen.width * 5.0f)/9.0f) * (currentEnergy/maxEnergy)) * 0.75f,25 * 0.55f), g_BlueGUITex, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(new Rect(Screen.width/11.0f + 10,Screen.height - 25,
								 ((Screen.width * 5.0f)/9.0f) * 0.75f,25 * 0.55f), g_OutlineGUITex, ScaleMode.StretchToFill, true, 0);
		GUI.Label(new Rect(Screen.width/11.0f - 2,Screen.height - 33, 128, 32), "Energy", EnergyHUDStyle);
		
		if (infEnergy)
		{
			if(!infEnTex)
			{
				Debug.LogError("Assign a Texture in the inspector.");
				return;
			}
			GUI.DrawTexture(new Rect(10,10,30,30), infEnTex, ScaleMode.ScaleToFit, true, 0);
		}
    }
	
	// Update is called once per frame
	void Update () {
		
		//Check to see if we must change the controls
		if (Input.GetKeyDown(KeyCode.C))
			controlChange = !controlChange;
		
		//Check to see if the ship has hit the boundary for the level
		if (!hitBoundary)
		{
			//Find the distance from the origin to the ship
			float distance = Mathf.Sqrt(transform.position.x * transform.position.x +
										transform.position.y * transform.position.y +
										transform.position.z * transform.position.z);
			//If this distance is past the boundary, begin turning the ship around
			if (distance >= boundary)
			{
				//Automatically rotate the ship in the correct direction
				transform.LookAt(Vector3.zero);
				transform.Rotate(Vector3.left * 180);
				transform.Rotate(Vector3.forward * 180);
				//Switch the U-Turn maneuver on
				currentManeuver = maneuver.ManualManeuver(transform,AerialManeuvers.ManeuverType.U_TURN1);
				usingManeuver = true;
				hitBoundary = true;
			}
			else
			{
				//Check to see if the right-mouse button is held down
				if (Input.GetMouseButton(1))
				{
					//If we are using a maneuver already, no need to check anything
					if (!usingManeuver)
					{
						//Otherwise, check to see if we need to make a maneuver
						//(and if we CAN)
						currentManeuver = maneuver.ChooseManeuver(transform);
						if (currentManeuver != AerialManeuvers.ManeuverType.NONE &&
							currentEnergy >= 30)
						{
							//Subtract the energy and use the maneuver
							if (!infEnergy)
							{
								currentEnergy -= 30;
								setEnergyTimer = false;
							}
							usingManeuver = true;
						}
					}
				}
				//If we are using a maneuver, the player should have no control of the ship
				if (usingManeuver)
				{
					//Update the maneuver's rotation
					usingManeuver = maneuver.UpdateManeuver(currentManeuver,transform);
					//If we have reached the destination and we have only finished the initial
					//movement for U-Turn, begin the second movement for the U-Turn
					if (!usingManeuver) 
						if (currentManeuver == AerialManeuvers.ManeuverType.U_TURN1)
						{
							usingManeuver = true;
							currentManeuver = AerialManeuvers.ManeuverType.U_TURN2;
						}
					//Move the ship forward
					transform.position += transform.forward * defaultForwardVelocity * Time.deltaTime;
				}
				else
				{
					//SHIP ROTATION
					
					float rotX = Input.GetAxis("Mouse X") * lookSpeed;
			        float rotY = Input.GetAxis("Mouse Y") * lookSpeed;
					
					if (controlChange)
						transform.Rotate (Vector3.up * rotX);
					else
						transform.Rotate (Vector3.forward * rotX *2.0f);
						transform.Rotate (Vector3.left * rotY);
					
					//FORWARD THRUSTERS
					
					//If we are not boosting or braking (or if we run out of energy), we should
					//return to the standard velocity by flipping the acceleration
					float thrust = Input.GetAxis("Vertical") * forwardAccelerationReaction;
					if ((thrust == 0 || currentEnergy == 0 || Input.GetMouseButton(1)) && forwardAcceleration != 0)
					{
						thrust = 0;
						if (forwardVelocity > defaultForwardVelocity)
							forwardAcceleration = -maxForwardAcceleration;
						else if (forwardVelocity < defaultForwardVelocity)
							forwardAcceleration = maxForwardAcceleration;
						else
							forwardAcceleration = 0;
					}
					//Otherwise we should increase the acceleration by the thrust amount
					if (currentEnergy != 0)
					{
				        forwardAcceleration += thrust;
				        forwardAcceleration = Mathf.Clamp(forwardAcceleration, -maxForwardAcceleration, maxForwardAcceleration);
					}
						
					//If we are not boosting or braking, we should reload the energy and check
					//to see if the velocity is going to reach the default velocity
					float deltaVelocity = forwardAcceleration * Time.deltaTime;
					if(thrust == 0 || currentEnergy == 0 || Input.GetMouseButton(1))
					{
						ReloadEnergy();
						if((forwardVelocity > defaultForwardVelocity && forwardVelocity + deltaVelocity < defaultForwardVelocity) ||
						   (forwardVelocity < defaultForwardVelocity && forwardVelocity + deltaVelocity > defaultForwardVelocity))
						{
							//If so, just change the values accordingly
							forwardVelocity = defaultForwardVelocity;
							deltaVelocity = 0;
						}				
					}
					//If we are boosting or braking, subtract from the energy
					else
					{
						//Subtract the energy
						if (!infEnergy)
						{
							currentEnergy -= Time.deltaTime * 10;
							setEnergyTimer = false;
							if (currentEnergy < 0)
								currentEnergy = 0;
						}
					}
			        forwardVelocity += deltaVelocity;
					
					//Make sure the velocity does not exceed the max or min values
					forwardVelocity = Mathf.Clamp(forwardVelocity,
										   defaultForwardVelocity - forwardVelocityDeviation,
						                   defaultForwardVelocity + forwardVelocityDeviation);
					
					//SIDE THRUSTERS
					
					//If we are not using the side thrusters, we should return to zero
					//horizontal velocity by flipping the acceleration
					/*thrust = Input.GetAxis("Horizontal") * horizontalAccelerationReaction;
					if ((thrust == 0 || Input.GetMouseButton(1)) && horizontalAcceleration != 0)
					{
						if (horizontalVelocity > 0)
							horizontalAcceleration = -maxHorizontalAcceleration;
						else if (horizontalVelocity < 0)
							horizontalAcceleration = maxHorizontalAcceleration;
						else
							horizontalAcceleration = 0;
					}
					
					//Otherwise we should increase the acceleration by the thrust amount
			        horizontalAcceleration += thrust;
			        horizontalAcceleration = Mathf.Clamp(horizontalAcceleration, -maxHorizontalAcceleration, maxHorizontalAcceleration);
					
					//If we are not using the side thrusters, we should check to see if the
					//velocity is going to reach zero
					deltaVelocity = horizontalAcceleration * Time.deltaTime;
					if(thrust == 0 || Input.GetMouseButton(1))
					{
						if((horizontalVelocity > 0 && horizontalVelocity + deltaVelocity < 0) ||
						   (horizontalVelocity < 0 && horizontalVelocity + deltaVelocity > 0))
						{
							//If so, just change the values accordingly
							horizontalVelocity = 0;
							deltaVelocity = 0;
						}				
					}
			        horizontalVelocity += deltaVelocity;
					
					//Make sure the velocity does not exceed the max or min values
					horizontalVelocity = Mathf.Clamp(horizontalVelocity, -horizontalVelocityDeviation, horizontalVelocityDeviation);*/
			
					thrust = Input.GetAxis("Horizontal") * lookSpeed;
					if (controlChange)
						transform.Rotate (Vector3.forward * thrust * 2.0f);
					else
						transform.Rotate (Vector3.up * thrust);
					
			        transform.position += transform.forward * forwardVelocity * Time.deltaTime;
			        //transform.position += transform.right * horizontalVelocity * Time.deltaTime;
				}
			}
			if (infEnergy)
			{
				infEnTimer -= Time.deltaTime;
				if (infEnTimer < 0)
					infEnergy = false;
			}
		}
		//If we are in the middle of the U-Turn to return to the battlefield
		if (hitBoundary)
		{
			//Update the U-Turn
			usingManeuver = maneuver.UpdateManeuver(currentManeuver,transform);
			//If we have reached the destination and we have only finished the initial
			//movement for U-Turn, begin the second movement for the U-Turn
			if (!usingManeuver)
			{
				if (currentManeuver == AerialManeuvers.ManeuverType.U_TURN1)
				{
					usingManeuver = true;
					currentManeuver = AerialManeuvers.ManeuverType.U_TURN2;
				}
				else
					hitBoundary = false;
			}
			//Move the ship forward
			transform.position += transform.forward * defaultForwardVelocity * Time.deltaTime;
		}
	}
	
	//Return whether or not we are using a maneuver
	public bool GetUsingManeuver()
	{
		return usingManeuver;
	}
	
	//Return the current maneuver
	public AerialManeuvers.ManeuverType GetManeuverType()
	{
		return currentManeuver;
	}
	
	//This will be called whenever energy should be reloading
	public void ReloadEnergy()
	{
		//If we have set the energy timer...
		if (setEnergyTimer)
		{
			//If the energy timer is finished...
			if (energyTimer < 0)
			{
				//Add to the current energy
				currentEnergy += Time.deltaTime * 20;
				if (currentEnergy > maxEnergy)
					currentEnergy = maxEnergy;
			}
			//Otherwise, continue wasting the timer
			else
				energyTimer -= Time.deltaTime;
		}
		//Otherwise, set the energy timer
		else
		{
			setEnergyTimer = true;
			energyTimer = 3;
		}
	}
	
	public void ActivateInfiniteEnergy()
	{
		infEnergy = true;
		infEnTimer = 15;
		currentEnergy = maxEnergy;
	}
}