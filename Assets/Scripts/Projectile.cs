using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolObject
{
    private UnitBase owner;
    private UnitBase target;
    private float speed;
    private float damage;

    public void Init(UnitBase owner, UnitBase target, float speed, float damage)
    {
        this.owner = owner;
        this.target = target;
        this.speed = speed;
        this.damage = damage;
    }

    private void Update()
    {

        var dir = target.transform.position - transform.position;
        dir.z = 0;
        dir.Normalize();
        
        // 이동
        transform.position += Time.deltaTime * speed * dir;
        // 바라보는방향
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);

        if (!target.dead) return;

        if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
            ReturnToPool();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<UnitBase>(out UnitBase unit))
        {
            if(unit == target)
            {
                target.GetDamage(damage);
                ReturnToPool();
            }
        }
    }
}
