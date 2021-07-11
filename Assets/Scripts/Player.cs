using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigid;
    private bool move;
    public Bullet fab;
    public ParticleSystem beam;
    public Ray ray_fab, ray;
    public float movespeed = 2.0f, turnspeed = 0.4f;
    private float turn;
    public int ammo = 3;
    public Text text;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        move = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);//move
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) //left
        {
            turn = 1.0f;
        }
        else
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) //right
        {
            turn = -1.0f;
        }
        else
            turn = 0.0f;
        if (Input.GetKeyDown(KeyCode.E))
            Fire();
        if (Input.GetKeyDown(KeyCode.Space) && !ray)
            Ray();
    }
    private void FixedUpdate()
    {
        if (move)
        {
            rigid.AddForce(this.transform.up * this.movespeed);
        }
        if (turn != 0.0f)
        {
            rigid.AddTorque(turn * this.turnspeed);
        }
        if(ray)
        {
            this.beam.transform.position = this.transform.position;
            this.beam.transform.rotation = this.transform.rotation;
            ray.transform.rotation = this.transform.rotation;
            BoxCollider2D box = ray.GetComponent<BoxCollider2D>();
            ray.transform.position = this.transform.position + ray.transform.up.normalized * box.size.y * ray.transform.localScale.y * 0.5f;
            Vector3 mult = new Vector3(0.0f, ray.transform.localScale.y / 5.0f, 0.0f);
            ray.transform.position = ray.transform.position + ray.transform.up.normalized * box.size.y * (ray.transform.localScale.y / 10.0f);
            ray.transform.localScale = ray.transform.localScale + mult;
            rigid.AddForce(-this.transform.up * this.movespeed/2.0f);
        }
    }
    private void Fire()
    {
        Bullet bullet = Instantiate(this.fab,this.transform.position,this.transform.rotation);
        bullet.Shoot(this.transform.up);
    }
    private void Ray()
    {
        if (ammo > 0)
        {
            this.ammo--;
            this.text.text = "Ammo: ";
            for (int i = 0; i < ammo; i++)
                this.text.text= this.text.text + "[]";
            this.ray = Instantiate(this.ray_fab, this.transform.position, this.transform.rotation);
            ray.Shoot(this.transform.up);
            this.beam.transform.position = this.transform.position;
            this.beam.transform.rotation = this.transform.rotation;
            this.beam.Play();
            Invoke(nameof(Add), 5.0f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "BadBullet")
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = 0.0f;
            this.gameObject.SetActive(false);
            FindObjectOfType<Game>().Died();
        }
    }
    void Add()
    {
        this.ammo++;
        this.text.text = "Ammo: ";
        for (int i = 0; i < ammo; i++)
            this.text.text = this.text.text + "[]";
    }
}
