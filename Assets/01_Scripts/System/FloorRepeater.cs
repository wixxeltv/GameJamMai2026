using UnityEngine;

public class FloorRepeater : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private GameObject[] floorPrefabs;
    [Range(0, 4)] private int _floorIndex;
    
    private GameObject[] _floors;
    
    void FixedUpdate()
    {
        _floors = GameObject.FindGameObjectsWithTag("Floor");

        foreach (GameObject floorTile in _floors)
        {
            if (floorTile == null) continue;

            float step = speed * Time.fixedDeltaTime;
            Vector3 newPos = Vector3.MoveTowards(floorTile.transform.position, targetPoint.position, step);

            Rigidbody rb = floorTile.GetComponent<Rigidbody>();
            if (rb != null)
                rb.MovePosition(newPos);
            else
                floorTile.transform.position = newPos;

            if (Vector3.Distance(newPos, targetPoint.position) < 0.025f)
            {
                Instantiate(
                    floorPrefabs[_floorIndex],
                    spawnPoint.position,
                    floorPrefabs[_floorIndex].transform.rotation,
                    transform
                );
                _floorIndex = (_floorIndex + 1) % floorPrefabs.Length;
                Destroy(floorTile);
            }
        }
    }
}