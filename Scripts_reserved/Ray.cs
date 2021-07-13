using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    public float lifetime = 0.5f;
    private Rigidbody2D rigid;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        BoxCollider2D box = this.GetComponent<BoxCollider2D>();
        this.transform.position = this.transform.position + this.transform.up.normalized * box.size.y * this.transform.localScale.y * 0.5f;
    }
    public void Shoot(Vector2 dir)
    {
        Destroy(this.gameObject, this.lifetime);
    }
}
