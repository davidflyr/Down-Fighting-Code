using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject _victory;
    private Animator _anim;

    [SerializeField] private int _maxHealth = 100;
    public int _currentHealth;

    public LevelLoader _load;


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        _anim.SetTrigger("hurt");
        _currentHealth -= damage;

        if (_currentHealth <= 0)
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
        Time.timeScale = 0;
        _anim.SetBool("dead", true);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyCombat>().enabled = false;
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
