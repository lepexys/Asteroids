using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool gameOn = false;
    public Asteroid fab;
    public Enemy enemy_fab;
    public float trajectoryVariance = 10.0f;
    public float spawnRate = 1.0f,spawnEnemyRate = 5.0f;
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
