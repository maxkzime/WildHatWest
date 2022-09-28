using System.Threading.Tasks;
using UnityEngine;

public class GoldNuggetBehavior : MonoBehaviour
{
    private GameObject player;
    private ObjectsGeneration generatorGenerationScript;

    private float floorLimit;

    #region /*** Expiration parameters. ***/
    private float goldNuggetSpawnTime;
    private float expirationOffset;
    #endregion /*** ^^^ Expiration parameters. ^^^ ***/

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        generatorGenerationScript = ObjectsGeneration.Instance;

        floorLimit = (generatorGenerationScript.GetFloorSize() / 2) - 1;


        #region /*** Set expiration parameters. ***/
        goldNuggetSpawnTime = Time.time;
        expirationOffset = Random.Range(10.0f, 20.0f);

        #endregion /*** ^^^ Set expiration parameters. ^^^ ***/
    }



    #region /*** Explosion parameters mechanics. ***/
    private bool isPartOfExplosion = false;

    public bool GetIsPartOfExplosion()
    {
        return isPartOfExplosion;
    }

    private float speedInExplosion = 1.0f;

    private Vector3 targetPos;

    public void SetIsPartOfExplosion(bool value)
    {
        // Explosion initialization if the property was set to false.
        if (!GetIsPartOfExplosion())
        {
            speedInExplosion = Random.Range(1.0f, 5.0f);

            targetPos = new Vector3(
                x: transform.localPosition.x + ((Random.value >= 0.5f ? 1.0f : -1.0f) * Random.Range(0.0f, 4.0f)),
                y: transform.localPosition.y + ((Random.value >= 0.5f ? 1.0f : -1.0f) * Random.Range(0.0f, 4.0f)),
                z: transform.localPosition.z);
        }

        isPartOfExplosion = value;
    }
    #endregion /*** ^^^ Explosion parameters mechanics. ^^^ ***/


    private float distanceToAttract = 4;
    private float accuracySafety = 0.5f;

    private void FixedUpdate()
    {
        if (GetIsPartOfExplosion())
        {
            GoldNuggetExplosion();
        }
        else if (generatorGenerationScript.HasUnlockedMagnet)
        {
            // Move toward player.
            Vector2 direction = player.transform.localPosition - transform.position;

            if (direction.magnitude < distanceToAttract + generatorGenerationScript.MagnetLevel &&
                direction.magnitude > accuracySafety)
            {
                transform.Translate(
                    translation: generatorGenerationScript.MagnetLevel * Time.deltaTime * direction.normalized,
                    relativeTo: Space.World);
            }
        }
    }



    #region /*** GoldNugget part of explosion logic. ***/
    private Vector3 targetDirection;
    private float accuracy = 0.5f;

    private void GoldNuggetExplosion()
    {
        targetDirection = targetPos - transform.position;

        if (targetDirection.magnitude > accuracy)
        {
            transform.position = Vector3.Slerp(transform.position, targetPos, speedInExplosion / 100);

            if (targetDirection.magnitude <= accuracy + 0.5f)
            {
                SetIsPartOfExplosion(false);
            }
        }
    }
    #endregion /*** ^^^ GoldNugget part of explosion logic. ^^^ ***/


    private Vector3 currentPos;

    private void Update()
    {
        #region /*** Grab Sprite renderers for gold nugget expiration. ***/
        if (goldNuggetSpr == null)
        {
            goldNuggetSpr = GetComponent<SpriteRenderer>();
        }

        if (goldNuggetShadowSpr == null)
        {
            goldNuggetShadowSpr = transform.Find("shadow").GetComponent<SpriteRenderer>();
        }
        #endregion /*** ^^^ Grab Sprite renderer for gold nugget expiration. ^^^ ***/



        #region /*** Expiration logic, ask to decrease color and the destroy the gold nugget. ***/
        if (goldNuggetSpawnTime + expirationOffset < Time.time &&
            goldNuggetSpr.color.a >= 1)
        {
            DecreaseGoldNuggetColor();
        }

        #endregion /*** ^^^ Expiration logic, ask to decrease color and the destroy the gold nugget. ^^^ ***/



        #region /*** Destroy gold nugget if out of bounds. ***/
        currentPos = transform.localPosition;

        if (currentPos.y > floorLimit
            || currentPos.y < -floorLimit
            || currentPos.x > floorLimit
            || currentPos.x < -floorLimit)
        {
            // Destroy gold nugget and its shadow, when the gold nugget is out of bounds.
            Destroy(obj: gameObject);
        }
        #endregion /*** ^^^ Destroy gold nugget if out of bounds. ^^^ ***/
    }



    #region /*** Reccursively slowly decrease and darken color, then destroy it, when its transparent. ***/
    private SpriteRenderer goldNuggetSpr = null;
    private SpriteRenderer goldNuggetShadowSpr = null;

    private async void DecreaseGoldNuggetColor()
    {
        if (goldNuggetSpr != null)
        {
            // Decrease and darken gold nugget's color.
            goldNuggetSpr.color = new Color(
                    r: goldNuggetSpr.color.r - 0.01f,
                    g: goldNuggetSpr.color.g - 0.01f,
                    b: goldNuggetSpr.color.b - 0.01f,
                    a: goldNuggetSpr.color.a - 0.01f
                    );

            // Decrease and darken gold nugget's shadow color.
            goldNuggetShadowSpr.color = new Color(
                    r: goldNuggetShadowSpr.color.r - 0.01f,
                    g: goldNuggetShadowSpr.color.g - 0.01f,
                    b: goldNuggetShadowSpr.color.b - 0.01f,
                    a: goldNuggetShadowSpr.color.a - 0.01f
                    );

            // Wait delay, then recursive call.
            if (goldNuggetSpr.color.a > 0.05f)
            {
                await Task.Delay(50);

                DecreaseGoldNuggetColor();
            }
            else
            {
                // Destroy gold nugget and its shadow, when the gold nugget is transparent.
                Destroy(obj: gameObject);
            }
        }
    }
    #endregion /*** ^^^ Reccursively slowly decrease and darken color, then destroy it, when its transparent. ^^^ ***/
}
