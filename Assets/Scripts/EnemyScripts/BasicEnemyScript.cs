using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyScript : MonoBehaviour
{
    public Rigidbody2D rb2D;
    UpdateUI uiScript;
    GameObject player;
    
    [SerializeField]
    private int HP=1;

    public GameObject RDrop1;
    public GameObject RDrop2;
    public GameObject RDrop3;

    [SerializeField]
    private AudioSource DeadExplosion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiScript = GameObject.Find("Canvas").GetComponent<UpdateUI>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 1f * Time.deltaTime);
        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet" || collision.tag == "BigBullet")
        {
            if (collision.tag == "Bullet")
            {
                collision.gameObject.SetActive(false);
                HP--;
            }
            if (collision.tag == "BigBullet")
            {
                HP -= 6;
            }
            if (HP <= 0)
            {
                uiScript.AddScore(2);
                int RandomDrop = Random.Range(1, 101);
                if (RandomDrop <= 10)
                {
                    int RandomPowerup = Random.Range(1, 4);
                    if (RandomPowerup == 1)
                    Instantiate(RDrop1, this.transform.position, Quaternion.identity);
                    if (RandomPowerup == 2)
                    Instantiate(RDrop2, this.transform.position, Quaternion.identity);
                    if (RandomPowerup == 3)
                    Instantiate(RDrop3, this.transform.position, Quaternion.identity);
                }
                DeadExplosion.Play();
                gameObject.SetActive(false);
            }
        }
    }
}
