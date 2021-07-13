using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    Rigidbody2D rigid;
    public float speed = 50.0f;
    public float size = 1.0f;
    public float minSize = 0.5f, maxSize = 2.0f;
    public float maxLifeTime = 30.0f;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, Random.value * 360.0f);
        rigid.AddTorque(5.0f);
        this.transform.localScale = Vector3.one * this.size;
        rigid.mass = this.size+2.0f;
    }
    public void Launch(Vector2 dir)
    {
        rigid.AddForce(dir * this.speed);
        Destroy(this.gameObject, this.maxLifeTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "Ray")
        {
            if(this.size/2.0f >= this.minSize)
            {
                for(int i = 0;i< this.size/ this.minSize; i++)
                    CreateAster();
            }
            FindObjectOfType<Game>().AsteroidDestroyed(this);
            Destroy(this.gameObject);
        }
    }
    private void CreateAster()
    {
        Vector2 position = this.transform.position;
        position += Random.insideUnitCircle.normalized * (this.size / (this.size / this.minSize));
        Asteroid half = Instantiate(this, position, this.transform.rotation);
        half.size = this.size / (this.size / this.minSize);
        half.speed = this.speed;
        half.Launch(Random.insideUnitCircle.normalized);
    }
}
