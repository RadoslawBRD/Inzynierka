using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject floatingUsernamePrefab;

    //public GameObject gunPrefab;

    public int id;
    public string username;
    public float health;
    public float maxHealth=100;
    public MeshRenderer model;
    public int itemCount = 0;
    public bool isMaster = false;
    public int moneyCount=0;
   

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        
        StartCoroutine(createFloatingNickname());
        //StartCoroutine(GunInit());
        /*BulletsCount.instance.ammoDisplay.gameObject.SetActive(true);
        HealthCount.instance.healthDisplay.gameObject.SetActive(true);
        GranadeCount.instance.granadeDisplay.gameObject.SetActive(true);*/
        UIManager.instance.playerHUD.SetActive(true);
    }

    public void SetHealth(float _health, int _id)
    {
        health = _health;

        if(health <= 0f)
        {
            Die();
        }
        if(_id==Client.instance.myId)
            HealthCount.instance.SetHealth(health);
    }
    public void Update()
    {
        if (isMaster)
            UIManager.instance.setMasterUI(isMaster, id);
        else
            UIManager.instance.setMasterUI(isMaster, id);

    }
    public void Die()
    {
        //StartCoroutine(createFloatingNickname());
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
    /*private IEnumerator GunInit()
    {
        yield return new WaitForSeconds(1f);

        if (gunPrefab != null)
        {
            var gun = Instantiate(gunPrefab,transform.position, Quaternion.identity);
            gun.transform.parent = GameManager.players[id].transform;
        }
    }*/
   
    public void RemoveFloatingNickname()
    {
        FloatingNickname.instance.Destroy();
     }
    public void Respawn()
    {
        model.enabled = true;
        RevertInventory();
    }
    public void SetWeaponState()
    {

    }
    public void RevertInventory()
    {
        SetHealth(maxHealth, id);
        MoneyCount.instance.setMoney(30);
        BulletsCount.instance.setCurrentBulets(30);
        BulletsCount.instance.setMaxBulets(0);
        GranadeCount.instance.setGranadeAmount(-100);
    }
    public void ChangeVisibilityOfWeapon(bool _value)
    {
        foreach (SkinnedMeshRenderer x in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            x.enabled = !_value;

    }
}
