using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Gun : AbstractWeapon
{
    public Transform source;
    public float raycastDistance;
    public int damage;

    public override void Shoot()
    {
        Ray ray = new Ray(source.position, source.forward);

        var obj = PhotonNetwork.Instantiate("shot", source.position, Quaternion.identity);
        var lineRenderer = obj.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, source.position);
        lineRenderer.SetPosition(1, source.position + source.forward.normalized * raycastDistance);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance)) {
            if (hit.collider && hit.collider.TryGetComponent<PhotonView>(out PhotonView view)) {
                view.RPC("DamageRPC", RpcTarget.All, damage);
            }
        }
    }
}
