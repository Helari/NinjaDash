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
        Event e = Event.current;
        m_sceneViewCamera = sceneview.camera;
        activePaint = false;

        if(e.type == EventType.mouseDown && e.button == 0)
        {
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                RaycastHit[] hitCapsule;
                hitCapsule = Physics.CapsuleCastAll(hit.point, hit.point, 0.9f, Vector3.up);

                bool enemyTouch = false;

                for(int i = 0; i < hitCapsule.Length; i++)
                {
                    if (hitCapsule[i].collider.CompareTag("Enemy"))
                    {
                        enemyTouch = true;
                    }
                }

                if(!enemyTouch)
                {
                    //if(m_domino[enemyIndex] != null)
                    if (GameObject.FindObjectOfType<ND_EnemyFactory>().objectPrefabs[enemyIndex] != null)
                    {
                        SetContainer();

                        Vector3 direction = Vector3.zero;
                        //if(lastLocation != Vector3.zero)
                        {
                            direction = Vector3.zero - hit.point;// hit.point;//No need to change direction we always want to face the center
                        }

                        GameObject obj = GameObject.Instantiate(GameObject.FindObjectOfType<ND_EnemyFactory>().objectPrefabs[enemyIndex], new Vector3(hit.point.x, /*hit.point.y + m_domino[enemyIndex].transform.localScale.y * 0.5f*/0.0f, hit.point.z), Quaternion.LookRotation(direction)) as GameObject;
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
                //Debug.Log(list[i].gameObject.transform.position.ToString());
                temp.position = list[i].gameObject.transform.position;//new Vector3(list[i].gameObject.transform.position.x, list[i].gameObject.transform.position.y, list[i].gameObject.transform.position.z);
                temp.rotation = list[i].gameObject.transform.rotation;
                temp.archetype = list[i].m_ArchetypeID;
                listTrans.Add(temp);
            }

            ND_EnemyPattern data = ND_EnemyPattern.CreateInstance(listTrans);


            //AssetDatabase.GenerateUniqueAssetPath("Assets/listDomino.asset");
            AssetDatabase.CreateAsset(data, "Assets/" + PatternName + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
    void createLevelScriptable()
    {
        ND_Level data = ND_Level.CreateInstance();

        AssetDatabase.CreateAsset(data, "Assets/Level.asset");
    }
    void saveAssets()
    {
        AssetDatabase.SaveAssets();
    }
	void loadDominos() 
    {
        ND_EnemyPattern dataLoaded = (ND_EnemyPattern)AssetDatabase.LoadAssetAtPath("Assets/" + PatternName + ".asset", typeof(ND_EnemyPattern));

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
        if (GUILayout.Button("Create Level"))
        {
            createLevelScriptable();
        }
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
		//Editor styles
		//myColor = EditorGUILayout.ColorField(myColor); 
        //for (int i = 0; i < NB_ARCHETYPES; i++)
        //{
        //    if (m_domino[i] != null)
        //    {
        //        GameObject test = m_domino[i];
        //        test = (GameObject)EditorGUILayout.ObjectField(test, typeof(GameObject));
        //    }
        //    //SerializedProperty myArray = m_domino;
        //    //myArray.GetArrayElementAtIndex
        //}
        /*serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objects"));
        serializedObject.ApplyModifiedProperties();*/

       // layer = EditorGUILayout.LayerField (layer);
        //enemyIndex = EditorGUILayout.IntField(enemyIndex);
      //  enemyIndex = EditorGUILayout.IntField(enemyIndex);
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        //SerializedProperty stringsProperty = so.FindProperty("enemyPrefabs");

        //EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties
        //Debug.Log(stringsProperty);
        EditorGUILayout.LabelField("Pattern Name : ");
        PatternName = EditorGUILayout.TextField(PatternName);
        //if (stringsProperty.arraySize > 0)
        {
            //if (stringsProperty.GetArrayElementAtIndex(enemyIndex).objectReferenceValue != null)
            {
                EditorGUILayout.LabelField("Current Selectec Enemy : " + factory.objectPrefabs[enemyIndex].name);//ND_EnemyFactory.instance.objectPrefabs[enemyIndex].name);//stringsProperty.GetArrayElementAtIndex(enemyIndex).objectReferenceValue.name);
                //for (int i = 0; i < stringsProperty.arraySize; i++)
                {
                    //GameObject GO = stringsProperty.GetArrayElementAtIndex(i).Copy() as System.Object as GameObject;
                   // GameObject GOd = stringsProperty.GetArrayElementAtIndex(i).objectReferenceValue as System.Object as GameObject;
                    //if (GO == null)
                    //{
                    //    Debug.Log("NULLLLLgo" + i);
                    //}
                    //if (GOd == null)
                    {
                        //Debug.Log("NULLLLLgod" + i);
                        //break;
                    }
                    //m_domino.SetValue(stringsProperty.GetArrayElementAtIndex(i).objectReferenceValue as System.Object as GameObject, i);
                    //if (!m_domino.Contains(GOd))
                    //{
                    //    m_domino.Add(GOd);
                    //}
                    //m_domino[i] = stringsProperty.GetArrayElementAtIndex(i).objectReferenceValue as System.Object as GameObject;//objectReferenceValue//serializedObject;
                }
            }
        }
	}
}

#endif