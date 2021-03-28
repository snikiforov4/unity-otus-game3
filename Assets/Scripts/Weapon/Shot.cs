using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Shot : MonoBehaviour, IPunObservable
{
    LineRenderer lineRenderer;
    public float timeUntilDisappear;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        timeUntilDisappear -= Time.deltaTime;
        if (timeUntilDisappear <= 0.0f)
            Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading) {
            float x1 = (float)stream.ReceiveNext();
            float y1 = (float)stream.ReceiveNext();
            float z1 = (float)stream.ReceiveNext();
            float x2 = (float)stream.ReceiveNext();
            float y2 = (float)stream.ReceiveNext();
            float z2 = (float)stream.ReceiveNext();
            lineRenderer.SetPosition(0, new Vector3(x1, y1, z1));
            lineRenderer.SetPosition(1, new Vector3(x2, y2, z2));
        } else {
            Vector3 p1 = lineRenderer.GetPosition(0);
            Vector3 p2 = lineRenderer.GetPosition(1);
            stream.SendNext(p1.x);
            stream.SendNext(p1.y);
            stream.SendNext(p1.z);
            stream.SendNext(p2.x);
            stream.SendNext(p2.y);
            stream.SendNext(p2.z);
        }
    }
}
