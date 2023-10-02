using UnityEngine;
using System.Collections;

public class bicycle_code : MonoBehaviour
{
    ///////////////////////////////////////////////////////// wheels ///////////////////////////////////////////////////////////
    // define wheel colliders
    public WheelCollider coll_frontWheel;
    public WheelCollider coll_rearWheel;
    // visual for wheels
    public GameObject meshFrontWheel;
    public GameObject meshRearWheel;
    // check isn't front wheel in air for front braking possibility
    bool isFrontWheelInAir = true;

    //////////////////////////////////////// Stifness, CoM(center of mass), crahsed /////////////////////////////////////////////////////////////
    //for stiffness counting when rear brake is on. Need that to lose real wheel's stiffness during time
    float stiffPowerGain = 0.0f;
    //for CoM moving along and across bike. Pilot's CoM.
    float tmpMassShift = 0.0f;
    // crashed status. To know when we need to desable controls because bike is too leaned.
    public bool crashed = false;
    // there is angles when bike takes status crashed(too much lean, or too much stoppie/wheelie)
    public float crashAngle01;										
    public float crashAngle02;											
    public float crashAngle03;											
    public float crashAngle04;											

    // define CoM of bike
    public Transform CoM; //CoM object
    public float normalCoM; //normalCoM is for situation when script need to return CoM in starting position										
    public float CoMWhenCrahsed; //we beed lift CoM for funny bike turning around when crahsed													

    //////////////////// some meshes for display visual parts of bike ////////////////////////////////////////////
    public Transform rearPendulumn; //rear pendulumn
    public Transform steeringWheel; //wheel bar
    public Transform suspensionFront_down; //lower part of front forge
    private int normalFrontSuspSpring; 
    private int normalRearSuspSpring; 
    private bool forgeBlocked = true;
                                      

    private float baseDistance; // need to know distance between wheels - base. It's for wheelie compensate(dont want wheelie for long bikes)

    // we need to clamp wheelbar angle according the speed. it means - the faster bike rides the less angle you can rotate wheel bar
    public AnimationCurve wheelbarRestrictCurve = new AnimationCurve(new Keyframe(0f, 20f), new Keyframe(100f, 1f));

    // temporary variable to restrict wheel angle according speed
    private float tempMaxWheelAngle;

    //for wheels vusials match up the wheelColliders
    private Vector3 wheelCCenter;
    private RaycastHit hit;

    /////////////////////////////////////////// technical variables ///////////////////////////////////////////////////////
    public float frontBrakePower; //brake power absract - 100 is good brakes																		

    public float LegsPower;																	
    
    
    public float airRes;  																										// 1 is neutral

    private GameObject ctrlHub;// gameobject with script control variables 
    private controlHub outsideControls;// making a link to corresponding bike's script
                                       /////////////////////////////////////////////////// BICYCLE CODE ///////////////////////////////////////////////////////
    private float frontWheelAPD;// usualy 0.05f
    private GameObject pedals;
    private pedalControls linkToStunt;
    private bool rearPend;

    [HideInInspector]
    public float bikeSpeed; //to know bike speed km/h
    public bool isReverseOn = false; //to turn On and Off reverse speed
    ////////////////////////////////////////////////  ON SCREEN INFO ///////////////////////////////////////////////////////
    void OnGUI()
    {
        GUIStyle biggerText = new GUIStyle("label");
        biggerText.fontSize = 40;
        GUIStyle middleText = new GUIStyle("label");
        middleText.fontSize = 22;
        GUIStyle smallerText = new GUIStyle("label");
        smallerText.fontSize = 14;

        //to show speed on display interface
        GUI.color = Color.black;
        GUI.Label(new Rect(Screen.width * 0.875f, Screen.height * 0.9f, 120, 80), string.Format("" + "{0:0.}", bikeSpeed), biggerText);

        if (!isReverseOn)
        {
            GUI.color = Color.grey;
            GUI.Label(new Rect(Screen.width * 0.885f, Screen.height * 0.96f, 60, 40), "REAR", smallerText);
        }
        else
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(Screen.width * 0.885f, Screen.height * 0.96f, 60, 40), "REAR", smallerText);
        }

       


    }
    void Start()
    {

        //if there is no pendulum linked to script in Editor, it means MTB have no rear suspension, so no movement of rear wheel(pendulum)
        if (rearPendulumn)
        {
            rearPend = true;
        }
        else rearPend = false;

        //bicycle code
        frontWheelAPD = coll_frontWheel.forceAppPointDistance;

        ctrlHub = GameObject.Find("gameScenario");//link to GameObject with script "controlHub"
        outsideControls = ctrlHub.GetComponent<controlHub>();//to connect c# mobile control script to this one

        pedals = GameObject.Find("bicycle_pedals");
        linkToStunt = pedals.GetComponent<pedalControls>();

        Vector3 setInitialTensor = GetComponent<Rigidbody>().inertiaTensor;//this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass
        GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);// now Center of Mass(CoM) is alligned to GameObject "CoM"
        GetComponent<Rigidbody>().inertiaTensor = setInitialTensor;////this string is necessary for Unity 5.3f with new PhysX feature when Tensor decoupled from center of mass

        // wheel colors for understanding of accelerate, idle, brake(white is idle status)
        meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
        meshRearWheel.GetComponent<Renderer>().material.color = Color.black;

        //for better physics of fast moving bodies
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

        // too keep LegsPower variable like "real" horse powers
        LegsPower = LegsPower * 20;

        //*30 is for good braking to keep frontBrakePower = 100 for good brakes.
        frontBrakePower = frontBrakePower * 30;//30 is abstract but necessary for Unity5

        //tehcnical variables
        normalRearSuspSpring = (int)coll_rearWheel.suspensionSpring.spring;
        normalFrontSuspSpring = (int)coll_frontWheel.suspensionSpring.spring;

        baseDistance = coll_frontWheel.transform.localPosition.z - coll_rearWheel.transform.localPosition.z;// now we know distance between two wheels

        var tmpMeshRWh01 = meshRearWheel.transform.localPosition;
        tmpMeshRWh01.y = meshRearWheel.transform.localPosition.y - coll_rearWheel.suspensionDistance / 4;
        meshRearWheel.transform.localPosition = tmpMeshRWh01;


        //and bike's frame direction
        var tmpCollRW01 = coll_rearWheel.transform.localPosition;
        tmpCollRW01.y = coll_rearWheel.transform.localPosition.y - coll_rearWheel.transform.localPosition.y / 20;
        coll_rearWheel.transform.localPosition = tmpCollRW01;

    }
    void FixedUpdate()
    {

        ApplyLocalPositionToVisuals(coll_frontWheel);
        ApplyLocalPositionToVisuals(coll_rearWheel);


        //////////////////////////////////// part where rear pendulum, wheelbar and wheels meshes matched to wheelsColliers and so on
        
        if (rearPend)
        {//rear pendulum moves only when bike is full suspension
            var tmp_cs1 = rearPendulumn.transform.localRotation;
            var tmp_cs2 = tmp_cs1.eulerAngles;
            tmp_cs2.x = 0 - 8 + (meshRearWheel.transform.localPosition.y * 100);
            tmp_cs1.eulerAngles = tmp_cs2;
            rearPendulumn.transform.localRotation = tmp_cs1;
        }
        //beauty - wheel bar rotating by front wheel
        var tmp_cs3 = suspensionFront_down.transform.localPosition;
        tmp_cs3.y = (meshFrontWheel.transform.localPosition.y - 0.15f);
        suspensionFront_down.transform.localPosition = tmp_cs3;
        var tmp_cs4 = meshFrontWheel.transform.localPosition;
        tmp_cs4.z = meshFrontWheel.transform.localPosition.z - (suspensionFront_down.transform.localPosition.y + 0.4f) / 5;
        meshFrontWheel.transform.localPosition = tmp_cs4;

        // debug - all wheels are white in idle(no accelerate, no brake)
        meshFrontWheel.GetComponent<Renderer>().material.color = Color.black;
        meshRearWheel.GetComponent<Renderer>().material.color = Color.black;

        // drag and angular drag for emulate air resistance
        if (!crashed)
        {
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 210 * airRes; 
            GetComponent<Rigidbody>().angularDrag = 7 + GetComponent<Rigidbody>().velocity.magnitude / 20;
        }

        //determinate the bike speed in km/h
        bikeSpeed = Mathf.Round((GetComponent<Rigidbody>().velocity.magnitude * 3.6f) * 10) * 0.1f; //from m/s to km/h

        ///bicycle code
        coll_frontWheel.forceAppPointDistance = frontWheelAPD - bikeSpeed / 1000;
        if (coll_frontWheel.forceAppPointDistance < 0.001f)
        {
            coll_frontWheel.forceAppPointDistance = 0.001f;
        }

        //////////////////////////////////// acceleration & brake /////////////////////////////////////////////////////////////
        //////////////////////////////////// ACCELERATE /////////////////////////////////////////////////////////////
        if (!crashed && outsideControls.Vertical > 0 && !isReverseOn)
        {//case with acceleration from 0.0f to 0.9f throttle
            coll_frontWheel.brakeTorque = 0;
            coll_rearWheel.motorTorque = LegsPower * outsideControls.Vertical;

            // debug - rear wheel is green when accelerate
            meshRearWheel.GetComponent<Renderer>().material.color = Color.green;

            // when normal accelerating CoM z is averaged
            var tmp_cs5 = CoM.localPosition;
            tmp_cs5.z = 0.0f + tmpMassShift;
            tmp_cs5.y = normalCoM;
            CoM.localPosition = tmp_cs5;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        //case for reverse
        if (!crashed && outsideControls.Vertical > 0 && isReverseOn)
        {
            coll_rearWheel.motorTorque = LegsPower * -outsideControls.Vertical / 2 + (bikeSpeed * 50);//need to make reverse really slow

            // debug - rear wheel is green when accelerate
            meshRearWheel.GetComponent<Renderer>().material.color = Color.green;

            // when normal accelerating CoM z is averaged
            var tmp_cs6 = CoM.localPosition;
            tmp_cs6.z = 0.0f + tmpMassShift;
            tmp_cs6.y = normalCoM;
            CoM.localPosition = tmp_cs6;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }


        //////////////////////////////////// BRAKING /////////////////////////////////////////////////////////////
        //////////////////////////////////// front brake /////////////////////////////////////////////////////////
        int springWeakness = 0;
        if (!crashed && outsideControls.Vertical < 0 && !isFrontWheelInAir)
        {

            coll_frontWheel.brakeTorque = frontBrakePower * -outsideControls.Vertical;
            coll_rearWheel.motorTorque = 0; // you can't do accelerate and braking same time.

           

            if (bikeSpeed > 1)
            {// no CoM pull up when speed is zero

                
                float rearBrakeAddon = 0.0f;
                if (outsideControls.rearBrakeOn)
                {
                    rearBrakeAddon = 0.0025f;
                }
                var tmp_cs11 = CoM.localPosition;
                tmp_cs11.y += (frontBrakePower / 200000) + tmpMassShift / 50f - rearBrakeAddon;
                tmp_cs11.z += 0.0025f;
                CoM.localPosition = tmp_cs11;

            }
            else if (bikeSpeed <= 1 && !crashed && this.transform.localEulerAngles.z < 45 || bikeSpeed <= 1 && !crashed && this.transform.localEulerAngles.z > 315)
            {
                if (this.transform.localEulerAngles.x < 5 || this.transform.localEulerAngles.x > 355)
                {
                    var tmp_cs12 = CoM.localPosition;
                    tmp_cs12.y = normalCoM;
                    CoM.localPosition = tmp_cs12;
                }
            }

            if (CoM.localPosition.y >= -0.2f)
            {
                var tmp_cs13 = CoM.localPosition;
                tmp_cs13.y = -0.2f;
                CoM.localPosition = tmp_cs13;
            }

            if (CoM.localPosition.z >= 0.2f + (GetComponent<Rigidbody>().mass / 500))
            {
                CoM.localPosition = new Vector3(CoM.localPosition.x, 0.2f + (GetComponent<Rigidbody>().mass / 500), CoM.localPosition.z);
            }

            //////////// 
            // front suspension when forge spring is compressed
        
            float maxFrontSuspConstrain;
            maxFrontSuspConstrain = CoM.localPosition.z;
            if (maxFrontSuspConstrain >= 0.5f) maxFrontSuspConstrain = 0.5f;
            springWeakness = (int)(normalFrontSuspSpring - (normalFrontSuspSpring * 1.5f) * maxFrontSuspConstrain);


            
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
            // debug - wheel is red when braking
            meshFrontWheel.GetComponent<Renderer>().material.color = Color.red;

            //we need to mark suspension as very compressed to make it weaker
            forgeBlocked = true;
        }
        else FrontSuspensionRestoration(springWeakness);//here is function for weak front spring and return it's force slowly








        //////////////////////////////////// turnning /////////////////////////////////////////////////////////////			
			
        tempMaxWheelAngle = wheelbarRestrictCurve.Evaluate(bikeSpeed);//associate speed with curve which tuned in Editor

        if (!crashed && outsideControls.Horizontal != 0)
        {

            // while speed is high, wheelbar is restricted 
            coll_frontWheel.steerAngle = tempMaxWheelAngle * outsideControls.Horizontal;
            steeringWheel.rotation = coll_frontWheel.transform.rotation * Quaternion.Euler(0, coll_frontWheel.steerAngle, coll_frontWheel.transform.rotation.z);
        }
        else coll_frontWheel.steerAngle = 0;


        /////////////////////////////////////////////////// PILOT'S MASS //////////////////////////////////////////////////////////

        if (outsideControls.VerticalMassShift > 0)
        {
            tmpMassShift = outsideControls.VerticalMassShift / 12.5f;//12.5f to get 0.08fm at final
            var tmp_cs19 = CoM.localPosition;
            tmp_cs19.z = tmpMassShift;
            CoM.localPosition = tmp_cs19;

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        if (outsideControls.VerticalMassShift < 0)
        {
            tmpMassShift = outsideControls.VerticalMassShift / 12.5f;//12.5f to get 0.08fm at final
            var tmp_cs20 = CoM.localPosition;
            tmp_cs20.z = tmpMassShift;
            CoM.localPosition = tmp_cs20;

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        if (outsideControls.HorizontalMassShift < 0)
        {
            var tmp_cs21 = CoM.localPosition;
            tmp_cs21.x = outsideControls.HorizontalMassShift / 40;
            CoM.localPosition = tmp_cs21;//40 to get 0.025m at final

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);

        }
        if (outsideControls.HorizontalMassShift > 0)
        {
            var tmp_cs22 = CoM.localPosition;
            tmp_cs22.x = outsideControls.HorizontalMassShift / 40;
            CoM.localPosition = tmp_cs22;//40 to get 0.025m at final

            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }


        //auto back CoM when any key not pressed
        if (!crashed && outsideControls.Vertical == 0 && !outsideControls.rearBrakeOn && !linkToStunt.stuntIsOn || (outsideControls.Vertical < 0 && isFrontWheelInAir))
        {
            var tmp_cs23 = CoM.localPosition;
            tmp_cs23.y = normalCoM;
            tmp_cs23.z = 0.0f + tmpMassShift;
            CoM.localPosition = tmp_cs23;
            coll_frontWheel.motorTorque = 0;
            coll_frontWheel.brakeTorque = 0;
            coll_rearWheel.motorTorque = 0;
            coll_rearWheel.brakeTorque = 0;
            GetComponent<Rigidbody>().centerOfMass = new Vector3(CoM.localPosition.x, CoM.localPosition.y, CoM.localPosition.z);
        }
        //autoback pilot's CoM along
        if (outsideControls.VerticalMassShift == 0 && outsideControls.Vertical >= 0 && outsideControls.Vertical <= 0.9f && !outsideControls.rearBrakeOn && !linkToStunt.stuntIsOn)
        {
            var tmp_cs24 = CoM.localPosition;
            tmp_cs24.z = 0.0f;
            CoM.localPosition = tmp_cs24;
            tmpMassShift = 0.0f;
        }
        //autoback pilot's CoM across

        if (outsideControls.HorizontalMassShift == 0 && outsideControls.Vertical <= 0 && !outsideControls.rearBrakeOn)
        {
            var tmp_cs25 = CoM.localPosition;
            tmp_cs25.x = 0.0f;
            CoM.localPosition = tmp_cs25;
        }






    }

    //void Update (){
    // everything here is about physics
    //}
    ///////////////////////////////////////////// FUNCTIONS /////////////////////////////////////////////////////////
    void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);
        wheelCCenter = collider.transform.TransformPoint(collider.center);


        
        if (!rearPend)
        {
            if (collider.gameObject.name != "coll_rear_wheel")
            {
                if (Physics.Raycast(wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius))
                {
                    visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
                    if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;
                }
                else
                {
                    visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                    if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
                }
            }
        }
        else
        {
            if (Physics.Raycast(wheelCCenter, -collider.transform.up, out hit, collider.suspensionDistance + collider.radius))
            {
                visualWheel.transform.position = hit.point + (collider.transform.up * collider.radius);
                if (collider.name == "coll_front_wheel") isFrontWheelInAir = false;

            }
            else
            {
                visualWheel.transform.position = wheelCCenter - (collider.transform.up * collider.suspensionDistance);
                if (collider.name == "coll_front_wheel") isFrontWheelInAir = true;
            }

        }

        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        collider.GetWorldPose(out position, out rotation);

        visualWheel.localEulerAngles = new Vector3(visualWheel.localEulerAngles.x, collider.steerAngle - visualWheel.localEulerAngles.z, visualWheel.localEulerAngles.z);
        visualWheel.Rotate(collider.rpm / 60 * 360 * Time.deltaTime, 0.0f, 0.0f);

    }
    //need to restore spring power for rear suspension after make it harder for wheelie
    void RearSuspensionRestoration()
    {
        var tmpRearSusp = coll_rearWheel.suspensionSpring;
        tmpRearSusp.spring = normalRearSuspSpring;
        coll_rearWheel.suspensionSpring = tmpRearSusp;   
    }
    //need to restore spring power for front suspension after make it weaker for stoppie
    void FrontSuspensionRestoration(int sprWeakness)
    {
        if (forgeBlocked)
        {//supress front spring power to avoid too much force back
            var tmpFrntSusp = coll_frontWheel.suspensionSpring;
            tmpFrntSusp.spring = sprWeakness;
            coll_frontWheel.suspensionSpring = tmpFrntSusp;
            forgeBlocked = false;
        }
        if (coll_frontWheel.suspensionSpring.spring < normalFrontSuspSpring)
        {//slowly returning force to front spring
            var tmpFrntSusp2 = coll_frontWheel.suspensionSpring;
            tmpFrntSusp2.spring += 500.0f;
            coll_frontWheel.suspensionSpring = tmpFrntSusp2;
        }
    }
}