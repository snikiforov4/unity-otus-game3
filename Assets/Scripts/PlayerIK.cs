using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    Animator animator;
    public Transform lookAtTarget;
    public Transform leftToeBase;
    public Transform rightToeBase;
    public float MaxDistanceAboveGround;
    public float MaxDistanceBelowGround;
    public LayerMask layerMask;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null)
            return;

        if (lookAtTarget != null) {
            animator.SetLookAtPosition(lookAtTarget.position);
            animator.SetLookAtWeight(1.0f);
        }

        FootIK(AvatarIKGoal.LeftFoot, leftToeBase, "IKLeftFootWeight");
        FootIK(AvatarIKGoal.RightFoot, rightToeBase, "IKRightFootWeight");
    }

    void FootIK(AvatarIKGoal goal, Transform toeBase, string weightName)
    {
        float weight = animator.GetFloat(weightName);
        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);

        Vector3 footPosition = animator.GetIKPosition(goal);
        float yOffset = footPosition.y - toeBase.position.y;

        RaycastHit hit;
        Ray ray = new Ray(footPosition + Vector3.up * MaxDistanceAboveGround, Vector3.down);
        if (Physics.Raycast(ray, out hit, MaxDistanceAboveGround + MaxDistanceBelowGround, layerMask)) {
            Vector3 newFootPosition = footPosition;
            newFootPosition.y = hit.point.y + yOffset;
            animator.SetIKPosition(goal, newFootPosition);
            animator.SetIKRotation(goal, Quaternion.LookRotation(transform.forward, hit.normal));
        }
    }
}
