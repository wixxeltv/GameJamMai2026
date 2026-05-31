using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class PrebossFightUI : MonoBehaviour
{
    [SerializeField] private GameObject[] dialogueUI;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private AudioClip panelSFX;
    
    [SerializeField] private Image brogleImage;
    [SerializeField] private Image kidImage;
    
    [SerializeField] private Color noTalkingColor;
    
    [SerializeField] private float swayAmount = 5f;
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float swaySmoothness = 5f;
    
    public UnityEvent OnDialogueComplete;
    
    private int displayCount = 0;
    private bool inDialogue = false;
    private PlayerInput _playerInput;

    
    private RectTransform _brogleRect;
    private RectTransform _kidRect;
    private Vector2 _brogleStartPos;
    private Vector2 _kidStartPos;
    private bool _brogleTalking = false;
    private bool _kidTalking = false;
    
    private void Start()
    {
        _playerInput = FindFirstObjectByType<PlayerInput>();
        
        if (brogleImage != null)
        {
            _brogleRect = brogleImage.rectTransform;
            _brogleStartPos = _brogleRect.anchoredPosition;
        }
        
        if (kidImage != null)
        {
            _kidRect = kidImage.rectTransform;
            _kidStartPos = _kidRect.anchoredPosition;
        }
    }
    
    private void Update()
    {
        if (!inDialogue) return;
        
        // Sway the currently talking character
        if (_brogleTalking && _brogleRect != null)
        {
            float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            Vector2 targetPos = _brogleStartPos + new Vector2(sway, 0f);
            _brogleRect.anchoredPosition = Vector2.Lerp(_brogleRect.anchoredPosition, targetPos, Time.deltaTime * swaySmoothness);
        }
        else if (_brogleRect != null)
        {
            // Return to starting position
            _brogleRect.anchoredPosition = Vector2.Lerp(_brogleRect.anchoredPosition, _brogleStartPos, Time.deltaTime * swaySmoothness);
        }
        
        if (_kidTalking && _kidRect != null)
        {
            float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            Vector2 targetPos = _kidStartPos + new Vector2(sway, 0f);
            _kidRect.anchoredPosition = Vector2.Lerp(_kidRect.anchoredPosition, targetPos, Time.deltaTime * swaySmoothness);
        }
        else if (_kidRect != null)
        {
            // Return to starting position
            _kidRect.anchoredPosition = Vector2.Lerp(_kidRect.anchoredPosition, _kidStartPos, Time.deltaTime * swaySmoothness);
        }
    }
    
    public void StartDialogue()
    {
        AudioManager.Instance.ChangeBGM(AudioManager.Instance.dialogueBGM);
        dialogueUI[0].SetActive(true);
        inDialogue = true;
        displayCount = 0;
        brogleImage.gameObject.SetActive(true);
        kidImage.gameObject.SetActive(true);
        backgroundPanel.SetActive(true);
        SetKidTalking();
        
        // // Disable player input during dialogue
        // if (_playerInput != null)
        //     _playerInput.DeactivateInput();
    }

    public void OnPanel(InputAction.CallbackContext context)
    {
        if (context.started && inDialogue)
        {
            AudioManager.Instance.PlaySfx(panelSFX, 100f);
            dialogueUI[displayCount].SetActive(false);
            displayCount++;
            
            if (displayCount >= dialogueUI.Length)
            {
                inDialogue = false;
                
                if (_playerInput != null)
                    _playerInput.ActivateInput();
                backgroundPanel.SetActive(false);
                brogleImage.gameObject.SetActive(false);
                kidImage.gameObject.SetActive(false);
                OnDialogueComplete?.Invoke();
            }
            else
            {
                dialogueUI[displayCount].SetActive(true);
                switch (displayCount)
                {
                    case 0:
                        SetKidTalking();
                        break;
                    case 2:
                        SetBrogleTalking();
                        break;
                    case 6:
                        SetKidTalking();
                        break;
                }
            }
        }
    }
    
    private void SetBrogleTalking()
    {
        _brogleTalking = true;
        _kidTalking = false;
        
        brogleImage.color= Color.white;
        kidImage.color= noTalkingColor;
    }
    
    private void SetKidTalking()
    {
        _brogleTalking = false;
        _kidTalking = true;
        
        brogleImage.color= noTalkingColor;
        kidImage.color= Color.white;
    }
}