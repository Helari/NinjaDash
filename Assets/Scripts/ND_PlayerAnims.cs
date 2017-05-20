using UnityEngine;
using System.Collections;

public class ND_PlayerAnims : MonoBehaviour {

    private Animator animator;
    public int IdleSpecialDelayMin = 5;
    public int IdleSpecialDelayMax = 20;

    private Transform SwordParentTransform;
    public Transform HandTransform;
    public Transform Sword;
    private Transform SwordInitTransform;
    private SkinnedMeshRenderer CharacterSkinRendered;
    private int PreviousRandomAtack = -1;
    private int RandomAtack;
    private int RandomCheckCounter;

    public void ShowMesh()
    {
        CharacterSkinRendered.enabled = true;
        Sword.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    public void HideMesh()
    {
        if (animator.GetBool("IsSlowMo"))
        {
            Sword.gameObject.GetComponent<MeshRenderer>().enabled = false;
            CharacterSkinRendered.enabled = false;
            if (!animator.GetBool("IsAttackStarting")) gameObject.transform.parent.gameObject.GetComponent<ND_Player>().m_DashPath.TriggerNextAttack();
        }
    }
    public void HoldSwordEvent()
    {
        if (animator.GetBool("IsSlowMo"))
        {
            Sword.transform.parent = HandTransform;
        }
        else { ReleaseSwordEvent(); }
    }
    public void ReleaseSwordEvent()
    {
        Sword.transform.parent = SwordParentTransform;
        Sword.transform.position = SwordInitTransform.position;
        Sword.transform.rotation = SwordInitTransform.rotation;
    }
    public void JumpBumpAnim(bool value)
    {
        animator.SetBool("IsJumping", value);
    }
    public void StartAttackAnim(bool value)
    {
        animator.SetBool("IsAttackStarting", value);
        if(!value) animator.SetTrigger("StartAttack");
    }
    public void EnemyHitAnim()
    {
        animator.SetTrigger("EnemyHit");
    }
    public void AttackAnim(bool value)
    {
        RandomAtack = Random.Range(0, 3);
        RandomCheckCounter = 0;
        while(RandomAtack == PreviousRandomAtack && RandomCheckCounter < 10)
        {        
            RandomAtack = Random.Range(0, 3);
            RandomCheckCounter++;
        }
        PreviousRandomAtack = RandomAtack;
        animator.SetInteger("AttackId", RandomAtack);
        animator.SetBool("ContinueAttacking", value);
    }
    public void DeathAnim(bool value)
    {
        animator.SetBool("IsDead", value);
    }
    public void SlowMoAnim(bool value)
    {
        animator.SetBool("IsSlowMo", value);
    }
	// Use this for initialization
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        CharacterSkinRendered = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        StartCoroutine(IdleSpecialAnim(Random.Range(10, 20)));
        SwordInitTransform = Sword.transform;
        SwordParentTransform = Sword.transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
        if (animator == null) animator = gameObject.GetComponent<Animator>();
	}

    private IEnumerator IdleSpecialAnim(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("IsIdleVariation", true);
        yield return new WaitForFixedUpdate();
        animator.SetBool("IsIdleVariation", false);
        StartCoroutine(IdleSpecialAnim(Random.Range(10, 20)));
    }
}
