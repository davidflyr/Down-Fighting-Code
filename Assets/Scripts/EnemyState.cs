using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Transform _rightBound;
    [SerializeField] Transform _leftBound;
    [SerializeField] float _attackingRange = 5.0f;
    [SerializeField] float _retreatingRange = 3.0f;

    Animator _anim;

    public enum State
    {
        advancing, attacking, retreating
    }
    public State CurrentState { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        CurrentState = (int)State.advancing;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {
        float distance = Vector2.Distance(transform.position, _target.transform.position);
        if (CurrentState == State.advancing)
        {
            if (distance <= _attackingRange)
            {
                CurrentState = State.attacking;
            }
        }
        else if (CurrentState == State.attacking)
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("HeavyBandit_Hurt"))
            {
                CurrentState = State.retreating;
            }
            else if (distance > _attackingRange)
            {
                CurrentState = State.advancing;
            }
        }
        else if (CurrentState == State.retreating)
        {
            if (distance > _retreatingRange || transform.position.x > _rightBound.transform.position.x || transform.position.x < _leftBound.transform.position.x)
            {
                CurrentState = State.advancing;
            }

        }
    }
    
}
