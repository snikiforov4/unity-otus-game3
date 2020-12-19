using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealth : MonoBehaviour, IPunObservable
{
    [Range(0, 100)]
    public int health;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
            health = (int)stream.ReceiveNext();
        else
            stream.SendNext(health);
    }

    [PunRPC]
    void DamageRPC(int damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;
    }
}
