using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public Vector3 upRecoil;
    Vector3 originalRotation;
    public GameObject gunPrefab;
    bool CR_running = false;
    public int bulletsCurrent=5;
    

    
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
            if(!CR_running)
                if (bulletsCurrent > 0) 
                {
                    AddRecoil();
                    bulletShoot();
                }else
                {

                    if (BulletsCount.instance.bulletsMax > 0)
                    {
                        CR_running = true;
                        StartCoroutine(gunReload());
                    }
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
        if (BulletsCount.instance.bulletsMax <= 30)
        {
            bulletsCurrent = BulletsCount.instance.bulletsMax;
            BulletsCount.instance.updateMaxBulets(-30);

        }
        else
        {
            
            BulletsCount.instance.updateMaxBulets(bulletsCurrent - 30);
            bulletsCurrent = 30;
        }
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);
        StopRecoil();

        CR_running = false;

        //StopRecoil();

    }
}
