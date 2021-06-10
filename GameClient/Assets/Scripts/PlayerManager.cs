using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject floatingUsernamePrefab;
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public MeshRenderer model;
    public int itemCount = 0;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        StartCoroutine(createFloatingNickname());
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if(health <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        StartCoroutine(createFloatingNickname());
        model.enabled = false;
    }
    private IEnumerator createFloatingNickname()
    {
        yield return new WaitForSeconds(1f);

        if (floatingUsernamePrefab != null)
        {
            var floatingText = Instantiate(floatingUsernamePrefab, new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z), Quaternion.identity);
            floatingText.GetComponent<TextMesh>().text = username;
            floatingText.transform.parent = GameManager.players[id].transform;
        }
    }
    public void RemoveFloatingNickname()
    {
        FloatingNickname.instance.Destroy();

    }
    public void Respawn()
    {
        model.enabled = true;
        createFloatingNickname();
        SetHealth(maxHealth);
    }
}
