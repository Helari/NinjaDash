using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ND_DashPath : MonoBehaviour {

    private GameObject m_LastSelected = null;
    private GameObject m_SecondLastSelected = null;
    public List<GameObject> m_DashPlanning = new List<GameObject>();
    public List<GameObject> m_DashDisplay = new List<GameObject>();
    public BoxCollider m_hitBox = null;

    private int m_iCurrentTargetIndex = 0;
    public ND_Player m_Player;
    public Color m_cTargetColor;

	// Use this for initialization
	void Start () {
        if (m_Player == null)
        {
            m_Player = GameObject.Find("Player").GetComponent<ND_Player>();//gameObject.GetComponent<ND_Player>();
        }
        m_hitBox = gameObject.GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        if(m_DashPlanning.Count > 0 && m_DashPlanning.Count != m_DashDisplay.Count)
        {
            if (m_DashPlanning.Count==1)
            {
                //Debug.Log("Create " + m_DashPlanning.Count);
                //Debug.Log("Create " + m_DashDisplay.Count);
                DrawLine(Vector3.zero, m_DashPlanning[0].transform.position, Color.magenta);
            }
            else
            {
                DrawLine(m_DashPlanning[m_DashPlanning.Count-2].transform.position, m_DashPlanning[m_DashPlanning.Count-1].transform.position, Color.magenta);
            }
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
        if (enemyComp != null && (enemyComp.m_uHPCurrent) > 0)
        {
            if (!((m_LastSelected != null && target == m_LastSelected) || (m_SecondLastSelected != null && target == m_SecondLastSelected)))
            {
                target.GetComponent<Renderer>().material.SetColor("_Color", m_cTargetColor);//new Color(0.5f, 0.5f, 0.5f, 1));//(c.r - 70.0f / 255, c.g + 20.0f / 255, c.b + 40.0f / 255));
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
                m_SecondLastSelected = m_LastSelected;
                m_LastSelected = target.gameObject;
                m_DashPlanning.Add(target);
            }
        }
    }
    public void StartDash()
    {
        if (m_DashPlanning.Count > 0)
        {
            m_iCurrentTargetIndex = 0;
            StartCoroutine(MoveOverSpeed(m_Player.gameObject, m_DashPlanning[m_iCurrentTargetIndex].transform.position, 50.0f));
        }
        else
        {
            GameEventManager.TriggerSlowMotionState_End();
        }
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
            StartCoroutine(MoveOverSpeed(m_Player.gameObject, m_DashPlanning[m_iCurrentTargetIndex].transform.position, 60.0f));
        }
        else if (m_iCurrentTargetIndex == m_DashPlanning.Count)
        {
            StartCoroutine("LastHit");
        }
    }
    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
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
            m_DashDisplay[m_DashDisplay.Count - 2].GetComponent<LineRenderer>().material.color = Color.red;
        }
        addColliderToLine(start, end);
    }
    public void RemoveLastLine()
    {
        if (m_DashDisplay.Count > 0)
        {
            GameObject toDestroy = m_DashDisplay[m_DashDisplay.Count - 1];
            m_DashDisplay.RemoveAt(m_DashDisplay.Count - 1);
            DestroyObject(toDestroy);
            if (m_DashDisplay.Count > 0)
            {
                m_DashDisplay[m_DashDisplay.Count - 1].GetComponent<LineRenderer>().material.color = Color.magenta;
            }
            ND_Enemy enemyComp = m_DashPlanning[m_DashPlanning.Count - 1].GetComponent<ND_Enemy>();
            if (enemyComp == null)
            {
                enemyComp = m_DashPlanning[m_DashPlanning.Count - 1].transform.parent.gameObject.GetComponent<ND_Enemy>();
            }
            if (enemyComp != null)
            {
                enemyComp.ResetColor();
                enemyComp.RevertDamage();
                m_DashPlanning.RemoveAt(m_DashPlanning.Count - 1);
                if (m_DashPlanning.Count > 0)
                {
                    m_LastSelected = m_DashPlanning[m_DashPlanning.Count - 1];
                    if (m_DashPlanning.Count > 1)
                    {
                        m_SecondLastSelected = m_DashPlanning[m_DashPlanning.Count - 2];
                        addColliderToLine(m_LastSelected.transform.position, m_SecondLastSelected.transform.position);

                        //Color Management in case we roll back to a 2HP ennemy
                        ND_Enemy secondEnemyComp = m_SecondLastSelected.GetComponent<ND_Enemy>();
                        bool parent = false;
                        if (secondEnemyComp == null)
                        {
                            parent = true;
                            secondEnemyComp = m_SecondLastSelected.transform.parent.gameObject.GetComponent<ND_Enemy>();
                        }
                        if (secondEnemyComp != null && (secondEnemyComp.m_uHP - 1) > 0 /*&& (secondEnemyComp.m_uHPCurrent) > 0*/)
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
                                secondEnemyComp.GetComponent<Renderer>().material.SetColor("_Color", m_cTargetColor);
                            }
                        }
                    }
                    else
                    {
                        m_SecondLastSelected = null;
                        addColliderToLine(m_LastSelected.transform.position, Vector3.zero);
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
    private void addColliderToLine(Vector3 startPos, Vector3 endPos)
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
        StartCoroutine(MoveOverSpeed(m_Player.gameObject, new Vector3(0.33802f, 0.17451f, -0.3f), 30.0f));
        yield return new WaitForSeconds(1.0f); // Delay to play death anims
        foreach (GameObject line in m_DashDisplay)
        {
            Destroy(line);
        }
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
        m_DashDisplay.Clear();
        m_DashPlanning.Clear();

        m_SecondLastSelected = null;
        m_LastSelected = null;
        m_hitBox.transform.position = m_hitBox.size = Vector3.zero;

        GameEventManager.TriggerSlowMotionState_End();
    }
}
