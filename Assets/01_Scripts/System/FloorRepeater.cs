using UnityEngine;

public class FloorRepeater : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject targetPoint;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private GameObject[] floorsPrefab;
    [SerializeField] private GameObject floor;

    void Start()
    {
        Instantiate(floor, spawnPoint.transform.position, Quaternion.identity, transform);
    }
    void Update()
    {
        floorsPrefab = GameObject.FindGameObjectsWithTag("Floor");
        
        float step = speed * Time.deltaTime;
        foreach (GameObject floor in floorsPrefab)
        {
            floor.transform.position =
                Vector3.MoveTowards(floor.transform.position, targetPoint.transform.position, step);
            if (Vector3.Distance(floor.transform.position, targetPoint.transform.position) < 0.025f)
            {
                Debug.Log("It has reached its destination");
                floor.transform.position = spawnPoint.transform.position;
                //Instantiate(floor, spawnPoint.transform.position, Quaternion.identity, transform);
            }
        }
    }
}