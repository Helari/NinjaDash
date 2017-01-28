#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;
using System.Collections.Generic;

public class ND_EnemyPatternCreator : EditorWindow 
{
	static private Vector3 m_mousePositionOnScene;

    static private Camera m_sceneViewCamera = null;

	static public LayerMask layer;
	static bool activePaint = false;

	static Vector3 lastLocation = Vector3.zero;

    static public Transform container;
    public string PatternName = "[ND_DefaultPatternName]";
    static public bool isActive = false;
    static public int enemyIndex = 0;
    ND_EnemyFactory factory;

	//Show Window
	[MenuItem("Ninja Dash Tools/Pattern Creator")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(ND_EnemyPatternCreator));
		SceneView.onSceneGUIDelegate += OnScene;
	}
    
	static void OnScene(SceneView sceneview)
	{
        if (isActive)
        {
            Event e = Event.current;
            m_sceneViewCamera = sceneview.camera;
            activePaint = false;

            if (e.type == EventType.mouseDown && e.button == 0)
            {
                RaycastHit hit;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    RaycastHit[] hitCapsule;
                    hitCapsule = Physics.CapsuleCastAll(hit.point, hit.point, 0.9f, Vector3.up);

                    bool enemyTouch = false;

                    for (int i = 0; i < hitCapsule.Length; i++)
                    {
                        if (hitCapsule[i].collider.CompareTag("Enemy"))
                        {
                            enemyTouch = true;
                        }
                    }

                    if (!enemyTouch)
                    {
                        if (GameObject.FindObjectOfType<ND_EnemyFactory>().objectPrefabs[enemyIndex] != null)
                        {
                            SetContainer();

                            Vector3 direction = Vector3.zero;
                            direction = Vector3.zero - hit.point;//No need to change direction we always want to face the center

                            GameObject obj = GameObject.Instantiate(GameObject.FindObjectOfType<ND_EnemyFactory>().objectPrefabs[enemyIndex], new Vector3(hit.point.x, 0.0f, hit.point.z), Quaternion.LookRotation(direction)) as GameObject;
                            obj.transform.SetParent(container);
                            obj.SetActive(true);
                            if (Application.isPlaying)
                            {
                                //obj.GetComponent<Renderer>().material.color = myColor;
                            }
                            else
                            {
                                //obj.GetComponent<Renderer>().sharedMaterial.color = myColor;
                            }
                            lastLocation = hit.point;
                        }
                    }
                    e.Use();
                }
            }
            else if (e.type == EventType.Layout)
            {
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                HandleUtility.AddDefaultControl(controlID);

            }
            else if (e.type == EventType.scrollWheel && e.delta.y < 0)
            {
                enemyIndex++;
                if (enemyIndex >= GameObject.FindObjectOfType<ND_EnemyFactory>().objectPrefabs.Length)
                {
                    enemyIndex = 0;
                }
            }
            else if (e.type == EventType.scrollWheel && e.delta.y > 0)
            {
                enemyIndex--;
                if (enemyIndex < 0)
                {
                    enemyIndex = GameObject.FindObjectOfType<ND_EnemyFactory>().objectPrefabs.Length - 1;
                }
            }
        }
    }

    void updateScriptable()
    {
        SetContainer();
        if(container != null)
        {
            ND_Enemy[] list = container.GetComponentsInChildren<ND_Enemy>();
            List<ND_EnemyTransform> listTrans = new List<ND_EnemyTransform>();

            for(int i = 0; i < list.Length; i++)
            {
                ND_EnemyTransform temp = new ND_EnemyTransform();
                temp.position = list[i].gameObject.transform.position;
                temp.rotation = list[i].gameObject.transform.rotation;
                temp.archetype = list[i].m_ArchetypeID;
                listTrans.Add(temp);
            }

            ND_EnemyPattern data = ND_EnemyPattern.CreateInstance(listTrans);

            AssetDatabase.CreateAsset(data, "Assets/Resources/" + PatternName + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
    void createLevelScriptable()
    {
        ND_Level data = ND_Level.CreateInstance();

        AssetDatabase.CreateAsset(data, "Assets/Resources/Level.asset");
    }
    void saveAssets()
    {
        AssetDatabase.SaveAssets();
    }
	void loadDominos() 
    {
        ND_EnemyPattern dataLoaded = (ND_EnemyPattern)AssetDatabase.LoadAssetAtPath("Assets/Resources/" + PatternName + ".asset", typeof(ND_EnemyPattern));

        SetContainer();

        for(int i = 0; i < dataLoaded.pattern.Count; i++)
        {
            GameObject obj = (GameObject)Instantiate(factory.objectPrefabs[dataLoaded.pattern[i].archetype], dataLoaded.pattern[i].position, dataLoaded.pattern[i].rotation);
            obj.transform.SetParent(container);
        }
    }

    private static void SetContainer()
    {
        if (container == null)
        {
            if (GameObject.Find("PatternContainer") != null)
            {
                container = GameObject.Find("PatternContainer").transform;
            }
            else
            {
                GameObject go = new GameObject("PatternContainer");
                container = go.transform;
            }
        }
    }

	public void OnGUI()
    {
        if(factory == null)
        {
            factory = GameObject.FindObjectOfType<ND_EnemyFactory>();
        }
        //if (GUILayout.Button("Create Level"))
        //{
        //    createLevelScriptable();
        //}
        if (GUILayout.Button("Save Assets"))
        {
            saveAssets();
        }
        if (GUILayout.Button("SAVE Pattern - " + PatternName))
        {
			
			updateScriptable();
		}
        if (GUILayout.Button("LOAD Pattern - " + PatternName))
        {
			
			loadDominos();
        }
        if (GUILayout.Button("+"))
        {
            enemyIndex++;
            if (enemyIndex >= factory.objectPrefabs.Length)
            {
                enemyIndex = 0;
            }
        }
        if (GUILayout.Button("-"))
        {
            enemyIndex--;
            if (enemyIndex < 0)
            {
                enemyIndex = factory.objectPrefabs.Length - 1;
            }
        }

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        //SerializedProperty stringsProperty = so.FindProperty("enemyPrefabs");

        //EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties

        EditorGUILayout.LabelField("Pattern Name : ");
        PatternName = EditorGUILayout.TextField(PatternName);
        isActive = EditorGUILayout.Toggle("Active",isActive);
        EditorGUILayout.LabelField("Current Selectec Enemy : " + factory.objectPrefabs[enemyIndex].name);
	}
}

#endif