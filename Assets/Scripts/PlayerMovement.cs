using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float rotationSpeed = 60.0f;

    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        transform.rotation *= Quaternion.AngleAxis(horizontal * rotationSpeed * Time.deltaTime, Vector3.up);

        var vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.forward * vertical * movementSpeed * Time.deltaTime;
        agent.Move(forward);
        agent.SetDestination(transform.position + forward);
    }
}
