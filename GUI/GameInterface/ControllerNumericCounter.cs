using UnityEngine;
using System.Collections;

public class ControllerNumericCounter : MonoBehaviour 
{
    public Material numericMaterial1;
    public Material numericMaterial2;
    public Material numericMaterial3;

    private int numeric1 = 0;
    private int numeric2 = 0;
    private int numeric3 = 0;

    public float speedOfRotation = 1f;
    private bool flag = false;

    public void UpdateNumericCounter(int Number)
    {
        flag = true;
        numeric1 = Number % 10;
        Number /= 10;

        numeric2 = Number % 10;
        Number /= 10;

        numeric3 = Number % 10;
        Number /= 10;
    }

    private bool RotateDrum(Material _material, int Numeric)
    {
        Vector2 textureOffset = _material.GetTextureOffset("_MainTex");
        if ((Mathf.Abs((textureOffset.y % 1f) - (Numeric * 0.1f)) < 0.005f) ||
            (Mathf.Abs((textureOffset.y % 1f) - (Numeric * 0.1f)) <= speedOfRotation * 0.1f * tk2dUITime.deltaTime))
        {
            _material.SetTextureOffset("_MainTex", new Vector2(textureOffset.x, 1 + (Numeric * 0.1f)));
            return true;
        }
        else
        {
            int k = 1;

            float lengthForward = -1;
            if ((Numeric * 0.1f) > (textureOffset.y % 1f))
            {
                lengthForward = (Numeric * 0.1f) - (textureOffset.y % 1f);
            }
            else
            {
                lengthForward = (Numeric * 0.1f) - (textureOffset.y % 1f - 1);
            }

            float lengthBackward = -1;
            if ((Numeric * 0.1f) < (textureOffset.y % 1f))
            {
                lengthBackward = (textureOffset.y % 1f) - (Numeric * 0.1f);
            }
            else
            {
                lengthBackward = (textureOffset.y % 1f) - (Numeric * 0.1f - 1);
            }

            if (lengthBackward < lengthForward)
                k *= -1;

            _material.SetTextureOffset("_MainTex", _material.GetTextureOffset("_MainTex") + k * new Vector2(0, speedOfRotation*0.1f*tk2dUITime.deltaTime));
        }
        return false;
    }

	void Update () 
    {
        if (flag)
        {
            if (RotateDrum(numericMaterial1, numeric1) &
                RotateDrum(numericMaterial2, numeric2) &
                RotateDrum(numericMaterial3, numeric3) )
            {
                flag = false;
            }
        }
	}
}
