using UnityEngine;

public class Shell : WarEntity
{
    float age, blastRadius, damage;

    Vector3 launchPoint, targetPoint, launchVelociy;

    public void Initialize (
        Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity,
        float blastRadius, float damage
    )
    {
        this.launchPoint = launchPoint;
        this.targetPoint = targetPoint;
        this.launchVelociy = launchVelocity;
        this.blastRadius = blastRadius;
        this.damage = damage;
    }

    public override bool GameUpdate()
    {
        age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelociy * age;
        p.y -= 0.5f * 9.81f * age * age;

        if (p.y <= 0f)
        {
            Game.SpawnExplosion().Initialize(targetPoint, blastRadius, damage);
            OriginFactory.Reclaim(this);
            return false;
        }

        transform.localPosition = p;

        Vector3 d = launchVelociy;
        d.y -= 9.81f * age;
        transform.localRotation = Quaternion.LookRotation(d);
        Game.SpawnExplosion().Initialize(p, 0.1f);

        return true;
    }
}
