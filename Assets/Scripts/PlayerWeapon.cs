using Photon.Pun;
using UnityEngine;

public class PlayerWeapon : MonoBehaviourPun
{
    public AbstractWeapon[] weapons;

    private int _currentWeapon = -1;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        foreach (var weapon in weapons)
            weapon.gameObject.SetActive(false);
    }

    void SetWeapon(int index)
    {
        if (index != _currentWeapon) {
            if (_currentWeapon >= 0)
                weapons[_currentWeapon].gameObject.SetActive(false);

            _currentWeapon = index;

            if (_currentWeapon >= 0)
                weapons[_currentWeapon].gameObject.SetActive(true);
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

        if (Input.GetMouseButtonDown(0) && _currentWeapon >= 0) {
            _animator.SetTrigger("Shoot");
            weapons[_currentWeapon].Shoot();
        }
    }
}
