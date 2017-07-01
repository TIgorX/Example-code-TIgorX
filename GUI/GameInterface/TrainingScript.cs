using UnityEngine;
using System.Collections;

public class TrainingScript : UnitySingleton<TrainingScript>
{
    public GameObject pointerObject;
    public GameObject panelObject;

    public TapToShootObjectScript tapToShootObject;
    public PressWASDToMoveObjectScript pressWASDToMoveObject;

    public tk2dTextMesh textPanel;
    public int scriptStage = 0;
    private float _time = -1;

    protected override void Awake()
    {
        base.Awake();
    }

    private void VisualizePointer(float Rotation, string Text, int BackgroundNumber)
    {
        pointerObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Rotation));

        textPanel.text = Text;
        textPanel.Commit();

        if (BackgroundNumber == 1)
        {
            pointerObject.SetActive(true);
            panelObject.SetActive(true);
        }
        else
        {
            pointerObject.SetActive(false);
            panelObject.SetActive(true);
        }
    }

    private void VisualizePointer(Vector3 Position, float Rotation, string Text, int BackgroundNumber)
    {
        pointerObject.transform.localPosition = Position + new Vector3(0, 0, pointerObject.transform.localPosition.z);
        panelObject.transform.localPosition = Position + new Vector3(0, 0, panelObject.transform.localPosition.z);
        VisualizePointer(Rotation, Text, BackgroundNumber);
    }

    public void OpenedModernizationMenu()
    {
        if (scriptStage == 1)
        {
            scriptStage = 2;
            VisualizePointer(new Vector3(-2.0f, 0, 0), 0, "Select\nedge space\nto add\nnew cell", 1);
        }

        if (scriptStage == 4)
        {
            scriptStage = 5;
            VisualizePointer(new Vector3(-2.0f, 0, 0), 0, "Select\nnew cell", 2);
        }
    }

    public void SelectedNewCell()
    {
        if (scriptStage == 2)
        {
            scriptStage = 3;
            VisualizePointer(0, "Tap again\nto confirm", 2);
        }
    }

    public void AddedNewCell()
    {
        if (scriptStage == 3)
        {
            scriptStage = 5;
            VisualizePointer(0, "Select\nnew cell", 2);
        }
    }

    public void SelectedFreeCell()
    {
        if (scriptStage == 5)
        {
            scriptStage = 6;
            VisualizePointer(new Vector3(2.5f, 2, 0), 0, "Select category", 1);
        }
    }

    public void SelectedCategory()
    {
        if (scriptStage == 6)
        {
            scriptStage = 7;
            VisualizePointer(0, "Select module", 1);
        }
    }

    public void SelectedModule()
    {
        if (scriptStage == 7)
        {
            scriptStage = 8;
            VisualizePointer(0, "Tap again\nto confirm", 2);
        }
    }

    public void AddedNewModule()
    {
        if (scriptStage == 8)
        {
            scriptStage = 9;
            VisualizePointer(new Vector3(3.7f, -0.5f, 0), -90, "Return\nto battle", 1);
        }
    }

    public void ReturnedToGame()
    {
        if (scriptStage == 10)
            return;

        if (scriptStage == 9)
        {
            scriptStage = 10;
            pointerObject.SetActive(false);
            panelObject.SetActive(false);
            textPanel.gameObject.SetActive(false);
            tapToShootObject.gameObject.SetActive(true);
            tapToShootObject.MoveToPoint(new Vector3(5, 0, 10));
            return;
        }

        if (scriptStage >= 5)
        {
            scriptStage = 4;
            VisualizePointer(new Vector3(-2.2f, 2f, 0), 180, "Enter\nmodernization\nmenu", 1);
            return;
        }

        if (scriptStage >= 2)
        {
            scriptStage = 1;
            VisualizePointer(new Vector3(-2.2f, 2f, 0), 180, "Enter\nmodernization\nmenu", 1);
            return;
        }
    }

    public void DestroyedTapToShootObject()
    {
        pressWASDToMoveObject.gameObject.SetActive(true);
        pressWASDToMoveObject.MoveToPoint(new Vector3(10, 0, 10));
        _time = Time.time;
        LevelLoader.LevelTrainingScriptCompleted = true;
    }

	void Update () 
    {
        if ((scriptStage == 0) && (LevelLoader.LevelLoadScriptCompleted) && (!LevelLoader.LevelTrainingScriptCompleted))
        {
            scriptStage = 1;
            pointerObject.SetActive(true);
            textPanel.gameObject.SetActive(true);
            VisualizePointer(new Vector3(-2.2f,2f,0), 180, "Enter\nmodernization\nmenu", 1);
        }

        if ((_time != -1) && ((_time + 10) < Time.time))
        {
            _time = -1;
            LevelLoader.loadScriptPreparationDone = true;
        }	
	}
}
