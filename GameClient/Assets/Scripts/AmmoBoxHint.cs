using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxHint : MonoBehaviour
{
    public BoxCollider collider;
    public GameObject hint;



    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ShowHint());

    }
    private void OnT(Collision collision)
    {
        // hint.SetActive(true);
        Debug.Log("Collision ENter");
        StartCoroutine(ShowHint());
    }
   // private void OnCollisionExit(Collision collision)
    //{
     //   hint.SetActive(false);
   // }

    private IEnumerator ShowHint()
    {
        //GameObject.Find("HintWindow").SetActive(true);
        hint.SetActive(true);

        yield return new WaitForSeconds(1f);
       // GameObject.Find("HintWindow").SetActive(false);

        hint.SetActive(false);

    }
}

