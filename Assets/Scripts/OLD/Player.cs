using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private NavMeshAgent agent;
    public LineRenderer Line;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0)) return;
        var r = Camera.current.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(r, out hit);
        agent.SetDestination(hit.point);
        var p = new NavMeshPath();
        agent.CalculatePath(hit.point, p);

        Debug.Log(string.Join("; ", p.corners));
        Line.positionCount = p.corners.Length;
        Line.SetPositions(p.corners);
        //StartCoroutine(LateGo(p, 3));
    }

    private IEnumerator LateGo(NavMeshPath path, float time)
    {
        yield return new WaitForSeconds(time);
        agent.SetPath(path);
    }
}
