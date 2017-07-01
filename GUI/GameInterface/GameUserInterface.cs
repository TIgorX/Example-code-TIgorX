using UnityEngine;
using System.Collections;

public class GameUserInterface : MonoBehaviour
{
    public InputManager inputManager;

    public GUITexture virtualGamepad;
    public GUITexture bigGunButton;
    public GUITexture rocketButton;
    public GUITexture superEquipmentButton;
    public GUITexture upgradeButton;

    private Camera gameCamera;

    private float lastCharging = 0;
    private float delayBetweenShots = 0.2f;

    private float lastEnable = 0;
    private float sleepTime = 0.2f;

    private ChosenWeapon chosenWeapon = ChosenWeapon.bigGun;

    void Awake()
    {
        gameCamera = GameCameraScript.Instance.gameObject.GetComponent<Camera>();

        upgradeButton.pixelInset = new Rect(-70, Screen.height / 1.75f, upgradeButton.pixelInset.width, upgradeButton.pixelInset.height);
        bigGunButton.pixelInset = new Rect(Screen.width - 100, Screen.height / 1.75f, bigGunButton.pixelInset.width, bigGunButton.pixelInset.height);
        rocketButton.pixelInset = new Rect(Screen.width - 100, Screen.height / 2.75f, rocketButton.pixelInset.width, rocketButton.pixelInset.height);
        superEquipmentButton.pixelInset = new Rect(Screen.width - 100, Screen.height / 5f, superEquipmentButton.pixelInset.width, superEquipmentButton.pixelInset.height);
    }

    void OnEnable()
    {
        IT_Gesture.onMouse1E += Pressed;
        IT_Gesture.onTouchE += Pressed;
        IT_Gesture.onShortTapE += ShortTap;
        IT_Gesture.onChargingE += Charging;
        lastEnable = Time.time;
    }

    void OnDisable()
    {
        IT_Gesture.onMouse1E -= Pressed;
        IT_Gesture.onTouchE -= Pressed;
        IT_Gesture.onShortTapE -= ShortTap;
        IT_Gesture.onChargingE -= Charging;
    }
	
	void Update () 
    {
        if ((lastEnable + sleepTime) > Time.time)
            return;

        if ((superEquipmentButton.gameObject.activeSelf) && (SteamcopterManager.Instance.superEquipment == null))
        {
            superEquipmentButton.gameObject.SetActive(false);
        }
        if ((!superEquipmentButton.gameObject.activeSelf) && (SteamcopterManager.Instance.superEquipment != null))
        {
            superEquipmentButton.gameObject.SetActive(true);
        }
        Vector3 moveDir = new Vector3(0, 0, 0);
        bool flag = false;

        if (Input.GetKey(KeyCode.W))
        {
            flag = true;
            moveDir += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            flag = true;
            moveDir += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            flag = true;
            moveDir += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            flag = true;
            moveDir += new Vector3(0, 0, -1);
        }

        if (flag)
        {
            if (!LevelLoader.LevelTrainingScriptCompleted)
                return;
            SteamcopterManager.Instance.SetMoveDirectory(moveDir.normalized);
        }	
	}

    void ShortTap(Vector2 Position)
    {
        if ((lastEnable + sleepTime) > Time.time)
            return;

        if (upgradeButton.HitTest(Position))
        {
            UpgradeButtonPressed();
            return;
        }
        if (virtualGamepad.HitTest(Position))
        {
            return;
        }
        if (bigGunButton.HitTest(Position))
        {
            chosenWeapon = ChosenWeapon.bigGun;
            bigGunButton.color = Color.white;
            rocketButton.color = Color.black;
            return;
        }
        if (rocketButton.HitTest(Position))
        {
            chosenWeapon = ChosenWeapon.rocket;
            bigGunButton.color = Color.black;
            rocketButton.color = Color.white;
            return;
        }
        if (superEquipmentButton.HitTest(Position))
        {
            SteamcopterManager.Instance.UseSuperEquipment();
            return;
        }
        if (chosenWeapon == ChosenWeapon.bigGun)
        {
            OpenFireAtPointBigGun(Position);
        }
        if (chosenWeapon == ChosenWeapon.rocket)
        {
            OpenFireAtPointRocket(Position);
        }
    }

    void Charging(ChargedInfo chargedInfo)
    {
        if ((lastEnable + sleepTime) > Time.time)
            return;

        if (upgradeButton.HitTest(chargedInfo.pos))
        {
            return;
        }
        if (virtualGamepad.HitTest(chargedInfo.pos))
        {
            return;
        }
        if (bigGunButton.HitTest(chargedInfo.pos))
        {
            return;
        }
        if (rocketButton.HitTest(chargedInfo.pos))
        {
            return;
        }
        if (superEquipmentButton.HitTest(chargedInfo.pos))
        {
            return;
        }
        if (((lastCharging + delayBetweenShots) < Time.time) &&
            (chosenWeapon == ChosenWeapon.bigGun))
        {
            OpenFireAtPointBigGun(chargedInfo.pos);
            lastCharging = Time.time;
        }
        if (((lastCharging + delayBetweenShots) < Time.time) &&
            (chosenWeapon == ChosenWeapon.rocket))
        {
            OpenFireAtPointRocket(chargedInfo.pos);
            lastCharging = Time.time;
        }
    }

    void Pressed(Vector2 Position)
    {
        if ((lastEnable + sleepTime) > Time.time)
            return;

        if (!LevelLoader.LevelTrainingScriptCompleted)
            return;

        if (virtualGamepad.HitTest(Position))
        {
            VirtualGamepadPressed(Position);
            return;
        }
    }

    void UpgradeButtonPressed()
    {
        inputManager.ActivateUpgradeInterface();
        TrainingScript.Instance.OpenedModernizationMenu();
    }

    void OpenFireAtPointBigGun(Vector2 Position)
    {
        Vector3 targetPosition = (gameCamera.ScreenToWorldPoint(new Vector3(Position.x, Position.y, gameCamera.gameObject.transform.position.y)));
        SteamcopterManager.Instance.ShootBigGun((targetPosition));
    }

    void OpenFireAtPointRocket(Vector2 Position)
    {
        Vector3 targetPosition = (gameCamera.ScreenToWorldPoint(new Vector3(Position.x, Position.y, gameCamera.gameObject.transform.position.y)));
        SteamcopterManager.Instance.ShootRocket((targetPosition));
    }

    void VirtualGamepadPressed(Vector2 Position)
    {
        Vector3 moveDir = Vector3.zero;
        Rect screenRect = virtualGamepad.GetScreenRect();
        Vector2 center = new Vector2((screenRect.xMin + screenRect.xMax) / 2,
            (screenRect.yMin + screenRect.yMax) / 2);
        moveDir = new Vector3((Position - center).x, 0, (Position - center).y);
        moveDir = moveDir.normalized;
        SteamcopterManager.Instance.SetMoveDirectory(moveDir);
        GameCameraScript.Instance.dislocation = moveDir;
    }

    public enum ChosenWeapon
    {
        rocket,
        bigGun,
        superEquipment,
    }
}
