using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AmmoManager : MonoBehaviour
{
    public int startingAmmo = 3;
    public int maxAmmo = 12;
    public int ammo = 0;
    public Text AmmoUI;

    // Start is called before the first frame update
    void Start()
    {
        ammo = startingAmmo;
        updateAmmo();
    }

    public void removeAmmo()
    {
        //Debug.Log("AmmoManager::removeAmmo -- removing ammo");
        ammo--;
        updateAmmo();
    }

    public bool hasAmmo()
    {
        return ammo == 0 ? false : true;
    }

    public void pickupAmmo(int ammoLooted)
    {
        //Debug.Log("AmmoManager::pickupAmmo -- picking up ammo");
        ammo = Mathf.Min(ammo + ammoLooted, maxAmmo); ;
        updateAmmo();
    }

    private void updateAmmo()
    {
        //Debug.Log("AmmoManager::updateAmmo -- updating ammo " + ammo);
        AmmoUI.text = ammo + " / " + maxAmmo;
    }
}
