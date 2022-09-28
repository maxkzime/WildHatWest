using UnityEngine;

public class IndianAI : MonoBehaviour
{
    private float lastShotTime = 0.0f;
    private float lastShotOffset = 1.0f;

    private float circleOffset;
    private float amplitude = 1.0f;

    private void Start()
    {
        lastShotTime = Time.time;

        // Initialize random parameters.
        circleOffset = Random.Range(0.0f, 360.0f);
        amplitude = Random.Range(1.0f, 3.0f);

        distanceToOpponentToShoot = ObjectsGeneration.Instance.IndianDistanceToOpponentToShoot;

        InitializeBow();
    }


    #region /*** Initialize bow. ***/
    [SerializeField] private GameObject bowReference;

    private GameObject bowInstance;

    private void InitializeBow()
    {
        bowInstance = Instantiate(
                   original: bowReference,
                   position: new Vector3(
                       x: transform.position.x + (GetComponent<BoxCollider2D>().size.x),
                       y: transform.position.y - (GetComponent<BoxCollider2D>().size.y / 2),
                       z: transform.position.z),
                   rotation: Quaternion.identity);

        bowInstance.transform.SetParent(p: transform);
    }
    #endregion /*** ^^^ Initialize bow. ^^^ ***/




    private readonly float distanceToPlayer = 5.0f;
    private readonly float speed = 5.0f;

    private bool isNearPlayer = false;
    private float lastTimeNearPlayer = 0.0f;
    private float randIntervalBetweenDirection;


    private void FixedUpdate()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            // Target direction.
            Vector3 playerDirection = 
                GameObject.FindGameObjectWithTag("Player").transform.localPosition - transform.position;

            Vector3 circleDirection = playerDirection -
                new Vector3(
                    x: amplitude * Mathf.Sin(Time.time + (circleOffset * Mathf.Deg2Rad)),
                    y: amplitude * Mathf.Cos(Time.time + (circleOffset * Mathf.Deg2Rad)),
                    z: 0);


            // Move in player direction if not near player.
            if (playerDirection.magnitude > distanceToPlayer && !isNearPlayer)
            {
                transform.Translate(
                    translation: speed * Time.deltaTime * playerDirection.normalized,
                    relativeTo: Space.World);

                lastTimeNearPlayer = Time.time;
                randIntervalBetweenDirection = Random.Range(2.0f, 3.5f);
            }
            else
            {
                isNearPlayer = true;

                transform.Translate(
                    translation: speed * Time.deltaTime * circleDirection.normalized,
                    relativeTo: Space.World);
            }


            // Return to player after a timer interval, to make sure he is always near player.
            if (lastTimeNearPlayer + randIntervalBetweenDirection < Time.time && isNearPlayer)
            {
                isNearPlayer = false;
            }
        }
    }




    private GameObject opponentTarget = null;
    private float distanceToOpponentToShoot = 5.0f;

    [SerializeField] private GameObject bullet;

    private void Update()
    {
        #region /*** Select random opponent as target. ***/
        if (opponentTarget == null && GameObject.FindGameObjectWithTag("Opponent") != null)
        {
            opponentTarget =
                GameObject.FindGameObjectsWithTag("Opponent")[Random.Range(0, GameObject.FindGameObjectsWithTag("Opponent").Length)];
        }
        #endregion /*** ^^^ Select random opponent as target. ^^^ ***/


        if (opponentTarget != null)
        {
            FlipToTarget();

            // Parameters to shoot bullet in target direction + rotate bow.
            Vector2 targetDirection =
                opponentTarget.transform.localPosition - transform.position;

            Quaternion rotation = 
                Quaternion.Euler(
                    x: 0,
                    y: 0,
                    z: Mathf.Atan2(targetDirection.normalized.y, targetDirection.normalized.x) * Mathf.Rad2Deg);


            #region /*** Shoot bullet in target direction + at bow position , when target is near indian. ***/
            if (lastShotTime + lastShotOffset < Time.time &&
                targetDirection.magnitude < distanceToOpponentToShoot)
            {
                _ = Instantiate(
                    original: bullet,
                    position: bowInstance.transform.position,
                    rotation: rotation);

                lastShotTime = Time.time;
                lastShotOffset = Random.Range(2.5f, 4.0f);
            }
            #endregion /*** ^^^ Shoot bullet in target direction + at bow position , when target is near indian. ^^^ ***/


            #region /***  Bow Rotate to target direction. ***/
            targetDirection.Normalize();
            bowInstance.transform.localRotation = targetDirection.x > 0.1f
                ? rotation
                : Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, -targetDirection.x) * Mathf.Rad2Deg);

            #endregion /*** ^^^  Bow Rotate to target direction.  ^^^ ***/
        }
    }




    #region /*** Flip indian accordingly with target position. ***/
    private bool loopRunningToRight = false;
    private bool loopRunningToLeft = false;

    private void FlipToTarget()
    {
        if (opponentTarget.transform.localPosition.x < transform.position.x &&
           transform.localScale.x > 0 &&
           !loopRunningToLeft &&
           !loopRunningToRight)
        {
            loopRunningToLeft = true;
        }
        else if (opponentTarget.transform.localPosition.x > transform.position.x &&
            transform.localScale.x < 0 &&
            !loopRunningToLeft &&
            !loopRunningToRight)
        {
            loopRunningToRight = true;
        }

        if (loopRunningToLeft)
        {
            transform.localScale =
                new Vector3(transform.localScale.x - 1.0f, transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x <= -5)
            {
                loopRunningToLeft = false;
            }
        }

        if (loopRunningToRight)
        {
            transform.localScale =
                new Vector3(transform.localScale.x + 1.0f, transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x >= 5)
            {
                loopRunningToRight = false;
            }
        }
    }
    #endregion /*** ^^^ Flip indian accordingly with target position. ^^^ ***/




    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Opponent_bullet"))
        {
            // Call to destroy bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Trigger flash effect when taking damage.
            GetComponent<FlashEffect>().Flash();


            // Grab rigidbody to apply force on it.
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();

                // Reset velocity of the rigidbody (no more force).
                rb.velocity = Vector3.zero;
            }


            // Anim recoil push away from the player.
            if (rb != null && rb.velocity == Vector2.zero)
            {
                ShotgunKnockback(collider.gameObject);
            }
        }
    }




    // Security to avoid object destruction while async functions are running.
    public bool isReadyToBeDestroySecurity = true;


    #region /*** Shotgun Knockback on player when he shoots. ***/
    private Rigidbody2D rb = null;

    private async void ShotgunKnockback(GameObject hitObject)
    {
        if (rb != null)
        {
            // Apply knockback force on indian.
            rb.AddForce(
                force: Random.Range(25, 50) * -(hitObject.transform.localPosition - transform.localPosition),
                mode: ForceMode2D.Impulse);

            isReadyToBeDestroySecurity = false;


            // Wait random delay before stopping knockback.
            await System.Threading.Tasks.Task.Delay(Random.Range(200, 400));

            // Reset velocity of the rigidbody (no more force).
            rb.velocity = Vector3.zero;


            isReadyToBeDestroySecurity = true;
        }

    }
    #endregion /*** ^^^ Shotgun Knockback on player when he shoots. ^^^ ***/
}
