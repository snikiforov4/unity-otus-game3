using Photon.Pun;
using UnityEngine;

public class Grenade : MonoBehaviour, IPunInstantiateMagicCallback
{

    public int damage;
    public float blastRadius;
    public float timeUntilExplosion;

    private void OnCollisionEnter(Collision other)
    {
        foreach (var contact in other.contacts)
        {
            if (contact.thisCollider.gameObject.TryGetComponent(out PlayerHealth health))
            {
                DoExplosion(contact.point);
            }
        }
    }

    private void DoExplosion(Vector3 explosionEpicenter)
    {
        var grenadePhotonView = GetComponent<PhotonView>();
        if (grenadePhotonView.IsMine)
        {
            var colliders = Physics.OverlapSphere(explosionEpicenter, blastRadius);
            foreach (var col in colliders)
            {
                if (col.GetComponent<PlayerHealth>() && col.TryGetComponent(out PhotonView view))
                {
                    view.RPC("DamageRPC", RpcTarget.All, damage);
                    Debug.Log($"{col.gameObject.name}");
                }
            }
        }

        grenadePhotonView.RPC("ExplodeGrenadeRPC", RpcTarget.All, explosionEpicenter);
    }

    [PunRPC]
    private void ExplodeGrenadeRPC(Vector3 explosionEpicenter)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = explosionEpicenter;
        sphere.transform.localScale = Vector3.one * (blastRadius * 2);
        sphere.GetComponent<Renderer>().material.color = Color.red;
        Destroy(sphere, 1);
        Destroy(gameObject);
    }
    
    void Update()
    {
        timeUntilExplosion -= Time.deltaTime;
        if (timeUntilExplosion <= 0.0f)
            DoExplosion(gameObject.transform.position);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var instantiationData = info.photonView.InstantiationData;
        var forceDirection = (Vector3) instantiationData[0];
        GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
    }
}