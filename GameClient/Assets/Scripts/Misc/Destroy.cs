using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Star
    // t is called before the first frame update
    private void Start()
    {
        
        StartCoroutine(DestroyItem());
    }
    private IEnumerator DestroyItem()
    {
        yield return new WaitForSeconds(2f);
        try
        {
            Destroy(gameObject);
        }
        catch
        {
            Debug.Log("Usuwam nieistniejący obiekt");
        }

    }
}
