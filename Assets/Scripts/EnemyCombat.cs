using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    private Animator anim;
    public LayerMask playerLayer;

    public float attackCheck = 0f;
    public float attackCheckRecharge = .25f;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int hitDamage = 20;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        attackCheck -= Time.deltaTime;

        if (attackCheck < 0)
        {
            CheckAttack();
        }
    }

    void CheckAttack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            Attack();
        }

        attackCheck = attackCheckRecharge;
    }

    void Attack()
    {
        // Just starts the attack animation, which calls the Damage() function at the strike frame
        anim.SetTrigger("attack");
        attackCheck = attackCheckRecharge;
    }

    private void Damage()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<PlayerCombat>().TakeDamage(hitDamage);
            Debug.Log(player.name + "'s current health: " + player.GetComponent<PlayerCombat>().currentHealth);
        }
    }
}
