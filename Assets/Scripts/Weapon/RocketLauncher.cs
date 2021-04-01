using Photon.Pun;
using UnityEngine;

public class RocketLauncher : AbstractWeapon
{
    public Transform prefabPosition;
    public Transform shotDirection;
    public float force; 

    public override void Shoot()
    {
        object[] data = {shotDirection.forward * force};
        PhotonNetwork.Instantiate("rocket", prefabPosition.position, prefabPosition.rotation, 0, data);
    }

}
