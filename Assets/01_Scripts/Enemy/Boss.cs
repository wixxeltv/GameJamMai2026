using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    [System.Serializable]
    public class BossForm
    {
        public GameObject model;
        public ColorType color;
        public Color visualColor;
    }

    [Header("AudioClips")] [SerializeField]
    private AudioClip phase2SFX;

    [SerializeField] private AudioClip phase3SFX;

    [Header("Formes (modèles + couleurs)")]
    [SerializeField] private Material[] _materials;
    [SerializeField] private MeshRenderer _meshRenderer;

    private ProgressBar _progressBar;

    [Header("Bullets")] [SerializeField] private GameObject _bulletPrefabRed;
    [SerializeField] private GameObject _bulletPrefabYellow;
    [SerializeField] private GameObject _bulletPrefabBlue;
    [SerializeField] private Transform _firePoint;

    [Header("Mouvement en 8")] [SerializeField]
    private float _figureEightWidth = 8f;

    [SerializeField] private float _figureEightHeight = 4f;
    [SerializeField] private float _figureEightSpeedPhase1 = 0.6f;
    [SerializeField] private float _figureEightSpeedPhase2 = 1.0f;
    [SerializeField] private float _figureEightSpeedPhase3 = 1.8f;
    [SerializeField] private Vector3 _centerOffset = Vector3.zero;

    [Header("Tir - Rouge (Spiral)")] [SerializeField]
    private int _spiralBulletsPerVolley = 5;

    [SerializeField] private float _spiralFireInterval = 1.5f;

    [Header("Tir - Jaune (Shooter)")] [SerializeField]
    private float _shooterFireInterval = 1.2f;

    [Header("Tir - Bleu (Burst)")] [SerializeField]
    private int _burstBulletsCount = 5;

    [SerializeField] private float _burstSpreadAngle = 20f;
    [SerializeField] private float _burstFireInterval = 2f;
    [SerializeField] private float _burstTimeBetweenBullets = 0.08f;

    [Header("Phases")] [SerializeField] private float _colorSwitchInterval = 6f;
    [SerializeField] private int _rageBulletsCount = 12;
    [SerializeField] private float _phase3IntervalMultiplier = 0.55f;
    [SerializeField] private int _phase3SwitchEveryNShots = 4;
    [Range(0f, 1f)] [SerializeField] private float _phase2HpThreshold = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float _phase3HpThreshold = 0.2f;

    [Header("Transition de phase")] [SerializeField]
    private float _transitionDuration = 1.5f;

    [SerializeField] private float _transitionVibrateAmount = 0.3f;
    [SerializeField] private float _transitionShakeMagnitude = 0.25f;
    [SerializeField] private float _chaosShakeMagnitude = 0.55f;
    [SerializeField] private float _chaosFireInterval = 0.35f;

    private int _currentPhase = 0;
    private int _currentFormIndex = 0;
    private float _figureEightT;
    private Vector3 _spawnCenter;
    private float _volleyAngle;
    private bool _isTransitioning;
    private int _currentMaterialIndex = 0;

    protected override void Start()
    {
        _progressBar = GameObject.FindGameObjectsWithTag("BossHealthBar")[0].GetComponent<ProgressBar>();
        _progressBar.maximum = MaxHp;
        _progressBar.current = MaxHp;
        base.Start();
        _spawnCenter = transform.position;
        InitRandomForm();
        StartCoroutine(PhaseRoutine());
    }

    private void Update()
    {
        _progressBar.current = CurrentHp;
        if (!IsAlive) return;
        MoveFigureEight();
    }

    private void MoveFigureEight()
    {
        if (_isTransitioning) return;
        float speed = _currentPhase switch
        {
            2 => _figureEightSpeedPhase2,
            3 => _figureEightSpeedPhase3,
            _ => _figureEightSpeedPhase1
        };
        _figureEightT += speed * Time.deltaTime;
        float x = _figureEightWidth * Mathf.Sin(_figureEightT);
        float z = _figureEightHeight * Mathf.Sin(_figureEightT * 2f);
        transform.position = _spawnCenter + _centerOffset + new Vector3(x, 0f, z);
    }

    // --- Phases ---

    private IEnumerator PhaseRoutine()
    {
        _currentPhase = 1;
        yield return StartCoroutine(Phase1());

        AudioManager.Instance.PlaySfx(phase2SFX, 100f);
        yield return StartCoroutine(PhaseTransition(chaos: false));
        _currentPhase = 2;
        yield return StartCoroutine(Phase2());

        AudioManager.Instance.PlaySfx(phase3SFX, 100f);
        yield return StartCoroutine(PhaseTransition(chaos: true));
        _currentPhase = 3;
        yield return StartCoroutine(Phase3());
    }

    private IEnumerator PhaseTransition(bool chaos = false)
    {
        _isTransitioning = true;
        Vector3 frozenPos = transform.position;
        float elapsed = 0f;
        float nextChaosShot = 0f;
        int colorCycleIndex = 0;
        ColorType[] chaosColors = { ColorType.Red, ColorType.Yellow, ColorType.Blue };

        CameraShake.Instance?.Shake(_transitionDuration, chaos ? _chaosShakeMagnitude : _transitionShakeMagnitude);


        while (elapsed < _transitionDuration)
        {
            if (!IsAlive)
            {
                _isTransitioning = false;
                yield break;
            }

            elapsed += Time.deltaTime;
            Vector3 shake = Random.insideUnitSphere * _transitionVibrateAmount;
            shake.y = 0f;
            transform.position = frozenPos + shake;

            if (chaos && elapsed >= nextChaosShot)
            {
                SetBossColor(chaosColors[colorCycleIndex % chaosColors.Length]);
                colorCycleIndex++;
                StartCoroutine(RageBurst());
                nextChaosShot = elapsed + _chaosFireInterval;
            }

            yield return null;
        }

        transform.position = frozenPos;
        _isTransitioning = false;
    }

    private IEnumerator Phase1()
    {
        while (IsAlive && GetHpPercent() > _phase2HpThreshold)
            yield return StartCoroutine(ShootByColor(1f));
    }

    private IEnumerator Phase2()
    {
        StartCoroutine(ColorSwitchRoutine());
        while (IsAlive && GetHpPercent() > _phase3HpThreshold)
            yield return StartCoroutine(ShootByColor(1f));
    }

    private IEnumerator Phase3()
    {
        int shotCount = 0;
        while (IsAlive)
        {
            if (shotCount % _phase3SwitchEveryNShots == 0)
                SwitchToRandomForm();
            shotCount++;
            yield return StartCoroutine(ShootByColor(_phase3IntervalMultiplier));
        }
    }

    private IEnumerator ColorSwitchRoutine()
    {
        while (IsAlive && _currentPhase == 2)
        {
            yield return new WaitForSeconds(_colorSwitchInterval);
            SwitchToRandomForm();
            yield return StartCoroutine(RageBurst());
        }
    }

    // --- Tir ---

    private IEnumerator ShootByColor(float intervalMultiplier)
    {
        switch (EnemyColor)
        {
            case ColorType.Red:
                ShootSpiral();
                yield return new WaitForSeconds(_spiralFireInterval * intervalMultiplier);
                break;
            case ColorType.Yellow:
                ShootStraight(intervalMultiplier < 1f ? 3 : 1);
                yield return new WaitForSeconds(_shooterFireInterval * intervalMultiplier);
                break;
            default:
                yield return StartCoroutine(ShootBurst());
                yield return new WaitForSeconds(_burstFireInterval * intervalMultiplier);
                break;
        }
    }

    private void ShootSpiral()
    {
        if (_bulletPrefabRed == null || _firePoint == null) return;
        float angleStep = 360f / _spiralBulletsPerVolley;
        for (int i = 0; i < _spiralBulletsPerVolley; i++)
        {
            float angle = _volleyAngle + angleStep * i;
            Instantiate(_bulletPrefabRed, _firePoint.position, Quaternion.Euler(0f, angle, 0f), transform);
        }

        _volleyAngle += 15f;
    }

    private void ShootStraight(int count = 1)
    {
        if (_bulletPrefabYellow == null || _firePoint == null || _player == null) return;
        Vector3 dir = (_player.position - _firePoint.position).normalized;
        if (count == 1)
        {
            Instantiate(_bulletPrefabYellow, _firePoint.position, Quaternion.LookRotation(dir), transform);
        }
        else
        {
            float spread = 15f;
            float half = spread * (count - 1) / 2f;
            for (int i = 0; i < count; i++)
            {
                float angle = -half + spread * i;
                Instantiate(_bulletPrefabYellow, _firePoint.position,
                    Quaternion.LookRotation(dir) * Quaternion.Euler(0f, angle, 0f), transform);
            }
        }
    }

    private IEnumerator ShootBurst()
    {
        if (_bulletPrefabBlue == null || _firePoint == null || _player == null) yield break;
        Vector3 dir = (_player.position - _firePoint.position).normalized;
        Quaternion baseRot = Quaternion.LookRotation(dir);
        float halfSpread = _burstSpreadAngle * (_burstBulletsCount - 1) / 2f;
        for (int i = 0; i < _burstBulletsCount; i++)
        {
            float angle = -halfSpread + _burstSpreadAngle * i;
            Instantiate(_bulletPrefabBlue, _firePoint.position, baseRot * Quaternion.Euler(0f, angle, 0f), transform);
            yield return new WaitForSeconds(_burstTimeBetweenBullets);
        }
    }

    private IEnumerator RageBurst()
    {
        if (_firePoint == null) yield break;
        GameObject prefab = EnemyColor switch
        {
            ColorType.Red => _bulletPrefabRed,
            ColorType.Yellow => _bulletPrefabYellow,
            _ => _bulletPrefabBlue
        };
        if (prefab == null) yield break;

        float angleStep = 360f / _rageBulletsCount;
        for (int i = 0; i < _rageBulletsCount; i++)
        {
            float angle = angleStep * i;
            Instantiate(prefab, _firePoint.position, Quaternion.Euler(0f, angle, 0f), transform);
        }

        yield return null;
    }

    // --- Mort ---

    protected override void Die()
    {
        base.Die();
        
        _progressBar.gameObject.SetActive(false);
        WaveManager.Instance.BossKilled();
    }

    private void InitRandomForm()
    {
        if (_materials == null || _materials.Length == 0 || _meshRenderer == null) return;
        _currentMaterialIndex = Random.Range(0, _materials.Length);
        ApplyMaterial(_currentMaterialIndex);
    }

    private void SwitchToRandomForm()
    {
        if (_materials == null || _materials.Length == 0 || _meshRenderer == null) return;
        int newIndex;
        do
        {
            newIndex = Random.Range(0, _materials.Length);
        } while (newIndex == _currentMaterialIndex && _materials.Length > 1);

        _currentMaterialIndex = newIndex;
        ApplyMaterial(_currentMaterialIndex);
    }

    private void ApplyMaterial(int index)
    {
        _meshRenderer.material = _materials[index];
    
        // Determine color type based on material index (0=Red, 1=Yellow, 2=Blue or customize)
        ColorType materialColor = index switch
        {
            0 => ColorType.Red,
            1 => ColorType.Yellow,
            2 => ColorType.Blue,
            _ => ColorType.Red
        };
    
        SetBossColor(materialColor);
    }

    private void SetBossColor(ColorType colorType)
    {
        SetEnemyColor(colorType);
        switch (EnemyColor)
        {
            case ColorType.Red:
                _progressBar.SetSliderColor(Color.red);
                break;
            case ColorType.Blue:
                _progressBar.SetSliderColor(Color.blue);
                break;
            case ColorType.Yellow:
                _progressBar.SetSliderColor(Color.yellow);
                break;
            default:
                sliderImage.color = Color.black;
                break;
        }
    }

    private float GetHpPercent() => CurrentHp / MaxHp;
}