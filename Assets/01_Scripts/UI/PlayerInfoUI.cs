using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private PlayerColorController _playerColorController;
    [SerializeField] private Image[] slots;
    private int _previousSelectedSlot;
    private Vector3 _increasedScale = new Vector3(1.1f, 1.1f, 1.1f);

    void Start()
    {
        _playerColorController = FindFirstObjectByType<PlayerColorController>();
    }
    private void Update()
    {
        switch (_playerColorController.CurrentColor)
        {
            case ColorType.Red:
                slots[0].color = Color.gray;
                slots[0].transform.localScale = _increasedScale;
                _previousSelectedSlot = 1;
                break;
            case ColorType.Blue:
                slots[1].color = Color.gray;
                slots[1].transform.localScale = _increasedScale;
                _previousSelectedSlot = 0;
                break;
            case ColorType.Yellow:
                slots[2].color = Color.gray;
                slots[2].transform.localScale = _increasedScale;
                _previousSelectedSlot = 1;
                break;
        }
        slots[_previousSelectedSlot].color = Color.white;
        slots[_previousSelectedSlot].transform.localScale = Vector3.one;
        
    }
}
