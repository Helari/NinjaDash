using UnityEngine;
using System.Collections;

public class ND_EnemyAnim : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Hit()
    {
        GameEventManager.TriggerHitAnim(gameObject.transform.position);
    }
}
