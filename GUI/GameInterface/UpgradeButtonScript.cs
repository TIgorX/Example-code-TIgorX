using UnityEngine;
using System.Collections;

public class UpgradeButtonScript : AbstractUpgradeButtonScript
{
    public int spriteID;
    public UpgradeManager upgradeManager;
    public tk2dSlicedSprite slicedSprite;
    public tk2dUIItem uIItem;

    public string text = "";
    private tk2dTextMesh label;

    void OnEnable()
    {
        
    }

    void Awake()
    {
        label = transform.GetComponentInChildren<tk2dTextMesh>();
        uIItem.OnClickUIItem += OnClick;
    }

    public override void ActivateCheckSprite()
    {
        slicedSprite.SetSprite(0);
        label.text = "Confirm";
        label.transform.localPosition = new Vector3(-0.5f, 0.15f, label.transform.localPosition.z); 
    }

    public override void DeactivateCheckSprite()
    {
        slicedSprite.SetSprite(spriteID);
        label.text = text;
    }

    void OnClick(tk2dUIItem Tk2dUIItem)
    {
        upgradeManager.UpgradeButtonClick(this);
    }

	void Start () 
    {
        DeactivateCheckSprite();
        this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y, -0.02f);
	}
}
