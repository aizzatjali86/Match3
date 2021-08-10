using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    Vector3 _target;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        _target = new Vector3(0, -5, 0);
        speed = .1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, _target, speed);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerManager>().health -= transform.parent.gameObject.GetComponent<EnemyManager>().attack;
            Destroy(gameObject);
        }
    }
}
