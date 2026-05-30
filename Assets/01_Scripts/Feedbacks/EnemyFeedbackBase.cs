using UnityEngine;

public abstract class EnemyFeedbackBase : MonoBehaviour
{
    public abstract void Blink(bool wrongColor = false);
    public abstract void DeathEffect();
    public virtual void Knockback(Vector3 from, float forceMultiplier = 1f) { }
}
