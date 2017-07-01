using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIBasic : AbstractStateComponent 
{
    public EnemyAIControlComponent AIControlComponent;
    public float coefficientStun = 1f;
    public bool isDead = false;
    public float health = 500;
    public float currentHealth;
    public Rigidbody _rigidbody;
    public float maxSpeed = 6;

    public AIControlComponentType aIControlComponentType = AIControlComponentType.BearTactics;
    public List<BigGunAbstract> bigGunList = new List<BigGunAbstract>();
    public List<RocketAbstract> rocketList = new List<RocketAbstract>();
    public List<AutoTurretAbstract> autoTurretList = new List<AutoTurretAbstract>();

    void Awake()
    {
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
        bigGunList.AddRange(this.gameObject.GetComponentsInChildren<BigGunAbstract>());
        rocketList.AddRange(this.gameObject.GetComponentsInChildren<RocketAbstract>());
        autoTurretList.AddRange(this.gameObject.GetComponentsInChildren<AutoTurretAbstract>());

        ChangeAIControlComponent(aIControlComponentType);

        AbstractStateComponent[] ascList = this.gameObject.GetComponentsInChildren<AbstractStateComponent>();
        foreach (AbstractStateComponent asc in ascList)
        {
            asc.gameObject.tag = "Enemy";
        }
        this.gameObject.tag = "EnemyBasis";
    }

    public void ChangeAIControlComponent(AIControlComponentType _aIControlComponentType)
    {
        Destroy(AIControlComponent);
        switch (_aIControlComponentType)
        {
            case AIControlComponentType.TortoiseTactics:
                AIControlComponent = this.gameObject.AddComponent<TortoiseTactics>();
                break;
            case AIControlComponentType.BearTactics:
                AIControlComponent = this.gameObject.AddComponent<BearTactics>();
                break;
            case AIControlComponentType.Boss1Tactics:
                AIControlComponent = this.gameObject.AddComponent<Boss1Tactics>();
                break;
            case AIControlComponentType.TurretTactics:
                AIControlComponent = this.gameObject.AddComponent<TurretTactics>();
                break;
        }
        AIControlComponent.AIBasic = this;
        aIControlComponentType = _aIControlComponentType;
    }

    public override void Sleep(float duration)
    {
        base.Sleep(duration * coefficientStun);
    }

    void FixedUpdate()
    {
        if (!base.BasicUpdate())
            return;

        if (_rigidbody.velocity.magnitude > maxSpeed)
        {
            _rigidbody.velocity = this.gameObject.GetComponent<Rigidbody>().velocity.normalized * maxSpeed;
        }
        AIControlComponent.PerformActions();

        {
            List<AutoTurretAbstract> readyPointDefenseList = new List<AutoTurretAbstract>();
            List<Collider> enemyRocketList = new List<Collider>();

            foreach (AutoTurretAbstract sg in autoTurretList)
            {
                if ((sg is AutoPointDefenseScript))
                {
                    if (sg.GetTimeLeftToRecharge() == 0)
                    {
                        readyPointDefenseList.Add(sg);
                    }
                }
            }

            Collider[] colliders = Physics.OverlapSphere(this.gameObject.transform.position, 30);
            for (int i = 0; i < colliders.Length; i++)
            {
                if ((colliders[i].gameObject.transform.position.y > -5) &&
                    (colliders[i].gameObject.tag == "SteamcopterRocket") &&
                    (!colliders[i].gameObject.GetComponent<RocketScript>().underTheSight))
                {
                    enemyRocketList.Add(colliders[i]);
                }
            }

            foreach (AutoTurretAbstract ata in readyPointDefenseList)
            {
                if (enemyRocketList.Count == 0)
                {
                    continue;
                }
                enemyRocketList[0].gameObject.GetComponent<RocketScript>().underTheSight = true;
                ata.OpenFire(enemyRocketList[0].gameObject);
                enemyRocketList.RemoveAt(0);
            }
        }
    }

    public override void Destroy()
    {
        if (aIControlComponentType != AIControlComponentType.TurretTactics)
        {
            ManagerAIEnemies.Instance.RemoveEnemyAIBasic(this);
        }
        EventOnFieldManager.Instance.VisualizeDestructionOfEnemy(this.gameObject);
        EventOnFieldManager.Instance.CreateDebris(this.gameObject);

        PlayerInfoManager.KilledEnemies += 1;
        AIControlComponent.Destroy();
        Destroy(this.gameObject);
    }

    public override void DealDamage(float Damage)
    {
        currentHealth -= Damage;
        if (currentHealth <= health / 2)
            VisualizeLowHPEffect();
        if (currentHealth <= 0)
        {
            if (!isDead)
            {
                Destroy();
                isDead = true;
            }
        }
    }

    public void ShootBigGun(Vector3 Direction)
    {
        Collider _collider = this.gameObject.GetComponent<BoxCollider>();
        foreach (BigGunAbstract bg in bigGunList)
        {
            if (bg == null)
                continue;

            RaycastHit[] raycastHitList = Physics.RaycastAll(bg.gameObject.transform.position, (Direction - bg.gameObject.transform.position), 40f);
            bool flag = true;
            for (int i = 0; i < raycastHitList.Length; i++)
            {
                if (((raycastHitList[i].collider.gameObject.tag == "Enemy") ||
                    (raycastHitList[i].collider.gameObject.tag == "EnemyBasis")) &&
                    (raycastHitList[i].collider != _collider))
                {
                    flag = false;
                    break;                
                }
            }
            if (flag)
                bg.OpenFire(Direction);
        }
    }

    public void ShootRocket(Vector3 Direction)
    {
        foreach (RocketAbstract r in rocketList)
        {
            r.OpenFire(Direction);
        }
    }

    void Start()
    {
        if (aIControlComponentType != AIControlComponentType.TurretTactics)
        {
            ManagerAIEnemies.Instance.AddEnemyAIBasic(this);
        }
        currentHealth = health;
    }
}
