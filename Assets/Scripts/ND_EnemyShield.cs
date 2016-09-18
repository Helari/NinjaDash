using UnityEngine;
using System.Collections;

public class ND_EnemyShield : MonoBehaviour {

    //private bool m_bCanBeTargetted = true;
    private float m_fShieldRadius = 180.0f;

    private float m_fHalfShieldRadius = 90.0f;

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
