using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    WeaponInfo weaponInfo;
    public float attackInterval;
    [HideInInspector]
    public float attackTimer;
    public Transform attackStart;
    Animator anim;

    public void SetWeapon(WeaponInfo weaponInfo_)
    {
        weaponInfo = weaponInfo_; //setWeaponInfo
        attackTimer = attackInterval; //setup attack timer to be attack interval initally
        anim = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        attackTimer = Mathf.Clamp(attackTimer += Time.deltaTime, 0f, attackInterval); //increase attackTimer up to attackInterval
    }

    public IEnumerator prepAttack()
    {
        //attack after set period
        if (attackTimer == attackInterval)
        {
            Attack();
        }
        yield return new WaitForEndOfFrame();
    }

    public void Attack()
    {
        Debug.Log("Attacking");
        anim.SetBool("Shoot", true);
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Shooting"))
        {
            Vector3 attackDir = transform.forward; //get enemy forward
            Ray attack = new Ray(attackStart.position, attackDir); //casts ray from attack start in direction enemy is looking
            RaycastHit hit;
            switch (weaponInfo.weaponType)
            {
                case WeaponType.Projectile: //if weapon is projectile
                    GameObject projectile = Instantiate(weaponInfo.projectile, attackStart.position, attackStart.rotation, GameObject.Find("GameController").transform); //create bullet at position of weapon with game controller as parent
                    if (projectile.GetComponent<Bullet>() != null) projectile.GetComponent<Bullet>().SetDamageAndRange(weaponInfo.range, weaponInfo.damage); //pass weapon range to bullet 
                    break;
                case WeaponType.Melee: //if weapon is melee - at current functionally same as hitscan
                    if (Physics.Raycast(attack, out hit, weaponInfo.range))
                    {
                        if (hit.collider.gameObject.GetComponent<HP>() != null)
                        {
                            hit.collider.gameObject.GetComponent<HP>().Damage(weaponInfo.damage);
                        }
                    }
                    break;
                case WeaponType.HitScan: //if weapon is hitscan
                    if (Physics.Raycast(attack, out hit, weaponInfo.range))
                    {
                        if (hit.collider.gameObject.GetComponent<HP>() != null)
                        {
                            hit.collider.gameObject.GetComponent<HP>().Damage(weaponInfo.damage);
                        }
                    }
                    break;
                default:
                    break;
            }
            attackTimer = 0f; //reset attackTimer
        }
        
    }

}
