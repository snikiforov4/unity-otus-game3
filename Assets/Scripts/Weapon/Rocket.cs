using Photon.Pun;
using UnityEngine;

public class Rocket : MonoBehaviour, IPunInstantiateMagicCallback
{

    public int damage;
    public float blastRadius;
    public float timeUntilDisappear;

    private void OnCollisionEnter(Collision other)
    {
        DoExplosion(other.contacts[0].point);
    }

    private void DoExplosion(Vector3 explosionEpicenter)
    {
        var rocketPhotonView = GetComponent<PhotonView>();
        if (rocketPhotonView.IsMine)
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

        rocketPhotonView.RPC("ExplodeRocket", RpcTarget.All, explosionEpicenter);
    }

    [PunRPC]
    private void ExplodeRocket(Vector3 explosionEpicenter)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = explosionEpicenter;
        sphere.transform.localScale = Vector3.one * blastRadius;
        sphere.GetComponent<Renderer>().material.color = Color.red;
        Destroy(sphere, 1);
        Destroy(gameObject);
    }
    
    void Update()
    {
        timeUntilDisappear -= Time.deltaTime;
        if (timeUntilDisappear <= 0.0f)
            Destroy(gameObject);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var instantiationData = info.photonView.InstantiationData;
        var shotDirectionForward = (Vector3) instantiationData[0];
        GetComponent<Rigidbody>().AddForce(shotDirectionForward, ForceMode.Force);
    }
}