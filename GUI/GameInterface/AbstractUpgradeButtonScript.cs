using UnityEngine;
using System.Collections;

public abstract class AbstractUpgradeButtonScript : MonoBehaviour 
{
    public UpgradeActionID upgradeActionID;
    public abstract void ActivateCheckSprite();
    public abstract void DeactivateCheckSprite();
}
