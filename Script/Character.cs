using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    // protected Animator myAnimator;

    [SerializeField] protected float movementSpeed;

    [SerializeField] protected bool facingRight;

    [SerializeField] protected GameObject kunaiPrefab;

    [SerializeField] protected Transform kunaipos;

    //[SerializeField] protected int health;

    [SerializeField] protected Stat healthStat;

    [SerializeField] private EdgeCollider2D swordCollider;

    [SerializeField] private List<string> damageSources;

    public abstract bool IsDeath {get;}

    public bool Attack {get; set;}

    public Animator MyAnimator {get; private set;} //making myAnimator a property from variable.

    public bool TakingDamage { get; set; }
    public EdgeCollider2D SwordCollider { get => swordCollider;}

    // Start is called before the first frame update
    public virtual void Start()
    {
        //facingRight = true;

        MyAnimator = GetComponent<Animator>();

        healthStat.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract IEnumerator TakeDamage();

    public abstract void Death();

    public virtual void ChangeDirection ()
    {
        facingRight = !facingRight;

        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

    public virtual void ThrowKunai(int value)
    {
        if(facingRight)
        {
            GameObject tmp = (GameObject)Instantiate(kunaiPrefab, kunaipos.position, Quaternion.Euler(new Vector3(0, 0 , -90)));

            tmp.GetComponent<Kunai>().Initialize(Vector2.right);
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(kunaiPrefab, kunaipos.position, Quaternion.Euler(new Vector3(0, 0 , 90)));
            
            tmp.GetComponent<Kunai>().Initialize(Vector2.left);
        }
    }

    public void MeleeAttack()
    {
        SwordCollider.enabled = true;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
        }
    }


    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}