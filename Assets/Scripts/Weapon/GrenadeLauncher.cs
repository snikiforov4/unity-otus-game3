using Photon.Pun;
using UnityEngine;

public class GrenadeLauncher : AbstractWeapon
{
    public Transform prefabPosition;
    public float force;

    public override void Shoot()
    {
        object[] data = {prefabPosition.forward * force};
        PhotonNetwork.Instantiate("grenade", prefabPosition.position, prefabPosition.rotation, 0, data);
    }
}
