using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    public void Awake()
    {
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        parentPos.Enqueue(parent.position);
        Debug.Log("parentPos.Count : " + parentPos.Count);
        Debug.Log("followDelay : " + followDelay);
        if (parentPos.Count > followDelay)
        {
            followPos = parentPos.Dequeue();
            Debug.Log("followPos : " + followPos);
        }
    }

    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (!Input.GetButtonDown("Fire1")) return;
        if (curShotDelay < maxShotDelay) return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        curShotDelay = 0;
    }


    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

}
