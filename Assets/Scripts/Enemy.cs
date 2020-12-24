using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject _victory;
    public GameObject _attackTrigger;
    public LayerMask _playerLayer;
    public LevelLoader _load;
    public Transform _attackPoint;

    Animator _anim;
    EnemyState _enemyState;
    Rigidbody2D _rb;
    [SerializeField] Transform _target;

    [SerializeField] private int _maxHealth = 100;
    public int _currentHealth;

    public float _attackCheck = 0f;
    public float _attackCheckRecharge = .25f;
    public float _attackRange = 0.5f;
    public int _hitDamage = 20;

    Vector2 _direction;
    [SerializeField] float _runSpeed = 8f;
    [SerializeField] float _combatSpeed = 3f;
    float _xVelocity = 0;

    bool _isWinner = false;


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _enemyState = GetComponent<EnemyState>();
        _rb = GetComponent<Rigidbody2D>();

        _currentHealth = _maxHealth;
        _direction = Vector2.left;
    }

    // Update is called once per frame
    void Update()
    {
        _xVelocity = Mathf.Abs(_rb.velocity.x);
        _anim.SetFloat("xVelocity", _xVelocity);
    }

    private void FixedUpdate()
    {
        if (_isWinner)
            return;

        if (_enemyState.CurrentState == EnemyState.State.advancing || _enemyState.CurrentState == EnemyState.State.retreating)
            NonCombatMovement();
        else if (_enemyState.CurrentState == EnemyState.State.attacking)
            CombatMovement();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" &&
            !_anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyBandit_Attack") &&
            _enemyState.CurrentState != EnemyState.State.retreating)
        {
            Attack();
        }
    }

    void NonCombatMovement()
    {
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyBandit_Hurt"))
        {
            CheckDirection();

            _rb.velocity = _direction * _runSpeed;
        }
    }

    void CombatMovement()
    {
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyBandit_Attack") && !_anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyBandit_Hurt"))
        {
            if (_target.transform.position.y < (transform.position.y + 1))
            {
                if (!_attackTrigger.activeInHierarchy)
                {
                    _attackTrigger.SetActive(true);
                }
                CheckDirection();
                _rb.velocity = _direction * _combatSpeed;
            }
            else
            {
                if (_attackTrigger.activeInHierarchy)
                {
                    _attackTrigger.SetActive(false);
                }
                _rb.velocity = _direction * _runSpeed;
            }
        }


    }

    void CheckDirection()
    {
        if (_enemyState.CurrentState == EnemyState.State.retreating)
        {
            if (_direction == Vector2.left && transform.position.x > _target.transform.position.x)
            {
                _direction = Vector2.right;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (_direction == Vector2.right && transform.position.x < _target.transform.position.x)
            {
                _direction = Vector2.left;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            if (_direction == Vector2.left && transform.position.x < _target.transform.position.x)
            {
                _direction = Vector2.right;
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (_direction == Vector2.right && transform.position.x > _target.transform.position.x)
            {
                _direction = Vector2.left;
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void Attack()
    {
        // Just starts the attack animation, which calls the Damage() function at the strike frame
        _rb.velocity = Vector2.zero;
        _anim.SetTrigger("attack");
    }

    void Damage()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<PlayerCombat>().TakeDamage(_hitDamage);
            Debug.Log(player.name + "'s current health: " + player.GetComponent<PlayerCombat>().currentHealth);
            if (player.GetComponent<PlayerCombat>()._isDead)
            {
                _isWinner = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        _attackTrigger.SetActive(false);
        _anim.SetTrigger("hurt");
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void EndHurt()
    {
        StartCoroutine(QuicklyDisableAttack());
    }

    IEnumerator QuicklyDisableAttack()
    {
        yield return null;
        _attackTrigger.SetActive(true);
    }

    void Die()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        Time.timeScale = 0;
        _anim.SetBool("dead", true);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 0.33f;
        yield return new WaitForSecondsRealtime(0.75f);

        Time.timeScale = 1;
        _victory.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        _load.LoadNextLevel("MainMenu");
    }
}
