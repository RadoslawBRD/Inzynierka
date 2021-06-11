using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public Vector3 upRecoil;
    Vector3 originalRotation;
    public GameObject gunPrefab;

    public int bulletsCurrent=5;
    public int bulletsMax=30;
    

    
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.localEulerAngles;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);
    }

    // Update is called once per frame
    void Update()
    {
        //gunPrefab.transform.parent = Camera.main.transform;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);

        if (Input.GetButtonDown("Fire1"))
        {
            if (bulletsCurrent > 0) 
            {
                AddRecoil();
                bulletShoot();
            }else
            {
                StartCoroutine(gunReload());
            }

        }
        else if (Input.GetButtonUp("Fire1") && bulletsCurrent > 0)
        {
           StopRecoil();
        }
    }
    private void AddRecoil()
    {
        transform.localEulerAngles += upRecoil;
    }

    private void StopRecoil()
    {
        transform.localEulerAngles = originalRotation;

    }
    private void bulletShoot()
    {
        bulletsCurrent--;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);

    }
    private IEnumerator gunReload()
    {
        //AddRecoil();
        yield return new WaitForSeconds(1f);
        bulletsCurrent = bulletsMax;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);
        BulletsCount.instance.updateMaxBulets(bulletsMax);

        //StopRecoil();

    }
}
