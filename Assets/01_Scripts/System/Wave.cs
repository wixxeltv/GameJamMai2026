using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Wave")]
public class Wave : ScriptableObject
{
    public int enemeisToKill;
    public float spawnInterval;
    public Enemy[] enemies;
    public bool isBoss;
}
