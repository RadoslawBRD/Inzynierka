using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public Vector3 upRecoil;
    Vector3 originalRotation;
    public GameObject gunPrefab;
    bool CR_running = false;
    public int bulletsCurrent;
    private Animator animator;
    public PlayerWeaponState playerWeaponState = PlayerWeaponState.idle;
    private bool objectLoaded = false;


    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.localEulerAngles;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);

    }

    // Update is called once per frame
    void Update()
    {
        if(!objectLoaded)
        {
            animator = gameObject.GetComponentInChildren<Animator>();
            objectLoaded = true;


        }
        //gunPrefab.transform.parent = Camera.main.transform;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);

        if (Input.GetButtonDown("Fire1"))
        {
            if (CR_running || this.animator.GetCurrentAnimatorStateInfo(0).IsName("Fire"))
            {

            }
            else
            {
                if (bulletsCurrent > 0)
                {
                    //AddRecoil();
                    SetState(PlayerWeaponState.fire);
                    bulletShoot();
                }
                else
                {

                    if (BulletsCount.instance.bulletsMax > 0)
                    {
                        ClientSend.PlayerSendReload();
                        CR_running = true;
                        StartCoroutine(gunReload());
                    }
                }
            }

        }
        else if (Input.GetButtonUp("Fire1") && bulletsCurrent > 0)
        {
            //SetState(PlayerWeaponState.idle);
            //StopRecoil();
        }
    }
   private void bulletShoot()
    {
        bulletsCurrent--;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);

    }
    public void SetState(PlayerWeaponState _state)
    {
        switch (_state)
        {
            case PlayerWeaponState.idle:
                animator.SetInteger("ChangeState", 1);
                break;
            case PlayerWeaponState.fire:
                Debug.Log("STRZELAM################");
                animator.SetTrigger("Shoot");
                break;
            case PlayerWeaponState.reload:
                Debug.Log("RELOAD################################");
                animator.SetInteger("ChangeState", 3);
                break;
            case PlayerWeaponState.fullreload:
                animator.SetInteger("ChangeState", 4);
                break;
            default:
                animator.SetInteger("ChangeState", 1);
                break;

        }
        //animator.SetInteger("ChangeState", 1);
    }
        private IEnumerator gunReload()
        {
        //AddRecoil();
        SetState(PlayerWeaponState.reload);

        yield return new WaitForSeconds(2.5f);
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
            SetState(PlayerWeaponState.idle);

        CR_running = false;

            //StopRecoil();
        }
    }

public enum PlayerWeaponState
{
    idle,
    fire,
    reload,
    fullreload
}
