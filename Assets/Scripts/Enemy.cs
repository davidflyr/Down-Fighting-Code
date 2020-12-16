﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;
    //private GameObject player = GameObject.FindWithTag("Player"); *********
    //private Rigidbody2D rb;

    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    public LevelLoader load;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //FacePlayer(); *********
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
        Time.timeScale = 0;
        anim.SetBool("dead", true);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyCombat>().enabled = false;
        enabled = false;
        yield return new WaitForSecondsRealtime(0.15f);

        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(4);
        load.LoadNextLevel("MainMenu");
    }

    /*void FacePlayer() **********
    {
        if (player.GetComponent<Rigidbody2D>().position.x > rb.position.x) // Player to the right
            transform.localScale = new Vector2(-1, 1);
        else
            transform.localScale = new Vector2(1, 1);
    }*/
}
