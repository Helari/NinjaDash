using UnityEngine;
using System.Collections;

public class ND_Player : MonoBehaviour {

    public float m_fSlowMotionRecastTime = 3.0f;

    private ND_DashPath m_DashPath;
    public bool m_bSlowMotionInProgress = false;
    public bool m_bSlowMotionReady = true;
    public int m_iCurrentHP = 5;
		
	// Use this for initialization
    void Start()
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        GameEventManager.DamagePlayer += GetDamage;
        m_DashPath = gameObject.GetComponent<ND_DashPath>();
	}
	
    private RuntimePlatform m_Platform = Application.platform;

    void Update()
    {
        if (m_bSlowMotionReady)
        {
            if (m_Platform == RuntimePlatform.Android || m_Platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        checkTouch(Input.GetTouch(0).position);
                    }
                }
            }
            else if (m_Platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    checkTouch(Input.mousePosition);
                }
            }
        }
    }

    void checkTouch(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                trySlowMotion();
            }
            else if (hit.transform.gameObject.CompareTag("Enemy") && m_bSlowMotionInProgress)
            {
                m_DashPath.AddNewTarget(hit.transform.gameObject);
            }
        }
    }

    void trySlowMotion()
    {
        if (!m_bSlowMotionInProgress)
        {
            GameEventManager.TriggerSlowMotionState_Begin();
        }
        else
        {
            m_DashPath.StartDash();
        }
    }
    void GetDamage()
    {
        if (this != null)
        {
            m_iCurrentHP--;
        }
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            m_bSlowMotionInProgress = true;
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
           // this.transform.position = new Vector3(0.33802f, 0.17451f, -0.3f);//Vector3.zero; Done in DashPath.cs at the end of Dash
            m_bSlowMotionInProgress = false;
            m_bSlowMotionReady = false;
            StartCoroutine("resetSlowMotionDelay");
        }
    }
    IEnumerator resetSlowMotionDelay()
    {
        yield return new WaitForSeconds(m_fSlowMotionRecastTime);
        m_bSlowMotionReady = true;
    }
}
