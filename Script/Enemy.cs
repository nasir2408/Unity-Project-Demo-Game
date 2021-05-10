using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    private SceneChanger sceneChanger;
    private IEnemyState currentState;

    public bool dropItem = true;

    public GameObject Target {get; set;}

    [SerializeField] private float meleeRange;

    [SerializeField] private float throwRange;

    [SerializeField] private Transform leftEdge;

    [SerializeField] private Transform rightEdge;

    public bool isBossDead;

    private GameObject boss;

    private Vector3 startpos;

    private Canvas healthCanvas;

    public bool InMeleeRange
    {
        get
        {
            if(Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }

            return false;
        }
    }

    public bool InThrowRange
    {
        get
        {
            if(Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
            }

            return false;
        }
    }

    public override bool IsDeath
    {
        get
        {
            return healthStat.CurrentValue <= 0; // if health is less then equal to 0 then it will return IsDeath = true else it will return false.
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        // Calls the base start.
        base.Start();

        this.startpos = transform.position;

        boss = GameObject.FindWithTag("Boss");
        
        //Makes the Removetarget function listen to the player's Dead event
        Player.Instance.Dead += new DeathEventHandler(RemoveTarget);

        //Sets the Enemy in Idle State 
        ChangeState(new IdleState());

        healthCanvas = transform.GetComponentInChildren<Canvas>();
        sceneChanger = GameObject.FindObjectOfType<SceneChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsDeath)
        {
            if(!TakingDamage)
            {
                currentState.Execute();
            }
            
            LookAtTarget();
        }    
    }

    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new PatrolState());
    }

    private void LookAtTarget()
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;

            if (xDir < 0 && facingRight || xDir > 0 && !facingRight)
            {
                ChangeDirection();
            }
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        if(currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        currentState.Enter(this);
    }

    public void Move()
    {
        if(!Attack)
        {
            if((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
            {
                // Sets the speed to 1 to play the move animation
                MyAnimator.SetFloat("speed", 1);

                // Move the enemy in the correct direction
                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
            }
            else if (currentState is PatrolState)
            {
                ChangeDirection();
            }
            else if (currentState is RangedState)
            {
                Target = null;
                ChangeState(new IdleState());
            }
            
        }
    }

    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left; // Its a kind of if statement, if facingRight is true it will return right direction or otherwise.
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other); // Other is a collider2D.
        currentState.OnTriggerEnter(other);
    }

    public override IEnumerator TakeDamage()
    {
        if(!healthCanvas.isActiveAndEnabled)
        {
            healthCanvas.enabled = true;
        }

        //Reduce the health by 10
        healthStat.CurrentValue -= 10;

        if (!IsDeath) // Enemy isn't dead then play the damage anmimation.
        {
            MyAnimator.SetTrigger("damage");
        }
        else // else if Enemy is dead play dead animation.
        {
            if(dropItem)
            {
                GameObject coin = (GameObject)Instantiate(GameManager.Instance.CoinPrefab, new Vector3(transform.position.x,transform.position.y), Quaternion.identity);
                Physics2D.IgnoreCollision(coin.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                dropItem = false;
            }
            

            MyAnimator.SetTrigger("die");
            yield return null; // So that this IEnumerator is Satisfied.
        }
    }

    public override void Death()
    {
        if(gameObject.tag  == "Enemy")
        {
            dropItem = true;
            MyAnimator.ResetTrigger("die");
            MyAnimator.SetTrigger("idle");
            healthStat.CurrentValue = healthStat.MaxValue;
            transform.position = startpos;
        
            healthCanvas.enabled = false;
        }
        else if(gameObject.tag == "Boss")
        {
            Destroy(gameObject);
            isBossDead = true;
        }
        //Destroy(gameObject);
        
    }

    public override void ChangeDirection()
    {
        Transform tmp = transform.Find("EnemyHealthBarCanvas").transform;
        Vector3 pos = tmp.position;
        tmp.SetParent(null);

        base.ChangeDirection();

        tmp.SetParent(transform);
        tmp.position = pos;
    }
}
