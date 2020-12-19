using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerRagdoll : MonoBehaviour
{
    private Animator animator;
    private PhotonAnimatorView animatorView;
    private Collider shotsCollider;
    private NavMeshAgent navMeshAgent;
    private PlayerHealth health;
    private Rigidbody[] rigidBodies;

    void Start()
    {
        animator = GetComponent<Animator>();
        animatorView = GetComponent<PhotonAnimatorView>();
        shotsCollider = GetComponent<CapsuleCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<PlayerHealth>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in rigidBodies)
            body.isKinematic = true;
    }

    void Update()
    {
        if (health.health <= 0) {
            Destroy(shotsCollider);
            Destroy(animatorView);
            Destroy(animator);
            Destroy(navMeshAgent);
            foreach (Rigidbody body in rigidBodies)
                body.isKinematic = false;
            Destroy(this);
        }
    }
}
