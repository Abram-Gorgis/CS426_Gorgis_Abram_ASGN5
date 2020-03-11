using UnityEngine;
using UnityEngine.Networking;
public class PlayerSetup : NetworkBehaviour
{
    public Behaviour[] componentsToDisable;
    Camera mainCamera;
    // disable scenen overhead camera when player is loaded
    void Start()
    {
        if(isLocalPlayer == false){
            for(int i = 0; i < componentsToDisable.Length; ++i){
                componentsToDisable[i].enabled = false;
            }
        }
        else{
            mainCamera = Camera.main;
            if(mainCamera != null){
                mainCamera.gameObject.SetActive(false);
            }
        }
    }

    void OnDisable(){
        if(mainCamera != null){
            mainCamera.gameObject.SetActive(true);
        }
    }
}
