using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public LayerMask _enemyLayers;
    public GameObject _defeat;
    public LevelLoader _load;
    public Transform _attackPoint;
    public CameraShaker _shaker;

    private Animator _anim;

    [SerializeField] private int _maxHealth = 100;
    public int _currentHealth;

    public float _attackRemember = 0f;
    public float _attackRememberTime = 0.1f;

    public float _attackRange = 0.5f;
    public int _hitDamage = 10;

    public bool _isDead = false;
    private float _groundPosition;


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _shaker = GetComponent<CameraShaker>();
        _currentHealth = _maxHealth;
        _groundPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        _attackRemember -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.J))
            _attackRemember = _attackRememberTime;

        if (_attackRemember > 0 && !_anim.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Attack"))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Just starts the attack animation, which calls the Damage() function at the strike frame
        _anim.SetTrigger("attack");
        _attackRemember = 0f;
    }

    private void Damage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(_hitDamage);
            Debug.Log(enemy.name + "'s current health: " + enemy.GetComponent<Enemy>()._currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        _anim.SetTrigger("hurt");
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _isDead = true;
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
        _anim.SetBool("dead", true);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        enabled = false;
        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 0.33f;
        yield return new WaitForSecondsRealtime(0.75f);

        Time.timeScale = 1;
        transform.position = new Vector3(transform.position.x, _groundPosition, transform.position.z);
        _shaker.ShakeCamera();
        _defeat.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        _load.LoadNextLevel("MainMenu");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }
}
