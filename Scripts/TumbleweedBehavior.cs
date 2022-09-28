using UnityEngine;

public class TumbleweedBehavior : MonoBehaviour
{
    private float speed = 1.0f;
    private Vector2 spawnPos;
    private int mapSize = 1;

    // Start is called before the first frame update
    private void Start()
    {
        // Randomize speed.
        speed = Random.Range(1.0f, 5.0f);

        // Use spawn position to find direction.
        spawnPos = transform.position;

        mapSize = ObjectsGeneration.Instance.GetFloorSize() + 2;

    }

    private Vector3 currentPos;

    // Update is called once per frame
    void Update()
    {
        // Cross the map.
        transform.Translate(translation: speed
                            * Time.deltaTime
                            * Mathf.Sign(spawnPos.x)
                            * Vector2.left,
                            relativeTo: Space.World);

        // Rotation.
        transform.Rotate(
            xAngle: 0,
            yAngle: 0,
            zAngle: speed * Random.Range(2.0f, 360.0f) * Time.deltaTime);


        // Out of bounds security.
        currentPos = transform.localPosition;

        if (currentPos.y > mapSize / 2 ||
            currentPos.y < -mapSize / 2 ||
            currentPos.x > mapSize / 2 ||
            currentPos.x < -mapSize / 2)
        {
            Destroy(obj: gameObject);
        }
    }
}
