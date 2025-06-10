using UnityEngine;
using System.Collections.Generic;

public class TrafficCar : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public wayPoints WaypointSystem;
    public float Speed = 10f;
    public float RotationSpeed = 5f;
    public float WaypointReachRadius = 1f; 

    [Header("Car Physics")]
    public WheelCollider[] FrontWheels;
    public WheelCollider[] RearWheels;
    public float MotorForce = 1000f;
    public float MaxSteerAngle = 30f;
    public float BrakeForce = 2000f;
    public float DragCoefficient = 0.1f; 

    private int _currentWaypointIndex = 0;
    private Transform _targetWaypoint;
    private bool _isBraking = false;
    private Rigidbody _rigidbody;

    [Header("Debug")]
    public bool ShowDebugLines = true;
    public Color PathColor = Color.green;
    public Color TargetColor = Color.red;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("TrafficCar needs a Rigidbody component to function.");
            enabled = false; // Disable the script if no Rigidbody is found
            return;
        }

        // Disable default Unity physics handling (we'll apply forces directly)
        _rigidbody.useGravity = false;
        _rigidbody.linearDamping = DragCoefficient; // Apply drag to the rigidbody

        // Get the first waypoint
        if (WaypointSystem != null && WaypointSystem.transform.childCount > 0)
        {
            _targetWaypoint = WaypointSystem.transform.GetChild(_currentWaypointIndex);
        }
        else
        {
            Debug.LogError("Waypoint system is not set up correctly.  Car needs waypoints to follow.");
            enabled = false;
            return;
        }
    }

    void FixedUpdate()
    {
        if (_targetWaypoint == null) return;

        Vector3 directionToTarget = _targetWaypoint.position - transform.position;
        directionToTarget.y = 0; 

        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget <= WaypointReachRadius)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % WaypointSystem.transform.childCount;
            _targetWaypoint = WaypointSystem.transform.GetChild(_currentWaypointIndex);
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

        if (!_isBraking)
        {
            foreach (WheelCollider wheel in FrontWheels)
            {
                wheel.motorTorque = MotorForce;
                wheel.brakeTorque = 0; 
            }
            foreach (WheelCollider wheel in RearWheels)
            {
                wheel.motorTorque = MotorForce;
                wheel.brakeTorque = 0;
            }
        }
        else
        {
            foreach (WheelCollider wheel in FrontWheels)
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = BrakeForce;
            }
            foreach (WheelCollider wheel in RearWheels)
            {
                wheel.motorTorque = 0;
                wheel.brakeTorque = BrakeForce;
            }
        }

        float steeringAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        steeringAngle = Mathf.Clamp(steeringAngle, -MaxSteerAngle, MaxSteerAngle); 
        foreach (WheelCollider wheel in FrontWheels)
        {
            wheel.steerAngle = steeringAngle;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            _isBraking = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isBraking = false;
        }
    }

    void OnDrawGizmos()
    {
        if (ShowDebugLines && WaypointSystem != null)
        {
            Vector3 previousPosition = transform.position;
            for (int i = 0; i < WaypointSystem.transform.childCount; i++)
            {
                Transform waypoint = WaypointSystem.transform.GetChild(i);
                Gizmos.color = PathColor;
                Gizmos.DrawLine(previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }
            if (_targetWaypoint != null)
            {
                Gizmos.color = TargetColor;
                Gizmos.DrawLine(transform.position, _targetWaypoint.position);
                Gizmos.DrawWireSphere(_targetWaypoint.position, WaypointReachRadius); 
            }
        }
    }
}
