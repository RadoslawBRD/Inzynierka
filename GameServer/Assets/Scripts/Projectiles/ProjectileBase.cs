using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    /// <summary>
    /// ///////////////////////////////nazwa klasy jest inna ni¿ nazwa pliku
    /// </summary>
    public static Dictionary<int, ProjectileBase> projectiles = new Dictionary<int, ProjectileBase>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rigidBody;
    public int thrownByPlayer;
    public Vector3 initialForce;
    public float explosionRadius = 10f;
    public float explosionDamage = 10f;
    public string type = "";


    protected virtual void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        ServerSend.SpawnProjectile(this, thrownByPlayer, type);

        rigidBody.AddForce(initialForce);
        StartCoroutine(ExplodeAfterTime());
    }

    public void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
        
    }

    public virtual void Initialize(Vector3 _initialMovementDirection, float _initialForceStrenght, int _thrownByPlayer)
    {
        initialForce = _initialMovementDirection * _initialForceStrenght;
        thrownByPlayer = _thrownByPlayer;
    }

    private void Explode()
    {

        ServerSend.ProjectileExploded(this);

        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider _collider in _colliders)
        {
            if (_collider.CompareTag("Player"))
            {
                float dist = Vector3.Distance(_collider.gameObject.transform.position, transform.position);

                _collider.GetComponent<Player>().TakeDamage(explosionDamage * (1*explosionDamage/dist));
            }else
            if (_collider.CompareTag("Enemy"))
            {
                float dist = Vector3.Distance(_collider.gameObject.transform.position, transform.position);

                _collider.GetComponent<Enemy>().TakeDamage(explosionDamage * (1 * explosionDamage / dist));
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
