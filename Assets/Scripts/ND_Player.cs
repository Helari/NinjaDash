using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class ND_Player : MonoBehaviour {

    private RuntimePlatform m_Platform = Application.platform;
    public float m_fSlowMotionRecastTime = 3.0f;

    public ND_DashPath m_DashPath;
    public ND_PlayerAnims m_PlayerAnims;
    public Renderer m_LoadingMaterial;
    public bool m_bSlowMotionInProgress = false;
    public bool m_bSlowMotionReady = true;
    private bool m_bSlowMotionReloading = false;
    private bool m_bHitPunchInProgress = false;
    private bool m_bHitPunchReloading = false;
    public int m_iHP = 5;
    private int m_iCurrentHP = 5;
    public Text m_HPText;
    public int m_iDefaultDashCount = 5;

    private float reloadElapsed = 0.0f;

	// Use this for initialization
    void Start()
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        GameEventManager.DamagePlayer += GetDamage;
        GameEventManager.GameStart += GameStart;
        if (m_DashPath == null)
        {
            m_DashPath = GameObject.Find("DashPath").GetComponent<ND_DashPath>();//gameObject.GetComponent<ND_DashPath>();
        }
        if (m_PlayerAnims == null)
        {
            m_PlayerAnims = gameObject.GetComponentInChildren<ND_PlayerAnims>();
        }
	}

    void Update()
    {
        if (/*m_bSlowMotionReady && */Time.timeScale != 0)
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
            else //if (m_Platform == RuntimePlatform.WindowsEditor)
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

                m_LoadingMaterial.material.SetFloat("_Cutoff", 1.02f-reloadElapsed/m_fSlowMotionRecastTime);
            }
            else
            {
                m_bSlowMotionReady = true;
                m_bSlowMotionReloading = false;
                m_LoadingMaterial.material.SetFloat("_Cutoff", 0.001f);
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
            if (hit.transform.gameObject.CompareTag("Player") && m_bSlowMotionReady && !m_bHitPunchInProgress)
            {
                trySlowMotion();
            }
            if(m_bSlowMotionInProgress)
            {
                if (hit.transform.gameObject.CompareTag("Enemy") || hit.transform.gameObject.CompareTag("Shield"))
                {
                    m_DashPath.TryAddNewTarget(hit.transform.gameObject);
                }
                else if (hit.transform.gameObject.CompareTag("DashPath"))
                {
                    m_DashPath.RemoveLastLine();
                }
            }
            else if (!m_bSlowMotionInProgress && (hit.transform.gameObject.CompareTag("Enemy") || hit.transform.gameObject.CompareTag("Shield")) && !m_bHitPunchReloading)
            {
                PunchEnemy(hit.transform.gameObject);
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
    void GameStart()
    {
        if (this != null)
        {
            StopAllCoroutines();
            m_bSlowMotionReady = false;
            m_bSlowMotionReloading = true;
            m_LoadingMaterial.material.SetFloat("_Cutoff", 1.0f);
            reloadElapsed = 0.0f;
            m_iCurrentHP = m_iHP;
            m_HPText.text = "HP : " + m_iCurrentHP.ToString();
        }
    }
    void GetDamage()
    {
        if (this != null)
        {
            m_iCurrentHP--;
            m_HPText.text = "HP : " + m_iCurrentHP.ToString();
            if(m_iCurrentHP <= 0)
            {
                m_PlayerAnims.DeathAnim(true);
                StartCoroutine("WaitB4DeathAnim");
            }
        }
    }
    private IEnumerator WaitB4DeathAnim()
    {
        yield return new WaitForSeconds(2);
        m_PlayerAnims.DeathAnim(false);
        GameEventManager.TriggerGameOver();
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            m_bSlowMotionInProgress = true;
            m_PlayerAnims.SlowMoAnim(true);
            //m_LoadingMaterial.material.SetFloat("_Cutoff", 1.0f);
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
           // this.transform.position = new Vector3(0.33802f, 0.17451f, -0.3f);//Vector3.zero; Done in DashPath.cs at the end of Dash
            m_bSlowMotionInProgress = false;
            m_PlayerAnims.SlowMoAnim(false);
            //StartCoroutine("resetSlowMotionDelay");
        }
    }
    public void ReloadDash()
    {
        m_bSlowMotionReady = false;
        m_bSlowMotionReloading = true;
        m_LoadingMaterial.material.SetFloat("_Cutoff", 1.0f);
        reloadElapsed = 0.0f;
    }
    //IEnumerator resetSlowMotionDelay()
    //{
        //yield return new WaitForSeconds(m_fSlowMotionRecastTime);
        //m_bSlowMotionReady = true;
        //m_bSlowMotionReloading = false;
    //}
    private void PunchEnemy(GameObject enemy)
    {
        ND_Enemy enemyComp = enemy.GetComponent<ND_Enemy>();
        if (enemyComp == null)
        {
            enemyComp = enemy.transform.parent.gameObject.GetComponent<ND_Enemy>();
        }
        if (enemyComp != null)
        {
            m_PlayerAnims.JumpBumpAnim(true);
            //Launch punch animation
            Vector3 enemyHitPosition = enemy.transform.position;
            m_bHitPunchInProgress = true;
            m_bHitPunchReloading = true;
            Sequence mySequencePunch = DOTween.Sequence();
            mySequencePunch.Append(gameObject.transform.DOLookAt((Vector3.zero - enemyHitPosition), 0.2f, AxisConstraint.Y, Vector3.up)) //WHILE LOOKINGAT
                        .Join(gameObject.transform.DOJump(enemy.transform.position, 0.5f, 1, .25f)) //JUMP
                //.Append(gameObject.transform.DOShakePosition(0.5f, (enemy.transform.position - Vector3.zero), 15, 25.0f)) //On Jump end, Shake
                .Append(gameObject.transform.DORotate(Vector3.zero, 0/*0.5f*/)) //And Look Back home
                .Join(enemy.transform.parent.transform.DOJump((enemyHitPosition - Vector3.zero) * 2.0f, 0.25f, 1, .35f)) //And Push Ennemy
                .Join(gameObject.transform.DOMove(Vector3.zero, 0.5f)).AppendCallback(ResetPunch); //Then Come back and Reset
            //    m_DashBonus.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.25f))
            //  .Append(m_DashBonus.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.25f));
            //Sequence mySequence = DOTween.Sequence();
            //mySequence.Append(mySequenceScale);
            //mySequence.Join(m_DashBonus.DOMove(new Vector3(-1.0f, 1.5f, 3.5f), 1f)).AppendCallback(HideBonus);
        }
    }
    private void ResetPunch()
    {
        m_bHitPunchInProgress = false;
        m_bHitPunchReloading = false; //will be used for skill recast
        m_PlayerAnims.JumpBumpAnim(false);
    }
}
