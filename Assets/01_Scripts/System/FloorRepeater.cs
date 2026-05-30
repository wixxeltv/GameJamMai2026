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
            
            // Move the floor tile
            float step = speed * Time.fixedDeltaTime;
            floorTile.transform.position = Vector3.MoveTowards(
                floorTile.transform.position, 
                targetPoint.position, 
                step
            );
            
            // Move all child objects with colliders (walls, obstacles, etc.)
            MoveChildColliders(floorTile);
            
            // Check if reached destination
            if (Vector3.Distance(floorTile.transform.position, targetPoint.position) < 0.025f)
            {
                Debug.Log("Floor reached destination");
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
    
    void MoveChildColliders(GameObject floorTile)
    {
        // Get all colliders in children that need to move with the floor
        Rigidbody[] childRigidbodies = floorTile.GetComponentsInChildren<Rigidbody>();
        
        foreach (Rigidbody rb in childRigidbodies)
        {
            if (rb.gameObject == floorTile) continue; // Skip the parent
            
            // Move wall/obstacle colliders to match parent's position
            // These should be kinematic so they can push the player
            rb.MovePosition(rb.transform.position);
        }
    }
}