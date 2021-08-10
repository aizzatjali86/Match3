using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer : MonoBehaviour
{
    Vector3 _target;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        _target =new Vector3(0, 2.4f, 0);
        speed = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != transform.position)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _target, speed);
        }
    }
}
