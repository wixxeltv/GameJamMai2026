using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerColorController _playerColorController;
    [SerializeField] private Image colorImage;

    void Start()
    {
        _playerColorController = FindFirstObjectByType<PlayerColorController>();
    }
    private void Update()
    {
        switch (_playerColorController.CurrentColor)
        {
            case ColorType.Blue:
                colorImage.color = Color.blue;
                break;
            case ColorType.Red:
                colorImage.color = Color.red;
                break;
            case ColorType.Yellow:
                colorImage.color = Color.yellow;
                break;
            default:
                colorImage.color = Color.white;
                break;
        }
    }
}
