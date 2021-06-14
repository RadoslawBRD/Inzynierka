using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public int id;
    public GameObject explosionPrefab;


    public virtual void Initialize(int _id)
    {
        id = _id;

    }

    public virtual void Explode(Vector3 _position)
    {
        transform.position = _position;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GameManager.projectiles.Remove(id);
        Destroy(gameObject);
    }


    private IEnumerator DestroyItem()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    
    }
}
