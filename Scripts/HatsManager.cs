using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HatsManager : MonoBehaviour
{
    public static HatsManager Instance;

    private void Awake()
    {
        #region /*** Initialize hats manager instance (always alive). ***/
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(target: gameObject);
        }
        else
        {
            Destroy(obj: gameObject);
        }
        #endregion /*** ^^^ Initialize hats manager instance (always alive). ^^^ ***/
    }



    #region /*** Generate and return new hat with random sprite. ***/
    [Header("Hats sprites: ")]
    [SerializeField] private Sprite[] hatsArray;
    [Space]
    [SerializeField] private GameObject hatReference;

    private GameObject GenerateRandomHat()
    {
        GameObject hatTemp = Instantiate(
            original: hatReference,
            position: Vector3.zero,
            rotation: Quaternion.identity);

        int randHat = UnityEngine.Random.Range(0, hatsArray.Length);

        hatTemp.GetComponent<SpriteRenderer>().sprite = hatsArray[randHat];

        return hatTemp;
    }

    #endregion /*** ^^^ Generate and return new hat with random sprite. ^^^ ***/



    /*** Initialize hats for a parent passed in parameters,
     * with the number of hats to initialize,
     * and the y offset for the first hat.
     * Each hat is initialized with a delay if its asked in parameters,
     * with the duration in ms (100 ms is the default value).
     * ***/
    public async Task<Tuple<GameObject[], int>> InitializeHatsForEntity(
        int maxHats,
        float yOffsetAtFirstHat,
        GameObject parentObject,
        bool needDelay = false,
        int delayDuration = 100)
    {
        /*** Initialize hats data to send back to object. ***/
        GameObject[] hatsArray = new GameObject[maxHats];
        int currentHatNum = 0;

        /*** ^^^ Initialize hats data to send back to object. ^^^ ***/

        float yOffset = yOffsetAtFirstHat;

        Vector3 hatReferenceScale = hatReference.transform.localScale;
        Vector3 parentScale = parentObject.transform.localScale;


        for (int i = 0; i < maxHats; i++)
        {
            if (parentObject == null)
                break;

            GameObject hat = GenerateRandomHat();

            #region /*** Setting hat position with parent object position. ***/
            hat.transform.localPosition = new Vector3(
                parentObject.transform.position.x - 0.03f,
                parentObject.transform.position.y + yOffset,
                parentObject.transform.position.z);

            #endregion /*** ^^^ Setting hat position with parent object position. ^^^ ***/


            /*** Setting hat's parent to parent's transform. ***/
            hat.transform.SetParent(parentObject.transform);

            /*** ^^^ Setting hat's parent to parent's transform. ^^^ ***/

            #region /*** Correcting size errors. ***/
            hat.transform.localScale = new Vector3(
                hatReferenceScale.x / parentScale.x,
                hatReferenceScale.y / parentScale.y,
                hatReferenceScale.z / parentScale.z);

            #endregion /*** ^^^ Correcting size errors. ^^^ ***/

            /*** Setting hat's random rotation. ***/
            Quaternion rotation = hat.transform.localRotation;
            rotation.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-10.0f, 10.0f));
            hat.transform.localRotation = rotation;
            /*** ^^^ Setting hat's random rotation. ^^^ ***/


            /*** Update data to send back. ***/
            hatsArray[i] = hat;

            currentHatNum++;
            /*** ^^^ Update data to send back. ^^^ ***/


            // Increase offset.
            yOffset += 0.25f;

            #region /*** Delay if specified in parameters. ***/
            if (needDelay)
            {
                await Task.Delay(delayDuration);
            }
            #endregion /*** ^^^ Delay if specified in parameters. ^^^ ***/
        }


        Tuple<GameObject[], int> dataToSendBack = new(hatsArray, currentHatNum);

        return dataToSendBack;
    }

    /*** ^^^ Initialize hats for a parent passed in parameters,
    * with the number of hats to initialize,
    * and the y offset for the first hat.
    *  ^^^ ***/




    #region /*** Add Hat to drop to the floor. ***/
    private Dictionary<GameObject, Vector3> hatsToDrop = new();

    public void AddHatToDropOnFloor(
        GameObject hatToDrop,
        GameObject parentObjectWearingTheHat)
    {
        Vector3 parentPos = parentObjectWearingTheHat.transform.position;

        float xOffset = UnityEngine.Random.Range(-2.0f, 2.0f);

        Vector3 customTargetPos =
                    new(
                    x: parentPos.x + (xOffset * 1.5f),
                    y: parentPos.y - (parentObjectWearingTheHat.GetComponent<BoxCollider2D>().size.y + UnityEngine.Random.Range(0.5f, 1.0f)),
                    z: parentPos.z
                    );

        hatToDrop.transform.SetParent(p: null);

        // Reset hat size.
        hatToDrop.transform.localScale = Vector3.one * 5;

        hatsToDrop.Add(
            key: hatToDrop,
            value: customTargetPos);
    }

    #endregion /*** ^^^ Add Hat to drop to the floor. ^^^ ***/



    #region /*** Reccursively slowly decrease hat color, then destroy it, when its transparent. ***/
    private async void DecreaseHatColor(
        GameObject hatToColorDecrease,
        SpriteRenderer hatSpr)
    {
        if (hatToColorDecrease != null)
        {
            hatSpr.color = new Color(
                r: (float)(hatSpr.color.r - 0.01),
                g: (float)(hatSpr.color.g - 0.01),
                b: (float)(hatSpr.color.b - 0.01),
                a: (float)(hatSpr.color.a - 0.01));

            if (hatSpr.color.a > 0.05f)
            {
                await Task.Delay(50);

                DecreaseHatColor(
                    hatToColorDecrease: hatToColorDecrease,
                    hatSpr: hatSpr);
            }
            else
            {
                hatsToDrop.Remove(hatToColorDecrease);
                hatsToDrop.TrimExcess();
                Destroy(obj: hatToColorDecrease);
            }
        }
    }
    #endregion /*** ^^^ Reccursively slowly decrease hat color, then destroy it, when its transparent. ^^^ ***/



    private void Update()
    {
        #region /*** Loop logic to drop hats to the floor. ***/
        if (hatsToDrop.Count > 0)
        {
            foreach (GameObject hat in hatsToDrop.Keys)
            {
                if (hat == null)
                {
                    GameObject hatGarbage = hat;

                    hatsToDrop.Remove(hat);
                    hatsToDrop.TrimExcess();
                    Destroy(obj: hatGarbage);
                    break;
                }

                // Drop hat.
                if (hat.transform.position.y > hatsToDrop[hat].y + 0.1f)
                {
                    hat.transform.position = Vector3.Slerp(
                        a: hat.transform.position,
                        b: hatsToDrop[hat],
                        t: 0.1f);
                }
                else
                {
                    // Grab spriteRender.
                    SpriteRenderer hatSpriteRenderer = hat.GetComponent<SpriteRenderer>();

                    // When hat is dropped, ask once to slowly decrease its color.
                    if (hatSpriteRenderer.color.a >= 1)
                    {
                        DecreaseHatColor(
                            hatToColorDecrease: hat,
                            hatSpr: hatSpriteRenderer);
                    }
                }
            }
        }
        #endregion /*** ^^^ Loop logic to drop hats to the floor. ^^^ ***/
    }
}
