using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;


    public int life;
    public int score;
    public float speed;
    public float power;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;

    public GameManager gameManager;
    public bool isHit;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Reload();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {
        Rigidbody2D rb, rbR, rbL, rbC;
        GameObject bulletR, bulletL, bulletC;
        if (!Input.GetButtonDown("Fire1")) return;
        if (curShotDelay < maxShotDelay) return;

        switch (power)
        {
            case 1:
                GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);
                rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
                bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation);
                rbR = bulletR.GetComponent<Rigidbody2D>();
                rbL = bulletL.GetComponent<Rigidbody2D>();
                rbR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rbL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.35f, transform.rotation);
                bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.35f, transform.rotation);
                bulletC = Instantiate(bulletObjB, transform.position, transform.rotation);
                rbR = bulletR.GetComponent<Rigidbody2D>();
                rbL = bulletL.GetComponent<Rigidbody2D>();
                rbC = bulletC.GetComponent<Rigidbody2D>();
                rbR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rbL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rbC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }

        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit) return;
            
            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            if (life <= 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
}
