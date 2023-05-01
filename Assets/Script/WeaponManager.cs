using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManager
{
    private Collider weaponColL;
    private Collider weaponColR;
    public GameObject whL;
    public GameObject whR;

    public WeaponController wcL;
    public WeaponController wcR;


    private void Start()
    {
        whL = this.transform.DeepFind("weaponHandleL").gameObject;
        weaponColL = whL.GetComponentInChildren<Collider>();
        wcL = BindWeaponController(whL);

        whR = this.transform.DeepFind("weaponHandleR").gameObject;
        weaponColR = whR.GetComponentInChildren<Collider>();
        wcR = BindWeaponController(whR);
    }

    public WeaponController BindWeaponController(GameObject obj)
    {
        WeaponController tempWc;
        tempWc = obj.GetComponent<WeaponController>(true);
        tempWc.wm = this;
        return tempWc;
    }
    public void WeaponEnable()
    {
        if (am.ac.CheckStateTag("AttackL"))
        {
            weaponColL.enabled = true;
        }
        else
        {
            weaponColR.enabled = true;
        }
    }
    public void WeaponDisable()
    {
        weaponColL.enabled = false;
        weaponColR.enabled = false;
    }

    public void CounterBackEnable()
    {
        am.SetIsCounterBack(true);
    }
    public void CounterBackDisable()
    {
        am.SetIsCounterBack(false);
    }
}
