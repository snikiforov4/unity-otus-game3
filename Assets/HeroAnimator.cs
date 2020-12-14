using UnityEngine;
using UnityEngine.AI;


public class HeroAnimator : MonoBehaviour
{
    private Animator Animator;
    private NavMeshAgent agent;
    public NavMeshAgent Agent => agent;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        var magnitude = Agent.velocity.magnitude;
        Animator.SetBool("Moving", magnitude > 0.01f);
        Animator.SetFloat("Velocity Z", magnitude);
    }
}
