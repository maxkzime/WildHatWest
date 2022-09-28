using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerRigidBody;

    private float floorLimit;

    private void Awake()
    {
        // Initialize speed.
        speed = maxSpeed;

        lastTimeFootprintsGeneration = Time.time;
        lastTimeFootprintsDeletion = Time.time;

        // Grab player's rigidbody.
        playerRigidBody = GetComponent<Rigidbody2D>();

        // Out of bounds security.
        floorLimit = ObjectsGeneration.Instance.GetFloorSize() / 2;

    }



    private void Update()
    {
        GenerateFootPrints();

        FlippingPlayerWithDirection();

        PlayerWalkingSoundEffectManager();
    }



    #region /*** Create dust for when running ***/
    [SerializeField] private ParticleSystem dustRunParticleSystem;

    private async void CreateDust()
    {
        dustRunParticleSystem.Play();
        await Task.Delay(20);
    }
    #endregion /*** ^^^ Create dust for when running ^^^ ***/




    #region /*** Player movements manager. ***/
    private void FixedUpdate()
    {
        bool isDeath = GetComponent<PlayerStats>().GetIsDeath();

        if (!isDeath)
        {
            PlayerMovementsManager();
        }

        // Set animator parameters.
        // Set Run to true if there's input and is not dead.
        GetComponent<Animator>().SetBool("Run", (horizontalInput != 0 || verticalInput != 0) && !isDeath);
    }


    private float maxSpeed = 9.0f;
    private float speed;
    private float horizontalInput;
    private float verticalInput;
    private Vector2 targetSpeed;
    private Vector2 speedDif;
    private Vector2 accelerationRate;
    private Vector2 movement;
    private Vector3 currentPos;
    private bool wasRunning = false;

    private void PlayerMovementsManager()
    {
        // Get the horizontal axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1.
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");


        //Calculate the direction we want to move in and our desired velocity
        targetSpeed.x = horizontalInput * speed;
        targetSpeed.y = verticalInput * speed;


        //Calculate difference between current velocity and desired velocity
        speedDif.x = targetSpeed.x - playerRigidBody.velocity.x;
        speedDif.y = targetSpeed.y - playerRigidBody.velocity.y;


        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        float runAccelAmount = 50 * 12.5f / maxSpeed;
        float runDeccelAmount = 50 * 1.5f / maxSpeed;


        accelerationRate.x = (Mathf.Abs(targetSpeed.x) > speed / 5) ? runAccelAmount : runDeccelAmount;
        accelerationRate.y = (Mathf.Abs(targetSpeed.y) > speed / 5) ? runAccelAmount : runDeccelAmount;


        // Calculate force along x-axis to apply to thr player.
        movement.x = speedDif.x * accelerationRate.x;
        movement.y = speedDif.y * accelerationRate.y;


        playerRigidBody.AddForce(
            force: movement,
            mode: ForceMode2D.Force);


        #region /*** Out of bounds security. ***/
        currentPos = transform.localPosition;

        if (currentPos.y > floorLimit ||
            currentPos.y < -floorLimit ||
            currentPos.x > floorLimit ||
            currentPos.x < -floorLimit)
        {
            Vector3 directionToCenter = Vector3.zero - transform.position;

            transform.Translate(
                translation: 5 * speed * Time.deltaTime * directionToCenter.normalized,
                relativeTo: Space.World);
        }
        #endregion /*** ^^^ Out of bounds security. ^^^ ***/


        #region /*** Create dust when the player stop running + take a turn. ***/
        if (horizontalInput == Mathf.Sign(horizontalInput) * 1 || verticalInput == Mathf.Sign(verticalInput) * 1)
        {
            wasRunning = true;
        }

        if (wasRunning && (horizontalInput == 0 || verticalInput == 0))
        {
            wasRunning = false;
            CreateDust();
        }
        #endregion /*** ^^^ Create dust when the player stop running + take a turn. ^^^ ***/
    }
    #endregion /*** ^^^ Player movements manager. ^^^ ***/




    #region /*** Player walking sound effect. ***/
    private void PlayerWalkingSoundEffectManager()
    {
        // Play sound effect if walking.
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            SoundManager.Instance.PlayPlayerWalkingSoundEffect();
        }

        // Stop sound effect if not walking.
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            SoundManager.Instance.StopPlayerWalkingSoundEffect();
        }
    }
    #endregion /*** ^^^ Player walking sound effect. ^^^ ***/




    #region /*** Footprints generation. ***/
    [SerializeField] private GameObject footprint;

    private readonly int maxFootprints = 14;

    public GameObject footprintsEmpty = null;

    private Queue<GameObject> footprints = new();

    private float lastTimeFootprintsGeneration = 0.0f;
    private float lastTimeFootprintsDeletion = 0.0f;

    private void GenerateFootPrints()
    {
        if (!GetComponent<PlayerStats>().GetIsDeath())
        {
            if (footprintsEmpty == null)
            {
                // Only use to store footprints (not child to player, otherwise it will move with it).
                footprintsEmpty = new GameObject("Player's Footprints");
            }


            if (horizontalInput <= -0.01f || verticalInput <= -0.01f ||
                horizontalInput >= 0.01f || verticalInput >= 0.01f)
            {
                if (lastTimeFootprintsGeneration + 0.1f < Time.time)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject go =
                            Instantiate(
                                original: footprint,
                                position:
                                new Vector3(
                                    x: transform.position.x + (i * 0.15f),
                                    y: transform.position.y - GetComponent<BoxCollider2D>().size.y - 0.4f + (0.2f * i),
                                    z: transform.position.z),
                                rotation: Quaternion.identity);

                        footprints.Enqueue(go);

                        go.transform.SetParent(footprintsEmpty.transform);
                    }


                    if (footprints.Count + 2 >= maxFootprints)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            GameObject go = footprints.Peek();

                            footprints.Dequeue();

                            Destroy(go);

                            footprints.TrimExcess();
                        }
                    }

                    lastTimeFootprintsGeneration = Time.time;
                }
            }
            else if (lastTimeFootprintsDeletion + 0.2f < Time.time &&
                footprints.Count >= 2)
            {
                // Remove footprints gradually when the player is not moving.
                for (int i = 0; i < 2; i++)
                {
                    GameObject go = footprints.Peek();

                    footprints.Dequeue();

                    Destroy(go);

                    footprints.TrimExcess();
                }

                lastTimeFootprintsDeletion = Time.time;
            }
        }
    }
    #endregion /*** ^^^ Footprints generation. ^^^ ***/




    #region /*** Flip player accordingly with his direction (cursor position). ***/
    public bool loopRunningToRight = false;
    public bool loopRunningToLeft = false;

    private void FlippingPlayerWithDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z += Camera.main.nearClipPlane;

        if (mousePosition.x < transform.position.x &&
            horizontalInput < -0.01f && transform.localScale.x > -5 && !loopRunningToLeft && !loopRunningToRight)
        {
            loopRunningToLeft = true;
        }
        else if (mousePosition.x > transform.position.x &&
            horizontalInput > 0.01f && transform.localScale.x < 5 && !loopRunningToRight && !loopRunningToLeft)
        {
            loopRunningToRight = true;
        }

        if (loopRunningToLeft)
        {
            transform.localScale = new Vector3(transform.localScale.x - 1.0f, transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x <= -5)
            {
                loopRunningToLeft = false;

                // Create dust when turning end.
                CreateDust();
            }
        }

        if (loopRunningToRight)
        {
            transform.localScale = new Vector3(transform.localScale.x + 1.0f, transform.localScale.y, transform.localScale.z);


            if (transform.localScale.x >= 5)
            {
                loopRunningToRight = false;

                // Create dust when turning end.
                CreateDust();
            }
        }
    }
    #endregion /*** ^^^  Flip player accordingly with his direction (cursor position). ^^^ ***/




    #region /*** Speed modifiers collisions. ***/
    private void OnTriggerEnter2D(Collider2D collider)
    {
        #region /*** Player is slower on small rock. ***/
        if (collider.gameObject.CompareTag("Rock"))
        {
            speed = Random.Range(5.0f, 7.0f);

            FixBackSpeedToMax(Random.Range(0.5f, 1.0f));
        }
        #endregion /*** ^^^ Player is slower on small rock. ^^^ ***/

        #region /*** Player is faster on Bison grass floor. ***/
        if (collider.gameObject.CompareTag("BisonGrassFloor"))
        {
            speed = Random.Range(10.0f, 12.5f);

            FixBackSpeedToMax(Random.Range(3.5f, 4.5f));
        }
        #endregion /*** ^^^ Player is faster on Bison grass floor. ^^^ ***/
    }


    private async void FixBackSpeedToMax(float duration)
    {
        await Task.Delay((int)duration * 1000);

        speed = maxSpeed;
    }
    #endregion /*** ^^^ Speed modifiers collisions. ^^^ ***/
}