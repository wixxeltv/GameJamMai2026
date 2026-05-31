using UnityEngine;

public class ItemHoverAnimation : MonoBehaviour
{
    private Vector3 upPosition;
    private Vector3 downPosition;

    private bool IsUp=false;

    private void Start()
    {
        Vector3 startPosition = transform.position;

        upPosition = new Vector3(startPosition.x, startPosition.y+0.5f, startPosition.z);
        downPosition = new Vector3(startPosition.x, startPosition.y-0.5f, startPosition.z);
    }

    private void Update()
    {
        switch(IsUp){
            case true:
                transform.position = Vector3.Lerp(transform.position, upPosition, Time.deltaTime);
                break;
            case false:
                transform.position = Vector3.Lerp(transform.position, downPosition, Time.deltaTime);
                break;
        }

        if (transform.position.y >= upPosition.y-0.25f || transform.position.y <= downPosition.y+0.25f)
        {
            IsUp = !IsUp;
        }

    }
}