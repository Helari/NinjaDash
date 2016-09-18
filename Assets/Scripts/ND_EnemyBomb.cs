using UnityEngine;
using System.Collections;

public class ND_EnemyBomb : MonoBehaviour {

    //public SphereCollider m_DamageArea;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
     //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(gameObject.transform.position, 1.0f);
    }
    public int GenerateExplosion()
    {
        int totalBonusScore = 0;
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 1.0f);
        foreach(Collider col in colliders)
        {
            //TODO : Check if not me ?
            ND_Enemy enemyComp = col.gameObject.transform.parent.GetComponent<ND_Enemy>();
            if (enemyComp != null)
            {
                enemyComp.Damage();
                enemyComp.CheckDeath();

                if (enemyComp.ShouldDie())
                {
                    totalBonusScore += enemyComp.m_iScore;
                }
            }
        }
        return totalBonusScore;
    }
}
