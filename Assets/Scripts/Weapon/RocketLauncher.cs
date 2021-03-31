using Photon.Pun;
using UnityEngine;

public class RocketLauncher : AbstractWeapon
{
    public Transform prefabPosition;
    public Transform shotDirection;
    public float propulsionForce; 

    public override void Shoot()
    {
        object[] data = {shotDirection.forward * propulsionForce};
        PhotonNetwork.Instantiate("rocket", prefabPosition.position, prefabPosition.rotation, 0, data);
    }

}
