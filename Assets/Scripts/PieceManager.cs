using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    GameObject parent;
    GameObject instance;

    GameObject player;

    public bool moving;
    bool attacking;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        transform.localPosition = new Vector2(0, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 _target;
        float speed;
        if (gameObject.transform.parent.name == "Garbage")
        {
            _target =new Vector3(transform.position.x, 0, 0);
            speed = 0.05f;
        }
        else
        {
            _target = Vector3.zero;
            speed = 0.3f;
        }

        if (_target != transform.localPosition)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _target, speed);
            moving = true;
        }
        else
        {
            moving = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "end")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "enemy")
        {
            collision.gameObject.GetComponent<EnemyManager>().health -= player.GetComponent<PlayerManager>().attack;
            Destroy(gameObject);
        }
    }
}
