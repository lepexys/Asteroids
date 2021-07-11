using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigid;
    Game game;
    public BadBullet fab;
    public float movespeed = 10.0f, turnspeed = 0.4f;
    public float turn;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        game = FindObjectOfType<Game>();
        InvokeRepeating(nameof(Fire), 1.0f, 1.0f);
    }
    private void FixedUpdate()
    {
        Vector3 vec = this.game.player.transform.position - this.transform.position;
        turn = Mathf.Atan(vec.y / vec.x) * Mathf.Rad2Deg;
        if (vec.x > 0.0f)
            turn = turn + 90.0f;
        else
            turn = turn - 90.0f;
        turn = turn - (this.transform.rotation.eulerAngles.z - 180.0f);
        if (Mathf.Abs(turn) > 180.0f)
        {
            if (turn > 0.0f)
                turn -= 360.0f;
            else
                turn += 360.0f;
        }
        rigid.AddForce(this.transform.up * this.movespeed);
        if (turn != 0.0f)
        {
            rigid.AddTorque(turn/90.0f * this.turnspeed);
        }
    }
    private void Fire()
    {
        BadBullet bullet = Instantiate(this.fab, this.transform.position, this.transform.rotation);
        bullet.Shoot(this.transform.up);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet"|| collision.gameObject.tag == "Ray")
        {
            game.EnemyDestroyed();
            Destroy(this.gameObject);
        }
    }
}
