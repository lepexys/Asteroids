using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text text,message;
    public ParticleSystem boom;
    public Player player,second_player;
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
        text.text = "Score: " + score+" HS: "+hs;
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
        this.message.text = "Your Score is: "+ score + "\nPress Space to start";
    }
    void Update()
    { 
        if(Input.GetKeyDown(KeyCode.Space))
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
