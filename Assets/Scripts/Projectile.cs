using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Arrow = 100,
    SlimeMisile = 300,
}

public class Projectile : PoolObject
{
    private UnitBase owner;
    private UnitBase target;
    private float speed;
    private float damage;
    private ProjectileType type;

    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

    public void Init(UnitBase owner, UnitBase target, float speed, float damage)
    {

        this.owner = owner;
        this.target = target;
        this.speed = speed;
        this.damage = damage;

        type = (ProjectileType)this.owner.CurrentUnitData.id;
        SetSprite();
        SetTrail();

        StartCoroutine("Misile");
    }

    private void SetTrail()
    {
        if (!trailRenderer) trailRenderer = GetComponentInChildren<TrailRenderer>();
        if (trailRenderer) trailRenderer.enabled = false;
    }

    private void SetSprite()
    {
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.sprite =
                Resources.Load<Sprite>(DataManager.Instance.GetProjectileData(owner.CurrentUnitData.id).spriteSrc);
    }


    private void Update()
    {
        if (type == ProjectileType.Arrow)
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

    }

    private IEnumerator Misile()
    {

        if (type == ProjectileType.SlimeMisile)
        {
            trailRenderer.enabled = true;
            float time = 0;

            var y = target.transform.position.y + Random.Range(0.2f, 0.7f);
            Vector3 P1 = new Vector3(Random.Range(-1.5f, 1.5f), y, 0);
            Vector3 P2 = new Vector3(Random.Range(-1.5f, 1.5f), y, 0);

            while (time <= 1f)
            {
                transform.position = Bezier(transform.position, P1, P2, target.transform.position, time);
                time += Time.deltaTime * speed;
                yield return null;
            }
            trailRenderer.enabled = false;
            target.GetDamage(damage);
            ReturnToPool();
        }
        yield return null;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == ProjectileType.Arrow)
        {
            if (collision.TryGetComponent<UnitBase>(out UnitBase unit))
            {
                if (unit == target)
                {
                    target.GetDamage(damage);
                    ReturnToPool();
                }
            }
        }
    }


    public Vector3 Bezier(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
    {
        var M0 = Vector3.Lerp(P0, P1, t);
        var M1 = Vector3.Lerp(P1, P2, t);
        var M2 = Vector3.Lerp(P2, P3, t);

        Vector3 B0 = Vector3.Lerp(M0, M1, t);
        Vector3 B1 = Vector3.Lerp(M1, M2, t);

        return Vector3.Lerp(B0, B1, t);
    }
}
