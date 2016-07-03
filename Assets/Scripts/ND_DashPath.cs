using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ND_DashPath : MonoBehaviour {

    private GameObject m_LastSelected;
    private GameObject m_SecondLastSelected;
    private List<GameObject> m_DashPlanning = new List<GameObject>();
    private List<GameObject> m_DashDisplay = new List<GameObject>();

    private int m_iCurrentTargetIndex = 0;
    public ND_Player m_Player;
    public Color m_cTargetColor;

	// Use this for initialization
	void Start () {
        m_Player = gameObject.GetComponent<ND_Player>();
	}
	
	// Update is called once per frame
	void Update () {
        if(m_DashPlanning.Count > 0 && m_DashPlanning.Count != m_DashDisplay.Count)
        {
            if (m_DashPlanning.Count==1)
            {
                //Debug.Log("Create " + m_DashPlanning.Count);
                //Debug.Log("Create " + m_DashDisplay.Count);
                DrawLine(Vector3.zero, m_DashPlanning[0].transform.position, Color.red);
            }
            else
            {
                DrawLine(m_DashPlanning[m_DashPlanning.Count-2].transform.position, m_DashPlanning[m_DashPlanning.Count-1].transform.position, Color.red);
            }
        }
	}

    public void TryAddNewTarget(GameObject target)
    {
        //TODO : Check if enemy has still HP and if he wasn't touched the 2 previous times
        if(!m_DashPlanning.Contains(target))
        {
            m_SecondLastSelected = m_LastSelected;
            m_LastSelected = target;
            Color c = target.GetComponent<Renderer>().material.GetColor("_Color");
            target.GetComponent<Renderer>().material.SetColor("_Color", m_cTargetColor);//new Color(0.5f, 0.5f, 0.5f, 1));//(c.r - 70.0f / 255, c.g + 20.0f / 255, c.b + 40.0f / 255));
            m_DashPlanning.Add(target);
        }
    }
    public void StartDash()
    {
        if (m_DashPlanning.Count > 0)
        {
            m_iCurrentTargetIndex = 0;
            StartCoroutine(MoveOverSpeed(m_Player.gameObject, m_DashPlanning[m_iCurrentTargetIndex].transform.position, 20.0f));
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
            StartCoroutine(MoveOverSpeed(m_Player.gameObject, m_DashPlanning[m_iCurrentTargetIndex].transform.position, 30.0f));
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
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        m_DashDisplay.Add(myLine);
    }
    IEnumerator LastHit()
    {
        StartCoroutine(MoveOverSpeed(m_Player.gameObject, new Vector3(0.33802f, 0.17451f, -0.3f), 10.0f));
        yield return new WaitForSeconds(1.0f); // Delay to play death anims
        foreach (GameObject line in m_DashDisplay)
        {
            Destroy(line);
        }
        foreach (GameObject enemy in m_DashPlanning)
        {
            if (enemy.GetComponent<ND_Enemy>() != null)
            {
                enemy.GetComponent<ND_Enemy>().Damage();
            }
            else if (enemy.transform.parent.gameObject.GetComponent<ND_Enemy>() != null)
            {
                enemy.transform.parent.gameObject.GetComponent<ND_Enemy>().Damage();
            }
        }
        m_DashDisplay.Clear();
        m_DashPlanning.Clear();

        GameEventManager.TriggerSlowMotionState_End();
    }
}
