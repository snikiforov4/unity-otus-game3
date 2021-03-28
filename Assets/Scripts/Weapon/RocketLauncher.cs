using UnityEngine;

public class RocketLauncher : AbstractWeapon
{
    public Transform prefabPosition;
    public Transform shotDirection;
    public float propulsionForce; 
    public int damage;
    public GameObject rocketPrefab;

    public override void Shoot()
    {
        var obj = Instantiate(rocketPrefab, prefabPosition.position, prefabPosition.rotation);
        obj.GetComponent<Rigidbody>().AddForce(shotDirection.forward * propulsionForce, ForceMode.Force);
        Destroy(obj, 4);
    }

}
