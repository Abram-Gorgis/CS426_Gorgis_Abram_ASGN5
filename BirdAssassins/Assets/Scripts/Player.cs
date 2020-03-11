using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public CharacterController controller;
    public AudioSource tickSource;
    public AudioClip ammoPickupSound;
    public AudioClip shootSound;
    public float speed = 8f;            // Speed of player
    public float jumpHeight = 2f;       // Jump height of player
    public float gravity = -9.81f;      // Gravity constant for world

    public Transform groundCheck;       // Transform of our "feet"
    public float groundDistance = 0.4f; // Distance from ground that will be used to calculate whether we are grounded or not
    public LayerMask groundMask;        // Mask for layers to only change grounded when we jump on "Ground" objects

    bool isGrounded;                    // Boolean value describing the groundness of the player
    bool isDead;                        // Boolean value describing the dead/alive state of the player

    Vector3 velocity;                   // Velocity of the player

    public Animator anim;               // Animator of the player (Where we determined the different animation states)
    public GameObject lpObjects;         // Gameobject housing all the objects only a local player should see
    public GameObject goCamera;         // Gameobject of camera
    public GameObject[] bullets;        // List of bullets to shoot
    public GameObject gunBarrel;        // Barrel on the gun where is located for our "bullets" to start from
    public AmmoManager ammoManager;     // Ammo Manager for the UI

    int bulletIndex = 0;                // Index for bullets[] to determine which bullet we are shooting
    int ammoPerPack = 3;                // How much ammo a ammo pack gives when touched

    void Start()
    {
        anim = GetComponent<Animator>();
        tickSource = GetComponent<AudioSource>();
        if (isLocalPlayer)
        {
            lpObjects.SetActive(true);
            randomSpawnLocation(); 
        }
        else
            lpObjects.SetActive(false);

        
    }

    [Command]
    void CmdFire()
    {
        tickSource.PlayOneShot(shootSound, 0.7f);
        Vector3 aim = goCamera.transform.position + goCamera.transform.forward;
        gunBarrel.transform.LookAt(aim * 100f);
        GameObject newBullet = Instantiate(bullets[bulletIndex], gunBarrel.transform.position, Quaternion.identity);

        // Have the bullet be parentless so no gameobject will determine it's position
        newBullet.transform.parent = null;

        // Make the bullet aim at the direction of the ray
        newBullet.transform.LookAt(aim);
        newBullet.GetComponent<Rigidbody>().velocity = goCamera.transform.forward * 10f;

        Physics.IgnoreCollision(newBullet.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        newBullet.GetComponent<Bullet>().spawnedBy = netId;
        // Spawn bullet onto the clients
        NetworkServer.Spawn(newBullet);

        Destroy(newBullet, 1f);

    }

    // Function to randomly spawn players
    void randomSpawnLocation()
    {
        float PlaceX = Random.Range(-50, 50);
        float PlaceY = Random.Range(10, 20);
        float PlaceZ = Random.Range(-40, 40);
        controller.Move(new Vector3(PlaceX, PlaceY, PlaceZ));
    }


    // Update is called once per frame
    void Update()
    {
        if (hasAuthority == false || isLocalPlayer == false)
        {
            return;
        }

        if (isDead)
        {
            return;
        }

        isGrounded = rayCastFeet();

        if (isGrounded && velocity.y < 0)
        {
            print(isGrounded);
            velocity.y = -0f;
            anim.SetBool("isJumping", false);
        }
        if (!isGrounded)
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
        }

        // Movement Handling
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (PauseMenu.GameIsPaused)
        {
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        // Apply movement
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);


        // Jumping Handling
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
           
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger("isJumping");
        }


        // Shooting handling; Have to wait for jumping animation to end then we can shoot
        if (Input.GetButtonDown("Fire1") && isGrounded && (!anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")))
        {
            if (ammoManager.hasAmmo())
            {
                ammoManager.removeAmmo();
                CmdFire();
            }
            else
            {
                // Add text to warn user that they dont have anything to shoot
                return;
            }
        }


        // Animation handling
        if (move != Vector3.zero)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

    }


    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Player will get ammo when touching ammopack objects
        if (hit.gameObject.tag.Equals("AmmoPack"))
        {
            tickSource.PlayOneShot(ammoPickupSound, 0.7f);
            print(hit);
            Destroy(hit.gameObject);
          
            ammoManager.pickupAmmo(ammoPerPack);

        }
        else if(hit.gameObject.tag.Equals("JumpPad"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -6f * gravity);
        }
    
    }

    bool rayCastFeet()
    {
        bool leftfoot = Physics.Raycast(groundCheck.position + new Vector3(0, 0.3f, 0) + -groundCheck.right * .5f, -groundCheck.up, groundDistance, groundMask);
        bool rightfoot = Physics.Raycast(groundCheck.position + new Vector3(0, 0.3f, 0) + groundCheck.right * .5f, -groundCheck.up, groundDistance, groundMask);
        bool middle = Physics.Raycast(groundCheck.position + new Vector3(0, 0.3f, 0), -groundCheck.up, groundDistance, groundMask);
        return leftfoot || rightfoot || middle;
    }

    //Draw the Raycasts as a gizmo to show where it currently is testing. This is for testing purposes
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Draw a Ray forward from GameObject toward the maximum distance
        Gizmos.DrawRay(groundCheck.position + new Vector3(0, 0.3f, 0) + groundCheck.right * .5f, -groundCheck.up * groundDistance);
        Gizmos.DrawRay(groundCheck.position + new Vector3(0, 0.3f, 0) + -groundCheck.right * .5f, -groundCheck.up * groundDistance);
        Gizmos.DrawRay(groundCheck.position + new Vector3(0, 0.3f, 0), -groundCheck.up * groundDistance);
    }

    public bool hasDied()
    {
        return isDead;
    }

    public void playerDeath()
    {
        anim.SetBool("isDead", true);
        isDead = true;
    }

}
