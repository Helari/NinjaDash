using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreTransform
{
    public Vector3 position;
    public Quaternion rotation;
    //public Vector3 localScale;
}

public class ND_DismemberManager : MonoBehaviour
{

    public List<Transform[,]> originalParent = new List<Transform[,]>();
    public List<Rigidbody> rigidBodies = new List<Rigidbody>();
    public StoreTransform[] initialTransforms;

    // Use this for initialization
    void Start()
    {
        InitMembers();
    }

    public void InitMembers()
    {
        Rigidbody[] myRigids = gameObject.GetComponentsInChildren<Rigidbody>(); //Get all body parts
        initialTransforms = new StoreTransform[myRigids.Length]; //Init transform tab size

        foreach (Rigidbody rigid in myRigids)
        {
            Transform[,] test = new Transform[1, 2]; //Store 3  transforms
            test[0, 0] = rigid.transform; //The current transform that will be edited
            test[0, 1] = rigid.transform.parent.transform; //The parent to reset links at default when the enemy is pooled back
            // test[0, 2] = rigid.transform; //A copy of the initial transform to reset the body parts to their initial positions
            originalParent.Add(test); //store the infos
            rigidBodies.Add(rigid);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            Explode(Vector3.zero);
        }
        else if (Input.GetKeyUp(KeyCode.O))
        {
            ResetSkelton();
        }
    }

    public void Explode(Vector3 explosionPosition)
    {
        int i = 0;
        foreach (Transform[,] t in originalParent)
        {
            //create a save of the initial positions
            initialTransforms[i] = new StoreTransform();
            initialTransforms[i].position = t[0, 0].transform.position;
            initialTransforms[i].rotation = t[0, 0].transform.rotation;
            //initialTransforms[i].localScale = t[0, 0].transform.localScale;

            //Then explode the body parts (unlink bones hierarchy and apply gravity)
            t[0, 0].transform.parent = this.transform;

            rigidBodies[i].useGravity = true;
            rigidBodies[i].isKinematic = false;
            rigidBodies[i].AddExplosionForce(250.0f, explosionPosition, 150.0f);
            rigidBodies[i].AddTorque(new Vector3(Random.Range(3.0f, 10.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)), ForceMode.Impulse);
            //t[0, 0].GetComponent<Rigidbody>().AddForce(new Vector3(Random.RandomRange(3.0f, 10.0f), Random.RandomRange(-5.0f, 5.0f), Random.RandomRange(-5.0f, 5.0f)), ForceMode.Impulse);
            i++;
        }
    }
    public void ResetSkelton()
    {
        int i = 0;
        foreach (Transform[,] t in originalParent)
        {
            t[0, 0].transform.parent = t[0, 1].transform;
            rigidBodies[i].useGravity = false;
            rigidBodies[i].isKinematic = true;
            if (initialTransforms[i] != null)
            {
                t[0, 0].transform.position = initialTransforms[i].position;//t[0, 2].transform.position;
                t[0, 0].transform.rotation = initialTransforms[i].rotation;//t[0, 2].transform.rotation;
                //t[0, 0].transform.localScale = initialTransforms[i].localScale;//t[0, 2].transform.rotation;
            }
            i++;
        }
    }
}
