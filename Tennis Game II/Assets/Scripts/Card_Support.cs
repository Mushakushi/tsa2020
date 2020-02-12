using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Support : MonoBehaviour
{
    public delegate Stats supports();
    private Stats stats; 
    private void Start()
    {
        stats = gameObject.GetComponent<Stats>();

        supports s = SpeedUp; 
    }

    private Stats SpeedUp()
    {
        Stats stats_t = new Stats();
        stats_t.moveSpeed += 1f; 
        return stats_t; 
    }
}
