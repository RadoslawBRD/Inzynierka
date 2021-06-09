using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    GameObject projetcilePrefab;
    Vector3 position;

    public int id;
    public Rigidbody rigidBody;
    public int thrownByPlayer;
    public Vector3 initialForce;
    public float explosionRadius = 10f;
    public float explosionDamage = 10f;


    private void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);
        GameObject projectile = Instantiate(projetcilePrefab, position, Quaternion.identity);
        projectile.GetComponent<ProjectileManager>().Initialize(id);

        rigidBody.AddForce(initialForce);
        StartCoroutine(ExplodeAfterTime());

    }

    public void FixedUpdate()
    {
        
        if (GameManager.projectiles.TryGetValue(id, out ProjectileManager projectile))
            projectile.transform.position = position;

    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();

    }

    public void Initialize(Vector3 _initialMovementDirection, float _initialForceStrenght)
    {
        initialForce = _initialMovementDirection * _initialForceStrenght;
    }

    private void Explode()
    {

        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider _collider in _colliders)
        {
            if (_collider.CompareTag("Player"))
            {
                float dist = Vector3.Distance(_collider.gameObject.transform.position, transform.position);

                _collider.GetComponent<OfflinePlayerManager>().TakeDamage(explosionDamage * (1 * explosionDamage / dist));
            }
            

        }
        projectiles.Remove(id);
        Destroy(gameObject);
    }
    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(5f);
        Explode();
    }
}
