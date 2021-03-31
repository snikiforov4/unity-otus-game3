using System;
using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IPunObservable
{
    [Range(0, 100)] public int health;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
            health = (int) stream.ReceiveNext();
        else
            stream.SendNext(health);
    }

    [PunRPC]
    void DamageRPC(int damage)
    {
        health = Math.Max(health - damage, 0);
    }
}