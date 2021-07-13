using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameLib
{
    public class Spawner : MonoBehaviour
    {
        public bool gameOn = false;
        public Asteroid fab;
        public Enemy enemy_fab;
        public float trajectoryVariance = 10.0f;
        public float spawnRate = 1.0f, spawnEnemyRate = 5.0f;
        public int spawnAmount = 2, total = 0;
        public float distance = 15.0f;
        void Start()
        {
            InvokeRepeating(nameof(Spawn), this.spawnRate, this.spawnRate);
            InvokeRepeating(nameof(Attack), this.spawnEnemyRate, this.spawnEnemyRate);
        }
        void Spawn()
        {
            if (gameOn)
            {
                for (int i = 0; i < this.spawnAmount; i++)
                {
                    Vector3 direct = Random.insideUnitCircle.normalized * this.distance;
                    Vector3 position = this.transform.position + direct;
                    float variance = Random.Range(-this.trajectoryVariance, this.trajectoryVariance);
                    Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);
                    Asteroid asteroid = Instantiate(this.fab, position, rotation);
                    asteroid.size = Random.Range(asteroid.minSize, asteroid.maxSize);
                    asteroid.Launch(rotation * -direct);
                }
            }
        }
        void Attack()
        {
            if (this.total + this.spawnAmount < 10 && gameOn)
            {
                this.total += this.spawnAmount;
                for (int i = 0; i < this.spawnAmount; i++)
                {
                    Vector3 direct = Random.insideUnitCircle.normalized * this.distance;
                    Vector3 position = this.transform.position + direct;
                    float variance = Random.Range(-this.trajectoryVariance, this.trajectoryVariance);
                    Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);
                    Enemy enemy = Instantiate(this.enemy_fab, position, rotation);
                }
            }
        }
    }

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

    public class Player : MonoBehaviour
    {
        public Rigidbody2D rigid;
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
            InvokeRepeating(nameof(Add), 5.0f, 5.0f);
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
            if (ray)
            {
                this.beam.transform.position = this.transform.position;
                this.beam.transform.rotation = this.transform.rotation;
                ray.transform.rotation = this.transform.rotation;
                BoxCollider2D box = ray.GetComponent<BoxCollider2D>();
                ray.transform.position = this.transform.position + ray.transform.up.normalized * box.size.y * ray.transform.localScale.y * 0.5f;
                Vector3 mult = new Vector3(0.0f, ray.transform.localScale.y / 5.0f, 0.0f);
                ray.transform.position = ray.transform.position + ray.transform.up.normalized * box.size.y * (ray.transform.localScale.y / 10.0f);
                ray.transform.localScale = ray.transform.localScale + mult;
                rigid.AddForce(-this.transform.up * this.movespeed / 2.0f);
            }
        }
        private void Fire()
        {
            Bullet bullet = Instantiate(this.fab, this.transform.position, this.transform.rotation);
            bullet.Shoot(this.transform.up);
        }
        private void Ray()
        {
            if (ammo > 0)
            {
                this.ammo--;
                this.text.text = "Ammo: ";
                for (int i = 0; i < ammo; i++)
                    this.text.text = this.text.text + "[]";
                this.ray = Instantiate(this.ray_fab, this.transform.position, this.transform.rotation);
                ray.Shoot(this.transform.up);
                this.beam.transform.position = this.transform.position;
                this.beam.transform.rotation = this.transform.rotation;
                this.beam.Play();
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
            if (ammo < 3)
                this.ammo++;
            this.text.text = "Ammo: ";
            for (int i = 0; i < ammo; i++)
                this.text.text = this.text.text + "[]";
        }
    }

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
            rigid.mass = this.size + 2.0f;
        }
        public void Launch(Vector2 dir)
        {
            rigid.AddForce(dir * this.speed);
            Destroy(this.gameObject, this.maxLifeTime);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "Ray")
            {
                if (this.size / 2.0f >= this.minSize)
                {
                    for (int i = 0; i < this.size / this.minSize; i++)
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

    public class BadBullet : MonoBehaviour
    {
        public float speed = 1000.0f;
        public float lifetime = 1.5f;
        private Rigidbody2D rigid;
        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        public void Shoot(Vector2 dir)
        {
            rigid.AddForce(dir * this.speed);
            Destroy(this.gameObject, this.lifetime);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(this.gameObject);
        }
    }

    public class Bullet : MonoBehaviour
    {
        public float speed = 1000.0f;
        public float lifetime = 1.5f;
        private Rigidbody2D rigid;
        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        public void Shoot(Vector2 dir)
        {
            rigid.AddForce(dir * this.speed);
            Destroy(this.gameObject, this.lifetime);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(this.gameObject);
        }
    }

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
                rigid.AddTorque(turn / 90.0f * this.turnspeed);
            }
        }
        private void Fire()
        {
            BadBullet bullet = Instantiate(this.fab, this.transform.position, this.transform.rotation);
            bullet.Shoot(this.transform.up);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "Ray")
            {
                game.EnemyDestroyed();
                Destroy(this.gameObject);
            }
        }
    }

    public class Game : MonoBehaviour
    {
        public Text text, message;
        public ParticleSystem boom;
        public Player player, second_player;
        public Spawner spawner, second_spawner;
        public int score = 0, hs = 0;
        public void EnemyDestroyed()
        {
            spawner.total--;
            score += 100;
        }
        public void AsteroidDestroyed(Asteroid aster)
        {
            this.boom.transform.position = aster.transform.position;
            this.boom.Play();
            if (aster.size < 0.5f)
                score += (int)((1 / aster.size) * 100);
            else
            if (aster.size < 1.2f)
                score += (int)(aster.size * 50);
            else
                score += (int)(aster.size * 20);
            if (hs < score)
                hs = score;
            text.text = "Score: " + score + " HS: " + hs;
        }
        public void Died()
        {
            this.boom.transform.position = player.transform.position;
            this.boom.Play();
            var enemy = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemy.Length; i++)
                Destroy(enemy[i]);
            var aster = GameObject.FindGameObjectsWithTag("Asteroid");
            for (int i = 0; i < aster.Length; i++)
                Destroy(aster[i]);
            spawner.total = 0;
            spawner.gameOn = false;
            this.message.text = "Your Score is: " + score + "\nPress Space to start";
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!spawner.gameOn)
                {
                    second_player.gameObject.SetActive(false);
                    second_spawner.gameOn = false;
                    spawner.gameOn = true;
                    Respawn();
                }
            }
            if (Input.GetKeyDown(KeyCode.Tab) && spawner.gameOn)
            {
                second_player.gameObject.SetActive(true);
                second_spawner.gameOn = true;
                second_player.transform.position = player.transform.position;
                second_player.transform.rotation = player.transform.rotation;
                second_player.rigid.velocity = player.rigid.velocity;
                second_player.rigid.angularVelocity = player.rigid.angularVelocity;
                var buff_player = second_player;
                second_player = player;
                player = buff_player;
                var buff_spawner = second_spawner;
                second_spawner = spawner;
                spawner = buff_spawner;
                second_player.gameObject.SetActive(false);
                second_spawner.gameOn = false;
            }
        }
        void Respawn()
        {
            message.text = "";
            if (hs < score)
                hs = score;
            score = 0;
            text.text = "Score: " + score + " HS: " + hs;
            player.transform.position = Vector3.zero;
            player.gameObject.SetActive(true);
            player.ammo = 3;
            player.text.text = "Ammo: [][][]";
        }
    }
}
