using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialUI : MonoBehaviour
{
    [Header("Tutorial UI")]
    [SerializeField] private GameObject[] tutorialUI;
    [SerializeField] private GameObject enemyArrow;
    [SerializeField] private GameObject colorArrow;
    
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private AudioClip panelSFX;
    
    private int displayCount = 0;
    private bool inTutorial = false;
    private EnemyManager _enemyManager;

    private void Start()
    {
        _enemyManager = FindFirstObjectByType<EnemyManager>();
    }

    public void StartTutorial()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.tutorialBGM, 100f);
        tutorialUI[0].SetActive(true);
        inTutorial = true;
    }

    public void OnPanel(InputAction.CallbackContext context)
    {
        if (context.started && inTutorial)
        {
            AudioManager.Instance.PlaySfx(panelSFX, 100f);
            tutorialUI[displayCount].SetActive(false);
            displayCount++;
            if (displayCount >= tutorialUI.Length)
            {
                inTutorial = false;
            }
            else
            {
                tutorialUI[displayCount].SetActive(true);
                switch (displayCount)
                {
                    case 2:
                        colorArrow.SetActive(true);
                        break;
                    case 5:
                        colorArrow.SetActive(false);
                        enemyArrow.SetActive(true);
                        break;
                    case 6:
                        enemyArrow.SetActive(false);
                        foreach (Enemy enemy in enemies)
                        {
                            Instantiate(enemy.gameObject, _enemyManager.GetSpawnLineCenter().position, Quaternion.identity);
                        }
                        break;
                }
            }
        }
    }
}