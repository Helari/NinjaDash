using UnityEngine;
using System.Collections;

/// <summary>
/// Enemy class :
/// Spawns at a random point on a circle of radius 5.0f (for now, but could be exported in variable)
/// Linked to delegate events SlowMo
/// Activated after a random range after being spawned
/// </summary>
public class ND_Enemy : MonoBehaviour {

    public uint             m_uHP = 1;                      //Health points
    public Color            m_DefaultColor;
    [SerializeField]
    public uint             m_uHPCurrent;
    public static float     m_fSlowModifierMax = 100.0f;    //The Speed and Time -SlowMotion ON- value, effect coefficient
    public float            m_fSpeed = 10.0f;               //Enemy Move Speed
    public bool             m_available = true;             //Enemy is available in the pool
    private float           m_fSlowModifier = 1.0f;         //Current speed variable

    /// <summary>
    /// Gets a 3DVector on a circle of radius at angleDegrees 
    /// </summary>
    /// <param name="angleDegrees">The angle of the unit circle to get a point on (can be a random int between 0 and 360)</param>
    /// <param name="radius">The radius of the circle</param>
    public Vector3 GetOnUnitCircle(float angleDegrees, float radius)
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
        this.transform.position = GetOnUnitCircle(Random.Range(0, 360), 5.0f);
        this.transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            m_fSlowModifier = 3.56f / (m_fSpeed * m_fSlowModifierMax);
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
            m_fSlowModifier = 3.56f / m_fSpeed;
        }
    }
    public void Activate() //(Re)Activation by POOL
    {
        this.transform.position = GetOnUnitCircle(Random.Range(0, 360), 5.0f);
        this.transform.rotation = Quaternion.LookRotation(Vector3.zero - transform.position);
        gameObject.SetActive(true);
        m_uHPCurrent = m_uHP;
        StartCoroutine(ActivateEnemy(Random.Range(0, 5)));
    }
	public IEnumerator ActivateEnemy (float delay) { //Start moving after a random delay
        m_available = false;
        m_fSlowModifier = 3.56f / m_fSpeed;
        yield return new WaitForSeconds(delay);
        if (GameEventManager.GameSlowed)
        {
            SlowMoActivation();
        }
        //StartCoroutine (MoveOverSeconds (this.gameObject, Vector3.zero, m_fTime));
        StartCoroutine(MoveOverSpeed(this.gameObject, Vector3.zero, m_fSlowModifier));
	}
	public IEnumerator MoveOverSpeed (GameObject objectToMove, Vector3 end, float speed){
		// speed should be 1 unit per second
		while (objectToMove.transform.position != end)
		{
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, m_fSlowModifier * Time.deltaTime);
			yield return new WaitForEndOfFrame ();
        }
        DamagePlayer();
	}
    private void DamagePlayer() //if arrived at destination
    {
        GameEventManager.TriggerPlayerDamage();
        Death();
    }
    public void Death() //BACK to the pool
    {
        StopAllCoroutines();
        m_available = true;
        this.gameObject.SetActive(false);
    }
    public void Damage()
    {
        m_uHPCurrent -= 1;
    }
    public void CheckDeath()
    {
        if (m_uHPCurrent <= 0)
        {
            ResetColor();
            Death();
        }
    }
    public void ResetColor()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.SetColor("_Color", m_DefaultColor);//(c.r + 70.0f / 255, c.g - 20.0f / 255, c.b - 40.0f / 255));
        }
    }
}
