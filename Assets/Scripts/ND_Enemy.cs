using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Enemy class :
/// Spawns at a random point on a circle of radius 5.0f (for now, but could be exported in variable)
/// Linked to delegate events SlowMo
/// Activated after a random range after being spawned
/// </summary>
public class ND_Enemy : MonoBehaviour {
    public int              m_ArchetypeID = 0;
    public uint             m_uHP = 1;                      //Health points
    public Color            m_DefaultColor;
    [SerializeField]
    public uint             m_uHPCurrent;
    public static float     m_fSlowModifierMax = 100.0f;    //The Speed and Time -SlowMotion ON- value, effect coefficient
    public float            m_fSpeed = 10.0f;               //Enemy Move Speed
    public bool             m_available = true;             //Enemy is available in the pool
    protected float           m_fSlowModifier = 1.0f;         //Current speed variable
    public int              m_iDashBonus = 0;
    public int              m_iScore = 0;

    private bool stopped = false;
    Vector3 stopPosition = Vector3.zero;
    private Animator animator;
    //public GameObject myDest;

    /// <summary>
    /// Gets a 3DVector on a circle of radius at angleDegrees 
    /// </summary>
    /// <param name="angleDegrees">The angle of the unit circle to get a point on (can be a random int between 0 and 360)</param>
    /// <param name="radius">The radius of the circle</param>
    private Vector3 GetOnUnitCircle(float angleDegrees, float radius)
    {
        // initialize calculation variables
        float _x = 0;
        float _y = 0;
        float angleRadians = 0;
        Vector3 _returnVector;
        // convert degrees to radians
        angleRadians = angleDegrees * Mathf.PI / 180.0f;
        // get the 2D dimensional coordinates
        _x = radius * Mathf.Cos(angleRadians);
        _y = radius * Mathf.Sin(angleRadians);
        // derive the 2D vector
        _returnVector = new Vector3(_x, 0.0f, _y);
        // return the vector info
        return _returnVector;
    }
    void Start() //Listen to SlowMo events and random positioning
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        GameEventManager.GameOver += GameOver;
        GameEventManager.Victory += Victory;
        //this.transform.position = GetOnUnitCircle(Random.Range(0, 360), 5.0f); //TODO Make random spawn mode
        this.transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);
        animator = gameObject.GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, Vector3.zero) <= Vector3.Distance(Vector3.zero, stopPosition) && !stopped)
        {
            stopped = true;
            //TODO REMOVE Debug destination oriented point
            //if (myDest != null)
            //{
            //    myDest.transform.parent = transform;
            //    myDest.transform.position = transform.parent.position;
            //}
            //Launch punch animation
            if(animator == null) animator = gameObject.GetComponentInChildren<Animator>();
            if (animator != null) animator.SetBool("IsAttack", true);
            Sequence mySequencePunch = DOTween.Sequence();
            mySequencePunch.Append(gameObject.transform.DOShakePosition(0.5f, 0.1f, 15, 10.0f)).AppendInterval(0.25f).AppendCallback(DamagePlayer);
        }
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            m_fSlowModifier = 3.56f / (m_fSpeed * m_fSlowModifierMax);
            if (animator != null)
                animator.speed = 0.01f;
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
            m_fSlowModifier = 3.56f / m_fSpeed;
            ResetColor();
            if (animator != null)
                animator.speed = 1.0f;
        }
    }
    public void Activate() //(Re)Activation by POOL
    {
        //this.transform.position = GetOnUnitCircle(Random.Range(0, 360), 5.0f); //TODO Make random spawn mode
        this.transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);

        gameObject.SetActive(true);

        if (animator == null) animator = gameObject.GetComponentInChildren<Animator>();
        if (animator != null) { animator.enabled = true; animator.SetBool("IsAttack", false); }

        m_uHPCurrent = m_uHP;
        stopped = false;
        StartCoroutine(ActivateEnemy(Random.Range(0, 0))); //TODO Make random spawn mode
        ResetColor();
    }
	private IEnumerator ActivateEnemy (float delay) { //Start moving after a random delay
        m_available = false;
        m_fSlowModifier = 3.56f / m_fSpeed;
        yield return new WaitForSeconds(delay);
        if (GameEventManager.GameSlowed)
        {
            SlowMoActivation();
        }
        stopPosition = Vector3.zero + Vector3.Normalize(transform.position - Vector3.zero)*0.5f;

        //TODO REMOVE Debug destination oriented point
        //if (myDest != null)
        //{
        //    myDest.transform.parent = myDest.transform.parent.transform.parent;
        //    myDest.transform.rotation = Quaternion.LookRotation(gameObject.transform.position);
        //    myDest.transform.position = stopPosition;
        //}
        //StartCoroutine (MoveOverSeconds (this.gameObject, Vector3.zero, m_fTime));
        StartCoroutine(MoveOverSpeed(this.gameObject, Vector3.zero, m_fSlowModifier));
	}
	private IEnumerator MoveOverSpeed (GameObject objectToMove, Vector3 end, float speed){
		// speed should be 1 unit per second
        while (objectToMove.transform.position != stopPosition && !stopped)
		{
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, m_fSlowModifier * Time.deltaTime);
			yield return new WaitForEndOfFrame ();
        }
	}
    private void DamagePlayer() //if arrived at destination
    {
        GameEventManager.TriggerPlayerDamage();
        Death();
    }
    public void Death() //BACK to the pool
    {
        StopAllCoroutines();
        if(!m_available)
        {
            ND_LevelEnemyRequestManager.enemyCount--;
        }
        m_available = true;
        this.gameObject.SetActive(false);
    }
    public void Damage()
    {
        if(m_uHPCurrent > 0)
        {
            m_uHPCurrent -= 1;
        }
    }
    public void RevertDamage()
    {
        m_uHPCurrent += 1;
    }
    public bool ShouldDie()
    {
        return m_uHPCurrent <= 0;
    }
    public void CheckDeath()
    {
        if (ShouldDie())
        {
            ResetColor();

            if (gameObject.GetComponent<ND_DismemberManager>() != null)
            {
                gameObject.GetComponent<ND_DismemberManager>().ResetSkelton();
            }
            Death();
        }
    }
    void GameOver()
    {
        if (this != null)
        {
            ResetColor();
            Death();
        }
    }
    void Victory()
    {
        if (this != null)
        {
            ResetColor();
            Death();
        }
    }
    public int GetScoreValue()
    {
        return m_iScore;
    }

    public void ResetColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.SetColor("_Color", m_DefaultColor);//(c.r + 70.0f / 255, c.g - 20.0f / 255, c.b - 40.0f / 255));
        }
    }

    public bool HasShieldComponent()
    {
       ND_EnemyShield shieldComponent = gameObject.GetComponent<ND_EnemyShield>();
       return shieldComponent != null;
    }
    public bool CanBeTargetted(Vector3 _from)
    {
        ND_EnemyShield shieldComponent = gameObject.GetComponent<ND_EnemyShield>();
        if(shieldComponent != null)
        {
            return true;// shieldComponent.CanBeTargetted(_from);
        }
        else
        {
            return true;
        }
    }
}
