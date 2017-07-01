using UnityEngine;
using System.Collections;

public class GameCameraScript : UnitySingleton<GameCameraScript>
{
    public Material land;
    public Material clouds;
    public Material lowClouds;

    private Vector3 differencePosition;
    private Vector3 deltaPosition = new Vector3(0, 0, 0);
    public Vector3 dislocation = Vector3.zero;
    public Vector3 gameModeRotation;
    public Vector3 upgradeModeDeltaPoistion = new Vector3(0, 20, 0);
    public Vector3 upgradeModeRotation = new Vector3(90, 0, 0);

    private float maxDeltaCoordinate = 3;
    private float shiftPerSecond = 2f;
    private bool isGameMode = true;

    protected override void Awake()
    {
        base.Awake();
    }

	void Start () 
    {
        differencePosition = -SteamcopterManager.Instance.transform.position + this.gameObject.transform.position;
	}

    void LateUpdate()
    {
        clouds.SetTextureOffset("_MainTex",-new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z)/50f);
        lowClouds.SetTextureOffset("_MainTex", -new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z) / 200f);
        land.SetTextureOffset("_MainTex", -new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.z) / 500f);

        if ((isGameMode) && (SteamcopterManager.Exists))
        {
            float maxDeltaCoordinateOx = 0;
            float maxDeltaCoordinateOz = 0;

            if ((Mathf.Abs(dislocation.x) + Mathf.Abs(dislocation.z)) > 0.0001f)
            {
                maxDeltaCoordinateOx = dislocation.x * (maxDeltaCoordinate / (Mathf.Abs(dislocation.x) + Mathf.Abs(dislocation.z)));
                maxDeltaCoordinateOz = dislocation.z * (maxDeltaCoordinate / (Mathf.Abs(dislocation.x) + Mathf.Abs(dislocation.z)));

                ShiftCoordinate(ref deltaPosition.x, dislocation.x, maxDeltaCoordinateOx);
                ShiftCoordinate(ref deltaPosition.z, dislocation.z, maxDeltaCoordinateOz);
            }
            this.gameObject.transform.position = SteamcopterManager.Instance.transform.position + differencePosition + deltaPosition;
            dislocation = Vector3.zero;
        }
    }

    public void ActivateGameMode()
    {
        isGameMode = true;
        this.gameObject.transform.rotation = Quaternion.Euler(gameModeRotation);
    }

    public void ActivateUpgradeMode()
    {
        gameModeRotation = this.gameObject.transform.rotation.eulerAngles;
        this.gameObject.transform.position = SteamcopterManager.Instance.transform.position + upgradeModeDeltaPoistion;
        this.gameObject.transform.rotation = Quaternion.Euler(upgradeModeRotation);
        isGameMode = false;
    }

    private void ShiftCoordinate(ref float deltaPosition_, float dislocation_, float maxDeltaCoordinate_)
    {
        if ((Mathf.Abs(deltaPosition_) - Mathf.Abs(maxDeltaCoordinate_)) > 0.001f)
        {
            deltaPosition_ -= Time.deltaTime * shiftPerSecond * Mathf.Sign(deltaPosition_);

            if (((-Mathf.Abs(deltaPosition_) + Mathf.Abs(maxDeltaCoordinate_)) > 0.001f) &&
                (Mathf.Sign(dislocation_) == Mathf.Sign(deltaPosition_)))
            {
                deltaPosition_ = Mathf.Abs(maxDeltaCoordinate_) * Mathf.Sign(deltaPosition_); 
            }
        }
        else
        {
            if ((-Mathf.Abs(deltaPosition_) + Mathf.Abs(maxDeltaCoordinate_)) > 0.001f)
            {
                deltaPosition_ += Time.deltaTime * shiftPerSecond * Mathf.Sign(dislocation_);
                if ((Mathf.Abs(deltaPosition_) - Mathf.Abs(maxDeltaCoordinate_)) > 0.001f)
                {
                    deltaPosition_ = Mathf.Abs(maxDeltaCoordinate_) * Mathf.Sign(deltaPosition_);
                }
            }
        }

        if ((Mathf.Abs(maxDeltaCoordinate_) < 0.01f) &&
            (Mathf.Abs(deltaPosition_)) < 0.01f)
        {
            deltaPosition_ = maxDeltaCoordinate_;
        }
        if (((Mathf.Abs(deltaPosition_) - Mathf.Abs(maxDeltaCoordinate_)) < 0.001f) &&
            (Mathf.Sign(deltaPosition_) != Mathf.Sign(maxDeltaCoordinate_)))
        {
            deltaPosition_ += Time.deltaTime * shiftPerSecond * Mathf.Sign(dislocation_);
        }
    }
}
