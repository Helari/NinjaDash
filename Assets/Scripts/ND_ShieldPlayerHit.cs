using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ND_ShieldPlayerHit : MonoBehaviour {

    private Animator animator;
	// Use this for initialization
    void Start()
    {
        GameEventManager.HitAnim += HitAnim;
        animator = gameObject.GetComponent<Animator>();
    }

    void HitAnim(Vector3 position)
    {
        if (this != null)
        {
            Sequence mySequenceHit = DOTween.Sequence();
            mySequenceHit.Append(gameObject.transform.DOLookAt((position-Vector3.zero), 0.0f, AxisConstraint.Y, Vector3.up));
            //gameObject.transform.rotation = Quaternion.LookRotation(position - gameObject.transform.position);
            animator.SetTrigger("hitEvent");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (animator == null) animator = gameObject.GetComponent<Animator>();
    }
}
