using UnityEngine;
using System.Collections;

public class ND_Player : MonoBehaviour {

    public float m_fSlowMotionRecastTime = 3.0f;

    public ND_DashPath m_DashPath;
    public Renderer m_LoadingMaterial;
    public bool m_bSlowMotionInProgress = false;
    public bool m_bSlowMotionReady = true;
    private bool m_bSlowMotionReloading = false;
    public int m_iCurrentHP = 5;
    public int m_iDefaultDashCount = 5;

    float reloadElapsed = 0.0f;

	// Use this for initialization
    void Start()
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        GameEventManager.DamagePlayer += GetDamage;
        if (m_DashPath == null)
        {
            m_DashPath = GameObject.Find("DashPath").GetComponent<ND_DashPath>();//gameObject.GetComponent<ND_DashPath>();
        }

        m_LoadingMaterial.material.SetFloat("_Cutoff", 0.001f);
	}
	
    private RuntimePlatform m_Platform = Application.platform;

    void Update()
    {
        if (m_bSlowMotionReady && Time.timeScale != 0)
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
        if(m_bSlowMotionReloading)
        {
            if(reloadElapsed < m_fSlowMotionRecastTime)
            {
                reloadElapsed += Time.deltaTime;

                m_LoadingMaterial.material.SetFloat("_Cutoff", 1.015f-reloadElapsed/m_fSlowMotionRecastTime);
            }
            //float i = 0.0f;
            //float rate = 1.0f / m_fSlowMotionRecastTime;
            //while (i < 1.0f)
            //{
            //    Debug.Log(i);
            //    i += Time.deltaTime * rate;
            //    float test = Mathf.Lerp(0.99f, 0.01f, i);
            //}
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
                m_DashPath.TryAddNewTarget(hit.transform.gameObject);
            }
            else if (hit.transform.gameObject.CompareTag("DashPath") && m_bSlowMotionInProgress)
            {
                m_DashPath.RemoveLastLine();
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
            m_LoadingMaterial.material.SetFloat("_Cutoff", 1.0f);
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
           // this.transform.position = new Vector3(0.33802f, 0.17451f, -0.3f);//Vector3.zero; Done in DashPath.cs at the end of Dash
            m_bSlowMotionInProgress = false;
            m_bSlowMotionReady = false;
            m_bSlowMotionReloading = true;
            m_LoadingMaterial.material.SetFloat("_Cutoff", 1.0f);
            reloadElapsed = 0.0f;
            StartCoroutine("resetSlowMotionDelay");
        }
    }
    IEnumerator resetSlowMotionDelay()
    {
        yield return new WaitForSeconds(m_fSlowMotionRecastTime);
        m_bSlowMotionReady = true;
        m_bSlowMotionReloading = false;
    }
}
