using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventOnFieldManager : UnitySingleton<EventOnFieldManager>
{
    public GameObject effectOfDestroyingCommandCell;
    public GameObject effectOfDestroyingEnemy;
    public AudioClip soundEffectOfDestroying;

    protected override void Awake()
    {
        base.Awake();
    }

    public void VisualizeDestructionOfCommandCell(List<GameObject> commandCellList)
    {
        foreach (GameObject go in commandCellList)
        {
            GameObject gameObjectEffectOfDestroyingCommandCell = (GameObject)Instantiate(effectOfDestroyingCommandCell, go.transform.position, Quaternion.identity);
            gameObjectEffectOfDestroyingCommandCell.transform.parent = go.transform;
            AudioSource.PlayClipAtPoint(soundEffectOfDestroying, go.transform.position);
        }
    }

    public void VisualizeDestructionOfEnemy(GameObject gameObject)
    {
        GameObject gameObjectEffectOfDestroyingEnemy = (GameObject)Instantiate(effectOfDestroyingEnemy, gameObject.transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(soundEffectOfDestroying, gameObject.transform.position);

    }

    public void CreateDebris(GameObject gameObject)
    {
        for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            Transform transform = gameObject.transform.GetChild(i);
            Rigidbody rigidbody = transform.gameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
            else
            {
                rigidbody = transform.gameObject.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.drag = 0.2f;
                rigidbody.angularDrag = 0.1f;
            }
            rigidbody.velocity = gameObject.transform.GetComponent<Rigidbody>().velocity;
            rigidbody.AddForce(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * 3, ForceMode.Impulse);
            rigidbody.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), ForceMode.Impulse);

            BoxCollider boxCollider = transform.gameObject.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                
            }
            else
            {
                boxCollider = transform.gameObject.AddComponent<BoxCollider>();
                if (transform.gameObject.GetComponent<MeshFilter>() != null)
                {
                    boxCollider.size = transform.gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
                }
                else
                {
                    boxCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
                }
            }
            boxCollider.isTrigger = false;
            boxCollider.material = null;

            DebrisScript debrisScript = transform.gameObject.AddComponent<DebrisScript>();
            debrisScript.income = Mathf.RoundToInt(rigidbody.mass * (20f / gameObject.transform.childCount));
            debrisScript._rigidbody = rigidbody;

            transform.gameObject.tag = "Debris";
            transform.parent = null;
        }
    }
}
