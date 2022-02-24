using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public Vector3 upRecoil;
    Vector3 originalRotation;
    public GameObject gunPrefab;
    bool CR_running = false;
    public int bulletsCurrent = 30;
    private Animator animator;
    public PlayerWeaponState playerWeaponState = PlayerWeaponState.idle;


    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.localEulerAngles;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);
        animator = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        //gunPrefab.transform.parent = Camera.main.transform;
        BulletsCount.instance.updateCurrentBulets(bulletsCurrent);

        if (Input.GetButtonDown("Fire1"))
        {
            if (!CR_running)
                if (bulletsCurrent > 0)
                {
                    //AddRecoil();
                    SetState(PlayerWeaponState.fire);
                    bulletShoot();
                } else
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
            SetState(PlayerWeaponState.idle);
            //StopRecoil();
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
    public void SetState(PlayerWeaponState _state)
    {
        switch (_state)
        {
            case PlayerWeaponState.idle:
                animator.SetInteger("ChangeState", 1);
                break;
            case PlayerWeaponState.fire:
                animator.SetInteger("ChangeState", 2);
                break;
            case PlayerWeaponState.reload:
                animator.SetInteger("ChangeState", 3);
                break;
            case PlayerWeaponState.fullreload:
                animator.SetInteger("ChangeState", 4);
                break;
            default:
                animator.SetInteger("ChangeState", 1);
                break;
        }
        animator.SetInteger("ChangeState", 1);
    }
        private IEnumerator gunReload()
        {
        //AddRecoil();
        SetState(PlayerWeaponState.reload);

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
