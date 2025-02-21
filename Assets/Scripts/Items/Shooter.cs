using NUnit.Framework;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Shooter : Weapon, Equipment {


    [Header("Required References")]
    public GameObject projectilePrefab;


    GameObject projectile;
    GameObject player;

    private void Start() {
        //make sure this is a child of the owner object!
        owner = GetComponentInParent<Vitality>();
        player = GameObject.FindGameObjectWithTag("Player");
    }


    public override void StartSwinging() {
        projectile = Instantiate(projectilePrefab, this.transform.position, Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up));
        projectile.GetComponent<Projectile>().Configure(player.transform, GetComponentInParent<Vitality>());
    }


    public override void StopSwinging() {
        if (projectile != null) {
            Destroy(projectile);
        }
    }


}
