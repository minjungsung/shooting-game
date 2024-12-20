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
    public int power;
    public int boom;
    public int maxPower;
    public int maxBoom;
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;

    public GameManager gameManager;
    public ObjectManager objectManager;

    public bool isHit;
    public bool isBoomTime;
    public GameObject[] followers;
    public bool isRespawnTime;
    public bool[] joyControl;
    public bool isControl;
    public bool isButtonA;
    public bool isButtonB;

    Animator anim;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable", 3f);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;

        if (isRespawnTime)
        {
            isRespawnTime = true;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            isRespawnTime = false;
            spriteRenderer.color = new Color(1, 1, 1, 1f);
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            }
        }
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    public void JoyPanel(int type)
    {
        for (int index = 0; index < joyControl.Length; index++)
        {
            joyControl[index] = index == type;
        }
    }

    public void JoyDown()
    {
        isControl = true;
    }

    public void JoyUp()
    {
        isControl = false;
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (joyControl[0])
        {
            h = -1;
            v = 1;
        }
        else if (joyControl[1])
        {
            h = 1;
            v = 1;
        }
        else if (joyControl[2])
        {
            h = 0;
            v = 1;
        }
        else if (joyControl[3])
        {
            h = -1;
            v = 0;
        }
        else if (joyControl[4])
        {
            h = 0;
            v = 0;
        }
        else if (joyControl[5])
        {
            h = 1;
            v = 0;
        }
        else if (joyControl[6])
        {
            h = -1;
            v = -1;
        }
        else if (joyControl[7])
        {
            h = 0;
            v = -1;
        }
        else if (joyControl[8])
        {
            h = 1;
            v = -1;
        }

        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1) || !isControl)
            h = 0;

        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1) || !isControl)
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    public void ButtonADown()
    {
        isButtonA = true;
    }

    public void ButtonAUp()
    {
        isButtonA = false;
    }

    public void ButtonBDown()
    {
        isButtonB = true;
    }

    void Fire()
    {
        // if (!Input.GetButtonDown("Fire1")) return;

        if (!isButtonA)
        {
            return;
        }

        if (curShotDelay < maxShotDelay) return;

        switch (power)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;

                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rbR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rbL = bulletL.GetComponent<Rigidbody2D>();

                rbR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rbL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;

                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;

                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;

                Rigidbody2D rbRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rbLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rbCC = bulletCC.GetComponent<Rigidbody2D>();

                rbRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rbLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rbCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        // if (!Input.GetButtonDown("Fire2")) return;
        if (!isButtonB) return;
        if (boom <= 0) return;
        if (isBoomTime) return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 3f);

        GameObject[] enemyL = objectManager.GetPool("EnemyL");
        GameObject[] enemyM = objectManager.GetPool("EnemyM");
        GameObject[] enemyS = objectManager.GetPool("EnemyS");
        foreach (GameObject enemy in enemyL)
        {
            if (enemy.activeSelf)
            {
                Enemy enemyLogic = enemy.GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        foreach (GameObject enemy in enemyM)
        {
            if (enemy.activeSelf)
            {
                Enemy enemyLogic = enemy.GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        foreach (GameObject enemy in enemyS)
        {
            if (enemy.activeSelf)
            {
                Enemy enemyLogic = enemy.GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        foreach (GameObject bullet in bulletsA)
        {
            if (bullet.activeSelf)
            {
                bullet.SetActive(false);
            }
        }
        foreach (GameObject bullet in bulletsB)
        {
            if (bullet.activeSelf)
            {
                bullet.SetActive(false);
            }
        }
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
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {

            if (isRespawnTime) return;
            if (isHit) return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if (life <= 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power >= maxPower)
                    {
                        score += 500;
                    }
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom >= maxBoom)
                    {
                        score += 500;
                    }
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void AddFollower()
    {
        if (power == 4)
        {
            followers[0].SetActive(true);
        }
        else if (power == 5)
        {
            followers[1].SetActive(true);
        }
        else if (power == 6)
        {
            followers[2].SetActive(true);
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
