using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enumerator containing each weapon type
public enum WeaponType
{
    None, HitScan, Projectile, Melee
}

//Struct for holding data on individual weapons
public struct WeaponInfo
{
    public string weaponName;
    public int damage;
    public WeaponType weaponType;
    public Mesh weaponMesh;
    public Material weaponMaterial;
    public float range;
    public GameObject projectile;
}

public class WeaponBehaviour : MonoBehaviour
{
    public WeaponType typeOfWeapon;
    public WeaponInfo weaponInfo;
    public int weaponDamage;
    public float weaponRange;
    public GameObject weaponProjectile;
     
    void Start()
    {
        WeaponSetup(); //setup weapon data
        GetComponentInParent<AttackBehaviour>().SetWeapon(weaponInfo); //pass weapon data to attack behaviour
    }


    void WeaponSetup()
    {
        weaponInfo.weaponType = typeOfWeapon; //assign weapon type to the type of weapon
        //weaponInfo.weaponMesh = GetComponent<MeshFilter>().mesh; //assign weapon mesh to gameobject mesh
        //weaponInfo.weaponMaterial = GetComponent<MeshRenderer>().materials[0]; //assign weapon material to gameobject material 
        weaponInfo.weaponName = gameObject.name; //assign weapon name to gameobject name

        //switch case for comparing typeOfWeapon to each weapon type
        switch (typeOfWeapon)
        {
            case WeaponType.HitScan:
                weaponInfo.damage = weaponDamage;
                weaponInfo.range = weaponRange;
                break;
            case WeaponType.Projectile:
                weaponInfo.damage = weaponDamage;
                weaponInfo.range = weaponRange;
                weaponInfo.projectile = weaponProjectile;
                break;
            case WeaponType.Melee:
                weaponInfo.damage = weaponDamage;
                weaponInfo.range = weaponRange;
                break;
            case WeaponType.None:
                break;
            default:
                Debug.LogError("Weapon type not given");
                break;
        }
    }

}
