using UnityEngine;
using System.Collections;

public class ND_EnemyShield : MonoBehaviour {

    //private bool m_bCanBeTargetted = true;
    private float m_fShieldRadius = 180.0f;

    private float m_fHalfShieldRadius = 90.0f;

	public float scrollSpeed = 0.5F;
	public Material m_Material;

	void Start()
	{
		m_Material = GetComponentInChildren<SkinnedMeshRenderer>().materials[2];
	}
	void Update() {
		float offset = Time.time * scrollSpeed;
		m_Material.SetTextureOffset("_MainTex", new Vector2(0, offset));
	}

    public bool CanBeTargetted(Vector3 _from)
    {
        m_fHalfShieldRadius = m_fShieldRadius * 0.5f;
        Vector3 _myForward = this.gameObject.transform.forward;
        Vector3 _playerDir = (_from-this.transform.position);
        float dot = Vector3.Dot(_playerDir, _myForward);

        return IsOverShieldAngle(dot, _playerDir.magnitude * _myForward.magnitude);
    }
    private bool IsOverShieldAngle(float dot, float vectorsMagnitude)
    {
        float angleInRadians = Mathf.Abs(Mathf.Acos(dot / vectorsMagnitude));
        float angleInDegrees = (angleInRadians * 180.0f) / Mathf.PI;
        return angleInDegrees > m_fHalfShieldRadius;
    }
}
