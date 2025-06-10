using PathCreation;
using UnityEngine;
using System.Collections.Generic;

// New Component: CarPriority
public class CarPriority : MonoBehaviour
{
    public int priority = 0; 
}

public class Follow : MonoBehaviour
{
    public PathCreator PathCreator;
    public float speed = 5f;
    public float distanceTraveled;
    public float collisionRadius = 1f;
    public LayerMask collisionLayers;
    public float fixedYPosition = 0f; 
    private float initialZRotation; 

    private bool isAvoiding = false;
    private float avoidanceTime = 0f;
    public float avoidanceDuration = 2f;
    private float originalSpeed;
    public bool useHierarchyPriority = true;
    private Collider myCollider;
    private CarPriority myPriority;
    private Follow otherAvoidingFollower = null;

    void Start()
    {
        if (PathCreator == null)
        {
            Debug.LogError(gameObject.name + ": PathCreator not assigned!");
            enabled = false;
            return;
        }

        initialZRotation = 0f;
        originalSpeed = speed;
        myCollider = GetComponent<Collider>();

        if (myCollider == null)
        {
            Debug.LogError(gameObject.name + ": No Collider component found! Add a Collider to this object.");
        }

        myPriority = GetComponent<CarPriority>();
        if (myPriority == null)
        {
            myPriority = new CarPriority();
        }

        fixedYPosition = 0f;
    }

    void Update()
    {
        if (PathCreator == null) return;

        if (isAvoiding)
        {
            avoidanceTime -= Time.deltaTime;
            if (avoidanceTime <= 0)
            {
                isAvoiding = false;
                speed = originalSpeed;
                otherAvoidingFollower = null;
            }
            return;
        }

        distanceTraveled += speed * Time.deltaTime;

        Vector3 targetPosition = PathCreator.path.GetPointAtDistance(distanceTraveled);
        Quaternion pathRotation = PathCreator.path.GetRotationAtDistance(distanceTraveled);

        targetPosition.y = 0f;

        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(pathRotation.eulerAngles.x, pathRotation.eulerAngles.y, initialZRotation); // Use initialZRotation
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLLIDINNG CAR");
        if (isAvoiding) return;

        Follow other = collision.gameObject.GetComponent<Follow>();
        if (other != null)
        {
            HandlePotentialCollision(other);
        }
    }

    void HandlePotentialCollision(Follow other)
    {
        if (!useHierarchyPriority)
        {
            StartAvoidance(other);
            other.StartAvoidance(this);
            return;
        }

        if (AmIHigherPriority(other))
        {
            if (!other.isAvoiding)
            {
                other.StartAvoidance(this);
            }
        }
        else
        {
            if (!isAvoiding)
            {
                StartAvoidance(other);
            }
        }
    }

    void StartAvoidance(Follow other)
    {
        if (isAvoiding && otherAvoidingFollower == other) return;

        isAvoiding = true;
        avoidanceTime = avoidanceDuration;
        otherAvoidingFollower = other;
        speed = 0f;
    }

    bool AmIHigherPriority(Follow other)
    {
        CarPriority otherPriority = other.GetComponent<CarPriority>();
        int otherPriorityValue = (otherPriority != null) ? otherPriority.priority : 0;
        return myPriority.priority > otherPriorityValue;
    }

    public void StopMovementTemporarily()
    {
        if (!isAvoiding)
        {
            originalSpeed = speed;
        }
        speed = 0f;
        isAvoiding = true;
        avoidanceTime = avoidanceDuration;
    }

    public void ResumeMovement()
    {
        speed = originalSpeed;
        isAvoiding = false;
        avoidanceTime = 0f;
        otherAvoidingFollower = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);
    }
}
