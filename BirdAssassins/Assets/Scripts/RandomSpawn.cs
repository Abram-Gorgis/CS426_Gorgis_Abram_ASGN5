using UnityEngine;
using UnityEngine.Networking;

public class RandomSpawn : NetworkBehaviour
{
    public GameObject AmmoPack;
    public float PlaceX;
    public float PlaceZ;
    private bool spawnOnce = false;


    [Command]
    void CmdSpawnAmmo()
    {
        for (int i = 0; i < 25; i++)
        {
            PlaceX = Random.Range(-50, 50);
            PlaceZ = Random.Range(-40, 40);
            GameObject ammoSpawn = Instantiate(AmmoPack, new Vector3(PlaceX, 1, PlaceZ), Quaternion.identity);
            NetworkServer.Spawn(ammoSpawn);
        }
    }

    void Start()
    {
        CmdSpawnAmmo();
    }

    // Update is called once per frame
    void Update()
    {


    }



}