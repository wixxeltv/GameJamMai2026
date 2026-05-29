using UnityEngine;

public class FloorRepeater : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject targetPoint;
    [SerializeField] private float speed = 1.0f;
    private GameObject[] _floorsPrefab;
    [SerializeField] private GameObject[] floor;
    [Range (0, 2)] private int _floorIndex; 

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
                Instantiate(floor[_floorIndex], spawnPoint.transform.position, floor[_floorIndex].transform.rotation, transform);
                
                if(_floorIndex == 2) _floorIndex = 0;
                else{_floorIndex++;}
                
                Destroy(onetilefloor);
            }
        }
    }
}