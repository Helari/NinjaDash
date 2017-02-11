using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class ND_DashPath : MonoBehaviour {

    private GameObject m_LastSelected = null;
    private GameObject m_SecondLastSelected = null;
    private List<GameObject> m_DashPlanning = new List<GameObject>();
    private List<GameObject> m_DashDisplay = new List<GameObject>();
    private BoxCollider m_hitBox = null;
    private int m_iCurrentDashCount = 0;

    private int m_iCurrentTargetIndex = 0;
    private int m_iCurrentScore = 0;
    public ND_Player m_Player;
    public Text m_DashText;
    public Text m_DashCancelText;
    public Text m_ScoreText;
    public Color m_cTargetColor;
    public Color m_cDefaultLineColor = Color.red;
    public Color m_cLastLineColor = Color.magenta;
    public Transform m_DashBonus;

    private RaycastHit hitInfo;
    private List<Collider> deactivatedColliders = new List<Collider>();

	// Use this for initialization
	void Start () {
        if (m_Player == null)
        {
            m_Player = GameObject.Find("Player").GetComponent<ND_Player>();//gameObject.GetComponent<ND_Player>();
        }
        m_hitBox = gameObject.GetComponent<BoxCollider>();
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        GameEventManager.GameOver += GameOver;
        GameEventManager.GameStart += GameStart;
        GameEventManager.Victory += Victory;
	}
	
	// Update is called once per frame
	void Update () {
        if(m_DashPlanning.Count > 0 && m_DashPlanning.Count != m_DashDisplay.Count)
        {
            if (m_DashPlanning.Count==1)
            {
                //Debug.Log("Create " + m_DashPlanning.Count);
                //Debug.Log("Create " + m_DashDisplay.Count);
                DrawLine(Vector3.zero, m_DashPlanning[0].transform.position, m_cLastLineColor);
            }
            else
            {
                DrawLine(m_DashPlanning[m_DashPlanning.Count-2].transform.position, m_DashPlanning[m_DashPlanning.Count-1].transform.position, m_cLastLineColor);
            }
        }
	}

    void GameOver()
    {
        if (this != null)
        {
            ClearDashPash();
        }
    }
    void GameStart()
    {
        if (this != null)
        {
            m_ScoreText.text = "Score : " + m_iCurrentScore.ToString();
            m_DashText.text = "Dash Remaining : " + m_iCurrentDashCount.ToString();
            m_DashCancelText.text = "0";
        }
    }
    void Victory()
    {
        if (this != null)
        {
            ClearDashPash();
        }
    }

    private void ClearDashPash()
    {
        StopAllCoroutines();
        foreach (GameObject line in m_DashDisplay)
        {
            Destroy(line);
        }
        m_DashDisplay.Clear();
        m_DashPlanning.Clear();

        m_SecondLastSelected = null;
        m_LastSelected = null;
        m_hitBox.transform.position = m_hitBox.size = Vector3.zero;
        m_LastSelected = null;
        m_SecondLastSelected = null;
        m_iCurrentDashCount = 0;

        m_iCurrentTargetIndex = 0;
        m_iCurrentScore = 0;

        if (deactivatedColliders.Count > 0) //List of colliders deativated on ShieldEnemies after selecting them
        {
            foreach (Collider col in deactivatedColliders)
            {
                col.enabled = true;
            }
            deactivatedColliders.Clear();
        }
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            m_iCurrentDashCount = m_Player.m_iDefaultDashCount;
            m_DashText.text = "Dash Remaining : " + m_iCurrentDashCount.ToString();
            m_DashCancelText.text = "0";
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
            m_DashText.text = "";
        }
    }
    public void TryAddNewTarget(GameObject target)
    {
        //Check if enemy has still HP and if he wasn't touched the 2 previous times
        //if(!m_DashPlanning.Contains(target))
        ND_Enemy enemyComp = target.GetComponent<ND_Enemy>();
        if(enemyComp == null)
        {
            enemyComp = target.transform.parent.gameObject.GetComponent<ND_Enemy>();
        }
        if (enemyComp != null && (enemyComp.m_uHPCurrent) > 0 && m_iCurrentDashCount > 0)
        {
            if (!((m_LastSelected != null && target == m_LastSelected) || (m_SecondLastSelected != null && target == m_SecondLastSelected)))
            {
                bool canBeTargetted = true;
                if ((m_LastSelected != null && target != m_LastSelected))
                {
                    canBeTargetted = !HasShieldInTheWay(m_LastSelected.transform.position, enemyComp.transform.position);
                }
                else
                {
                    canBeTargetted = !HasShieldInTheWay(m_Player.transform.position, enemyComp.transform.position);
                }
                if (!canBeTargetted)
                {
                    return;
                }

                if (deactivatedColliders.Count > 0) //List of colliders deativated on ShieldEnemies after selecting them
                {
                    foreach (Collider col in deactivatedColliders)
                    {
                        col.enabled = true;
                    }
                    deactivatedColliders.Clear();
                }

                if (enemyComp.HasShieldComponent()) //If target has a shield, deactivate it until next add try
                {
                    foreach (Collider col in enemyComp.GetComponentsInChildren<Collider>())
                    {
                        col.enabled = false;
                        deactivatedColliders.Add(col);
                    }
                }

                if (target.GetComponent<Renderer>() != null) target.GetComponent<Renderer>().material.SetColor("_Color", m_cTargetColor);
                else target.GetComponentInChildren<Renderer>().material.SetColor("_Color", m_cTargetColor);
                //target.GetComponent<Renderer>().material.SetColor("_Color", m_cTargetColor);//new Color(0.5f, 0.5f, 0.5f, 1));//(c.r - 70.0f / 255, c.g + 20.0f / 255, c.b + 40.0f / 255));
                if (m_SecondLastSelected != null)
                {
                    ND_Enemy secondEnemyComp = m_SecondLastSelected.GetComponent<ND_Enemy>();
                    if(secondEnemyComp == null)
                    {
                        secondEnemyComp = m_SecondLastSelected.transform.parent.gameObject.GetComponent<ND_Enemy>();
                    }
                    if (secondEnemyComp != null && (secondEnemyComp.m_uHP - 1) > 0 && (secondEnemyComp.m_uHPCurrent) > 0)
                    {
                        secondEnemyComp.ResetColor();
                    }
                }
                enemyComp.Damage();
                if(enemyComp.ShouldDie())
                {
                    m_iCurrentScore += enemyComp.m_iScore;
                }
                m_SecondLastSelected = m_LastSelected;
                m_LastSelected = target.gameObject;
                m_iCurrentDashCount--;
                m_DashText.text = "Dash Remaining : " + m_iCurrentDashCount.ToString();
                if (enemyComp.m_iDashBonus != 0)
                {
                    OnDashBonus(true, enemyComp);
                }
                m_DashPlanning.Add(target);
                m_DashCancelText.text = m_DashPlanning.Count.ToString();
            }
        }
    }
    private bool HasShieldInTheWay(Vector3 _from, Vector3 _to)
    {
        bool resutl = false;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(_from, (_to - _from), Vector3.Distance(_to, _from));

        for (int i = 0; i < hits.Length; i++) //Check if a shield is hit (and then can't dash on the target)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.tag == "Shield")
            {
                ND_Enemy enemyShieldComp = hit.transform.gameObject.GetComponent<ND_Enemy>();
                if (enemyShieldComp == null)
                {
                    enemyShieldComp = hit.transform.parent.gameObject.GetComponent<ND_Enemy>();
                }
                if (enemyShieldComp.m_uHPCurrent > 0) //The shield is blocking only if the shieldEnemy is still alive
                {
                    resutl = true;
                    break;
                }
            }
        }
        return resutl;
    }
    public void StartDash()
    {
        if (HasDashTargets())
        {
            m_iCurrentTargetIndex = 0;
            StartCoroutine(MoveOverSpeed(m_Player.gameObject, m_DashPlanning[m_iCurrentTargetIndex].transform.position, 30.0f));
        }
        else
        {
            GameEventManager.TriggerSlowMotionState_End();
        }
    }

    public bool HasDashTargets()
    {
        return m_DashPlanning.Count > 0;
    }

    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed)
    {
        // speed should be 1 unit per second
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        m_iCurrentTargetIndex++;
        if (m_iCurrentTargetIndex < m_DashPlanning.Count)
        {
            StartCoroutine(MoveOverSpeed(m_Player.gameObject, m_DashPlanning[m_iCurrentTargetIndex].transform.position, 30.0f));
        }
        else if (m_iCurrentTargetIndex == m_DashPlanning.Count)
        {
            StartCoroutine("LastHit");
        }
    }
    void DrawLine(Vector3 start, Vector3 end, Color color) //Draws a line using LineMaterial and adds a collider to it
    {
        start += new Vector3(0.0f, 0.05f, 0.0f);
        end += new Vector3(0.0f, 0.05f, 0.0f);
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.receiveShadows = false;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.material = Resources.Load("LineMaterial", typeof(Material)) as Material;//new Material(Shader.Find("Mobile/Diffuse"));
        lr.material.color = color;
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        m_DashDisplay.Add(myLine);
        if (m_DashDisplay.Count > 1)
        {
            m_DashDisplay[m_DashDisplay.Count - 2].GetComponent<LineRenderer>().material.color = m_cDefaultLineColor;
        }
        AddColliderToLine(start, end);
    }
    public void RemoveLastLine() //Revert last action
    {
        if (m_DashDisplay.Count > 0) //If we has at least one target selected
        {
            GameObject toDestroy = m_DashDisplay[m_DashDisplay.Count - 1]; //Remove last line
            m_DashDisplay.RemoveAt(m_DashDisplay.Count - 1);
            DestroyObject(toDestroy);
            if (m_DashDisplay.Count > 0) //If there's still lines displayed, the last one takes the "last one color"
            {
                m_DashDisplay[m_DashDisplay.Count - 1].GetComponent<LineRenderer>().material.color = m_cLastLineColor;
            }

            //Revert enemy datas
            ND_Enemy enemyComp = m_DashPlanning[m_DashPlanning.Count - 1].GetComponent<ND_Enemy>();
            if (enemyComp == null)
            {
                enemyComp = m_DashPlanning[m_DashPlanning.Count - 1].transform.parent.gameObject.GetComponent<ND_Enemy>();
            }
            if (enemyComp != null)
            {
                if (enemyComp.ShouldDie()) //Remove earned score
                {
                    m_iCurrentScore -= enemyComp.m_iScore;
                }
                enemyComp.ResetColor();
                enemyComp.RevertDamage();

                if (enemyComp.HasShieldComponent() && deactivatedColliders.Count > 0) //List of colliders deativated on ShieldEnemies after selecting them
                {
                    foreach (Collider col in deactivatedColliders)
                    {
                        col.enabled = true;
                    }
                    deactivatedColliders.Clear();
                }

                m_iCurrentDashCount++;
                m_DashText.text = "Dash Remaining : " + m_iCurrentDashCount.ToString();
                m_DashPlanning.RemoveAt(m_DashPlanning.Count - 1);
                m_DashCancelText.text = m_DashPlanning.Count.ToString();
                if(enemyComp.m_iDashBonus != 0) //Revert dash bonus
                {
                    OnDashBonus(false, enemyComp);
                }
                //Now, enemyComp points to the former last selected, if we still have some enemies selected, we can assign them to enemyComp and secondEnemyComp
                if (m_DashPlanning.Count > 0) //Refresh last ennemy
                {
                    m_LastSelected = m_DashPlanning[m_DashPlanning.Count - 1];

                    enemyComp = m_LastSelected.GetComponent<ND_Enemy>();
                    if (enemyComp == null)
                    {
                        enemyComp = m_LastSelected.transform.parent.gameObject.GetComponent<ND_Enemy>();
                    }
                    if (enemyComp.HasShieldComponent()) //If target has a shield, deactivate it until next add try
                    {
                        foreach (Collider col in enemyComp.GetComponentsInChildren<Collider>())
                        {
                            col.enabled = false;
                            deactivatedColliders.Add(col);
                        }
                    }

                    if (m_DashPlanning.Count > 1) //Refresh second last ennemy
                    {
                        m_SecondLastSelected = m_DashPlanning[m_DashPlanning.Count - 2];
                        AddColliderToLine(m_LastSelected.transform.position, m_SecondLastSelected.transform.position);

                        //Color Management in case we roll back to a 2HP ennemy
                        ND_Enemy secondEnemyComp = m_SecondLastSelected.GetComponent<ND_Enemy>();
                        bool parent = false;
                        if (secondEnemyComp == null)
                        {
                            parent = true;
                            secondEnemyComp = m_SecondLastSelected.transform.parent.gameObject.GetComponent<ND_Enemy>();
                        }
                        if (secondEnemyComp != null && (secondEnemyComp.m_uHP - 1) > 0 /*&& (secondEnemyComp.m_uHPCurrent) > 0*/) //Color management for 2HP+ enemies
                        {
                            if (parent)
                            {
                                Renderer[] renderers = secondEnemyComp.GetComponentsInChildren<Renderer>();
                                foreach (Renderer renderer in renderers)
                                {
                                    renderer.material.SetColor("_Color", m_cTargetColor);//(c.r + 70.0f / 255, c.g - 20.0f / 255, c.b - 40.0f / 255));
                                }
                            }
                            else
                            {
                                if(secondEnemyComp.GetComponent<Renderer>() != null) secondEnemyComp.GetComponent<Renderer>().material.SetColor("_Color", m_cTargetColor);
                                else secondEnemyComp.GetComponentInChildren<Renderer>().material.SetColor("_Color", m_cTargetColor);
                            }
                        }
                    }
                    else
                    {
                        m_SecondLastSelected = null;
                        AddColliderToLine(m_LastSelected.transform.position, Vector3.zero);
                    }
                }
                else
                {
                    m_LastSelected = null;
                    m_SecondLastSelected = null;
                    m_hitBox.transform.position = m_hitBox.size = Vector3.zero;
                }
            }
        }
    }
    private void OnDashBonus(bool isIncrement, ND_Enemy enemy)
    {
        if(isIncrement)
        {
            m_iCurrentDashCount += enemy.m_iDashBonus;

            m_DashBonus.transform.position = enemy.transform.position;
            m_DashBonus.gameObject.SetActive(true);
            Sequence mySequenceScale = DOTween.Sequence();
            mySequenceScale.Append(m_DashBonus.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.25f))
              .Append(m_DashBonus.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.25f));
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(mySequenceScale);
            mySequence.Join(m_DashBonus.DOMove(new Vector3(-1.0f, 1.5f, 3.5f), 1f)).AppendCallback(HideBonus);
        }
        else
        {
            m_iCurrentDashCount -= enemy.m_iDashBonus;
        }
        m_DashText.text = "Dash Remaining : " + m_iCurrentDashCount.ToString();
    }
    private void HideBonus()
    {
        m_DashBonus.gameObject.SetActive(false);
    }
    private void AddColliderToLine(Vector3 startPos, Vector3 endPos)
    {
        float lineLength = Vector3.Distance (startPos, endPos); // length of line
        m_hitBox.size = new Vector3(0.2f, 0.5f, lineLength); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos)/2;
        m_hitBox.transform.position = midPoint; // setting position of collider object
        Vector3 _direction = (startPos - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(_direction);
    }
    IEnumerator LastHit()
    {
        StartCoroutine(MoveOverSpeed(m_Player.gameObject, Vector3.zero, 30.0f));
        foreach (GameObject line in m_DashDisplay)
        {
            Destroy(line);
        }

        m_SecondLastSelected = null;
        m_LastSelected = null;
        m_hitBox.transform.position = m_hitBox.size = Vector3.zero;

        //List<ND_Enemy> colateralDamag = new List<ND_Enemy>();
        Dictionary<ND_Enemy, Vector3> colateralDamage = new Dictionary<ND_Enemy, Vector3>(); ;

        foreach (GameObject enemy in m_DashPlanning)
        {
            if (enemy.transform.parent.gameObject.GetComponent<ND_EnemyBomb>() != null)
            {
                m_iCurrentScore += enemy.transform.parent.gameObject.GetComponent<ND_EnemyBomb>().GenerateExplosion(colateralDamage);
                enemy.transform.parent.gameObject.GetComponent<ND_EnemyBomb>().LaunchExplosionAnim();
            }
            if (enemy.GetComponent<ND_Enemy>() != null)
            {
                if (enemy.GetComponent<ND_Enemy>().ShouldDie())
                {
                    if (enemy.GetComponent<ND_DismemberManager>() != null)
                        enemy.GetComponent<ND_DismemberManager>().Explode(Vector3.zero);
                    enemy.GetComponent<ND_Enemy>().StopAllCoroutines();
                }
            }
            else if (enemy.transform.parent.gameObject.GetComponent<ND_Enemy>() != null)
            {
                if (enemy.transform.parent.gameObject.GetComponent<ND_Enemy>().ShouldDie())
                {
                    if (enemy.transform.parent.gameObject.GetComponent<ND_DismemberManager>() != null)
                        enemy.transform.parent.gameObject.GetComponent<ND_DismemberManager>().Explode(Vector3.zero);
                    enemy.transform.parent.gameObject.GetComponent<ND_Enemy>().StopAllCoroutines();
                }
            }

            //if (m_DashPlanning[m_iCurrentTargetIndex].GetComponent<ND_Enemy>() != null && m_DashPlanning[m_iCurrentTargetIndex].GetComponent<ND_Enemy>().ShouldDie() &&
            //    m_DashPlanning[m_iCurrentTargetIndex].GetComponent<ND_DismemberManager>() != null)
            //{
            //    m_DashPlanning[m_iCurrentTargetIndex].GetComponent<ND_DismemberManager>().Explode();
            //}
            //else if (m_DashPlanning[m_iCurrentTargetIndex].transform.parent.gameObject.GetComponent<ND_Enemy>() != null &&
            //    m_DashPlanning[m_iCurrentTargetIndex].transform.parent.gameObject.GetComponent<ND_Enemy>().ShouldDie() &&
            //    m_DashPlanning[m_iCurrentTargetIndex].transform.parent.GetComponent<ND_DismemberManager>())
            //{
            //    m_DashPlanning[m_iCurrentTargetIndex].transform.parent.gameObject.GetComponent<ND_DismemberManager>().Explode();
            //}
        }
        foreach (ND_Enemy enemy in colateralDamage.Keys) //Additionnal enemies that have to explode after bomb explosion
        {
            if (enemy.gameObject.GetComponent<ND_DismemberManager>() != null)
            {
                Vector3 explosionPosition = Vector3.zero;
                colateralDamage.TryGetValue(enemy, out explosionPosition);
                enemy.gameObject.GetComponent<ND_DismemberManager>().Explode(explosionPosition);
            }
            enemy.StopAllCoroutines();
        }

        yield return new WaitForSeconds(1.0f); // Delay to play death anims
        yield return new WaitForSeconds(2.0f);
        foreach (GameObject enemy in m_DashPlanning)
        {
            if (enemy.GetComponent<ND_Enemy>() != null)
            {
                enemy.GetComponent<ND_Enemy>().CheckDeath();
            }
            else if (enemy.transform.parent.gameObject.GetComponent<ND_Enemy>() != null)
            {
                enemy.transform.parent.gameObject.GetComponent<ND_Enemy>().CheckDeath();
            }
        }
        foreach (ND_Enemy enemy in colateralDamage.Keys) //Additionnal enemies that have to explode after bomb explosion
        {
            enemy.CheckDeath();
        }
        m_ScoreText.text = "Score : " + m_iCurrentScore.ToString();
        m_DashDisplay.Clear();
        m_DashPlanning.Clear();
        m_Player.ReloadDash();
        GameEventManager.TriggerSlowMotionState_End();
    }
}
