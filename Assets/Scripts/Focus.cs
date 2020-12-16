using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
{
    [SerializeField] Focus _opponent;
    [SerializeField] float _maxDistance = 7.36f;
    [SerializeField] float _maxYOffset = 1.5f;
    [SerializeField] float _baseYOffset = 2.0f;

    Transform parent;
    float _distance;
    float _yOffset;
    

    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponent<Transform>().parent;
        _distance = Mathf.Abs(transform.position.x - _opponent.transform.position.x);
    }

    // Update is called once per frame
    void Update()
    {
        _distance = Mathf.Abs(transform.position.x - _opponent.transform.position.x);
        _yOffset = (_distance > 0.5f) ? _maxYOffset/_maxDistance * _distance + _baseYOffset : _baseYOffset;

        transform.position = new Vector3(
            parent.position.x,
            parent.position.y + _yOffset,
            parent.position.z
            );
    }
}
