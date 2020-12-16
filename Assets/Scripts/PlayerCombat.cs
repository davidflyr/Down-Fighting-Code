using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    public LayerMask enemyLayers;

    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    public float attackRemember = 0f;
    public float attackRememberTime = 0.1f;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int hitDamage = 10;

    public LevelLoader load;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        attackRemember -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.J))
            attackRemember = attackRememberTime;

        if (attackRemember > 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Attack"))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Just starts the attack animation, which calls the Damage() function at the strike frame
        anim.SetTrigger("attack");
        attackRemember = 0f;
    }

    private void Damage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(hitDamage);
            Debug.Log(enemy.name + "'s current health: " + enemy.GetComponent<Enemy>().currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        anim.SetTrigger("hurt");
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        Time.timeScale = 0f;
        anim.SetBool("dead", true);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 0.45f;
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(3);
        load.LoadNextLevel("MainMenu");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
