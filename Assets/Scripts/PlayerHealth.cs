using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealth : MonoBehaviour
{
    [Range(0, 100)]
    public int health;
}
