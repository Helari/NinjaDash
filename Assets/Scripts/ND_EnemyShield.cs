using UnityEngine;
using System.Collections;

public class ND_EnemyShield : ND_Enemy {

    public override bool CanBeTargetted()
    {
        return false;//base.CanBeTargetted();
    }
}
