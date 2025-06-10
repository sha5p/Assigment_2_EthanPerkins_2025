using UnityEngine;
using PathCreation;

public class NPC_Movment : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 5;
    public float distanceTravled; 
    private float initialZRotation;

    void Start()
    {
        initialZRotation = 0f;
    }

    void Update()
    {
        if (pathCreator == null) return;

        distanceTravled += speed * Time.deltaTime;
        Vector3 targetPosition = pathCreator.path.GetPointAtDistance(distanceTravled);
        Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(distanceTravled);

        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(pathRotation.eulerAngles.x, pathRotation.eulerAngles.y, initialZRotation);
    }
}