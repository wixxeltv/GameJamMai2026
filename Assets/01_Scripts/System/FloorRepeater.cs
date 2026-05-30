using UnityEngine;

public class FloorRepeater : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject targetPoint;
    [SerializeField] private float speed = 1.0f;
    private GameObject[] _floorsPrefab;
    [SerializeField] private GameObject[] floor;
    [Range (0, 2)] private int _floorIndex;
    private Quaternion _rotation = new Quaternion(0f, 180f, 0f, 0f);

    void Update()
    {
        _floorsPrefab = GameObject.FindGameObjectsWithTag("Floor");
        
        float step = speed * Time.deltaTime;
        foreach (GameObject onetilefloor in _floorsPrefab)
        {
            onetilefloor.transform.position =
                Vector3.MoveTowards(onetilefloor.transform.position, targetPoint.transform.position, step);

            if (Vector3.Distance(onetilefloor.transform.position, targetPoint.transform.position) < 0.025f)
            {
                Debug.Log("It has reached its destination");
                int randomIndex = Random.Range(0, 1);
                switch (randomIndex)
                {
                    case 0:
                        Instantiate(floor[_floorIndex], spawnPoint.transform.position, floor[_floorIndex].transform.rotation, transform);
                        break;
                    case 1:
                        Instantiate(floor[_floorIndex], spawnPoint.transform.position, floor[_floorIndex].transform.rotation, transform);
                        break;
                }
                _floorIndex = (_floorIndex + 1) % 3;
                
                Destroy(onetilefloor);
            }
        }
    }
}