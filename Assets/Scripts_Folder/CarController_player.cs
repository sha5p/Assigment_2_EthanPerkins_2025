using System.Collections.Generic;
using System;
using UnityEngine; 
using UnityEngine.InputSystem;
using static CarController_player;

public class CarController_player : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,
        Buttons
    };

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public ControlMode control;

    public float maxAcceleration = 30.0f; 
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;
    [Header("Inputs")]
    private PlayerControl playerControls;
    public InputActionReference actionReference;
    private InputAction action;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();

        carRb.centerOfMass = _centerOfMass;
        playerControls = new PlayerControl();
        action = actionReference.action;
        action.Enable();
        action.started += Brake;
        action.started += WheelEffects;
        action.canceled += WheelStop;
        var currentSpeed = PlayerPrefs.GetInt("CarSpeed", 0);
        if (currentSpeed ==25)
        {
            maxAcceleration = 40;
        }
        else if (currentSpeed == 50)
        {
            maxAcceleration = 60;
        }
        else if (currentSpeed == 75)
        {
            maxAcceleration = 80;
        }
        else if (currentSpeed == 100)
        {
            maxAcceleration = 100;
        }
    }
    
    void Update()
    {
        GetInputs();
        AnimateWheels();
    }
    Audio_Manager audio_manager;
   
    void LateUpdate()
    {
        audio_manager = GameObject.FindGameObjectWithTag("Audio").GetComponent<Audio_Manager>();
        audio_manager.PlaySFX(audio_manager.carEngine);
        Move();
        Steer();
    }

    public void MoveInput(float input)
    {

        moveInput = input;
    }

    public void SteerInput(float input)
    {
        steerInput = input;
    }

    void GetInputs()
    {
        if (control == ControlMode.Keyboard)
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
        }
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {

            wheel.wheelCollider.motorTorque = moveInput * 600*maxAcceleration *Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;

                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    private void Brake(InputAction.CallbackContext context)
    {
        if (wheels[0].wheelCollider != null)
        {
            print("BRAKING");
            foreach (var wheel in wheels)
            {

                wheel.wheelCollider.brakeTorque = brakeAcceleration;
            }
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
            
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {

            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    void WheelEffects(InputAction.CallbackContext contex)
    {
        if (wheels[0].wheelCollider != null)
        {
            foreach (var wheel in wheels)
            {
                // Only apply effects if the wheel is grounded and conditions met
                if (wheel.wheelCollider.isGrounded)
                {
                    if (wheel.axel == Axel.Rear && carRb.linearVelocity.magnitude >= 10.0f)
                    {
                        wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                        wheel.smokeParticle.Emit(10);
                    }
                }
            }
        }
            
    }
    void WheelStop(InputAction.CallbackContext contex)
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
        }
    }
}