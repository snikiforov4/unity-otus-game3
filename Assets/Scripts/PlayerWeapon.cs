using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerWeapon : MonoBehaviourPun
{
    public AbstractWeapon[] weapons;
    int currentWeapon = -1;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        foreach (var weapon in weapons)
            weapon.gameObject.SetActive(false);
    }

    void SetWeapon(int index)
    {
        if (index != currentWeapon) {
            if (currentWeapon >= 0)
                weapons[currentWeapon].gameObject.SetActive(false);

            currentWeapon = index;

            if (currentWeapon >= 0)
                weapons[currentWeapon].gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        for (int i = -1; i < weapons.Length; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                SetWeapon(i);
                break;
            }
        }

        if (Input.GetMouseButtonDown(0) && currentWeapon >= 0) {
            animator.SetTrigger("Shoot");
            weapons[currentWeapon].Shoot();
        }
    }
}
