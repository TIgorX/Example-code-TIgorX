using UnityEngine;
using System.Collections;

public class ExperimentScript : MonoBehaviour 
{
    public GameObject prefab;
    public Vector2 startPoint = new Vector2(0, 0);
    public Vector2 finishPoint = new Vector2(4, 8);
    public Vector2 motionVector = new Vector2(1.5f, 1);

    private int iteration = 20;
    public float maxForce = 0.4f;
    public float decelerationRate = 0.2f;
    public float l;
    public float angleA;
    public float angleB;

    public void PerformStepOfAlgorithm()
    {
        Vector2 decelerationedMotionVector = motionVector * (1 - decelerationRate);

        Vector2 start = startPoint + motionVector;

        for (int i = 0; i < iteration; i++)
        {
            Vector2 kVect = (finishPoint - start).normalized * maxForce * (1 - i / (iteration*1f));
            float angleb = Vector2.Angle(((decelerationedMotionVector + kVect) + start - finishPoint), (start - finishPoint));
            float anglea = Vector2.Angle(((decelerationedMotionVector + kVect)), (-start + finishPoint));

            if ((finishPoint - start).magnitude < 0.1)
            {
                Debug.Log("Finish");
                return;
            }

            float l1 = (decelerationedMotionVector + kVect).magnitude;
            float l2 = (((decelerationedMotionVector + kVect) + start) - finishPoint).magnitude;

            if (((((angleb + anglea) < (angleB+ angleA))))
        || (i == (iteration-1)))
            {
                Debug.Log(i);
                motionVector = decelerationedMotionVector + kVect;
                startPoint = start;
                angleB = angleb;
                angleA = anglea;
                break;
            }
        }

        GameObject point = (GameObject)Instantiate(prefab, new Vector3((motionVector + startPoint).y, 0, (motionVector + startPoint).x), Quaternion.identity);
    }

    public void Init()
    {
        angleA = Vector2.Angle((finishPoint - startPoint), motionVector);
        angleB = Vector2.Angle((-finishPoint + startPoint), ((startPoint+motionVector) -finishPoint));

        GameObject point = (GameObject)Instantiate(prefab, new Vector3((motionVector + startPoint).y, 0, (motionVector + startPoint).x), Quaternion.identity);
        point = (GameObject)Instantiate(prefab, new Vector3((finishPoint).y, 0, (finishPoint).x), Quaternion.identity);
        point = (GameObject)Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

	void Start () 
    {
        Init();	
	}
	
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            PerformStepOfAlgorithm();
        }
	}
}
