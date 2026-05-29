using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Wave")]
public class Wave : ScriptableObject
{
    public float spawnInterval;
    public Enemy[] enemies;
}
