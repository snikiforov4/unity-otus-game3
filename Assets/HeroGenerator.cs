using System.Collections.Generic;
using UnityEngine;


public class HeroGenerator : MonoBehaviour
{
    public HeroAnimator Prefab;
    //public Transform Cube;

    public Transform SpawnPoint;
    public Transform[] WayPoint;
    private float Timer;
    private float Interval = 2;
    private List<HeroAnimator> Heroes = new List<HeroAnimator>();

    private void Start()
    {
        //Instantiate(Cube);
    }

    private void Update()
    {
        if (Timer < 0)
        {
            Timer = Interval;
            var npsAgent = Instantiate(Prefab, SpawnPoint.position, Quaternion.identity, transform);
            Heroes.Add(npsAgent);
            var point = WayPoint[Random.Range(0, WayPoint.Length)];
            npsAgent.Agent.SetDestination(point.position);
            Heroes[Random.Range(0, WayPoint.Length)].Agent.SetDestination(point.position);
        }
        else Timer -= Time.deltaTime;
    }
}
