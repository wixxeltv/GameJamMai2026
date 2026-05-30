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

    [Header("Formes (modèles + couleurs)")]
    [SerializeField] private BossForm[] _forms;

    [Header("Bullets")]
    [SerializeField] private GameObject _bulletPrefabRed;
    [SerializeField] private GameObject _bulletPrefabYellow;
    [SerializeField] private GameObject _bulletPrefabBlue;
    [SerializeField] private Transform _firePoint;

    [Header("Mouvement en 8")]
    [SerializeField] private float _figureEightWidth = 8f;
    [SerializeField] private float _figureEightHeight = 4f;
    [SerializeField] private float _figureEightSpeedPhase1 = 0.6f;
    [SerializeField] private float _figureEightSpeedPhase2 = 1.0f;
    [SerializeField] private float _figureEightSpeedPhase3 = 1.8f;
    [SerializeField] private Vector3 _centerOffset = Vector3.zero;

    [Header("Tir - Rouge (Spiral)")]
    [SerializeField] private int _spiralBulletsPerVolley = 5;
    [SerializeField] private float _spiralFireInterval = 1.5f;

    [Header("Tir - Jaune (Shooter)")]
    [SerializeField] private float _shooterFireInterval = 1.2f;

    [Header("Tir - Bleu (Burst)")]
    [SerializeField] private int _burstBulletsCount = 5;
    [SerializeField] private float _burstSpreadAngle = 20f;
    [SerializeField] private float _burstFireInterval = 2f;
    [SerializeField] private float _burstTimeBetweenBullets = 0.08f;

    [Header("Phases")]
    [SerializeField] private float _colorSwitchInterval = 6f;
    [SerializeField] private int _rageBulletsCount = 12;
    [SerializeField] private float _phase3IntervalMultiplier = 0.55f;
    [SerializeField] private int _phase3SwitchEveryNShots = 4;
    [Range(0f, 1f)] [SerializeField] private float _phase2HpThreshold = 0.6f;
    [Range(0f, 1f)] [SerializeField] private float _phase3HpThreshold = 0.2f;

    [Header("Transition de phase")]
    [SerializeField] private float _transitionDuration = 1.5f;
    [SerializeField] private float _transitionVibrateAmount = 0.3f;

    private int _currentPhase = 0;
    private int _currentFormIndex = 0;
    private float _figureEightT;
    private Vector3 _spawnCenter;
    private float _volleyAngle;
    private bool _isTransitioning;

    protected override void Start()
    {
        base.Start();
        _spawnCenter = transform.position;
        InitRandomForm();
        StartCoroutine(PhaseRoutine());
    }

    private void Update()
    {
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

        yield return StartCoroutine(PhaseTransition());
        _currentPhase = 2;
        yield return StartCoroutine(Phase2());

        yield return StartCoroutine(PhaseTransition());
        _currentPhase = 3;
        yield return StartCoroutine(Phase3());
    }

    private IEnumerator PhaseTransition()
    {
        _isTransitioning = true;
        Vector3 frozenPos = transform.position;
        float elapsed = 0f;

        while (elapsed < _transitionDuration)
        {
            if (!IsAlive) { _isTransitioning = false; yield break; }
            elapsed += Time.deltaTime;
            Vector3 shake = Random.insideUnitSphere * _transitionVibrateAmount;
            shake.y = 0f;
            transform.position = frozenPos + shake;
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
            Instantiate(_bulletPrefabRed, _firePoint.position, Quaternion.Euler(0f, angle, 0f));
        }
        _volleyAngle += 15f;
    }

    private void ShootStraight(int count = 1)
    {
        if (_bulletPrefabYellow == null || _firePoint == null || _player == null) return;
        Vector3 dir = (_player.position - _firePoint.position).normalized;
        if (count == 1)
        {
            Instantiate(_bulletPrefabYellow, _firePoint.position, Quaternion.LookRotation(dir));
        }
        else
        {
            float spread = 15f;
            float half = spread * (count - 1) / 2f;
            for (int i = 0; i < count; i++)
            {
                float angle = -half + spread * i;
                Instantiate(_bulletPrefabYellow, _firePoint.position, Quaternion.LookRotation(dir) * Quaternion.Euler(0f, angle, 0f));
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
            Instantiate(_bulletPrefabBlue, _firePoint.position, baseRot * Quaternion.Euler(0f, angle, 0f));
            yield return new WaitForSeconds(_burstTimeBetweenBullets);
        }
    }

    private IEnumerator RageBurst()
    {
        if (_firePoint == null) yield break;
        GameObject prefab = EnemyColor switch
        {
            ColorType.Red    => _bulletPrefabRed,
            ColorType.Yellow => _bulletPrefabYellow,
            _                => _bulletPrefabBlue
        };
        if (prefab == null) yield break;

        float angleStep = 360f / _rageBulletsCount;
        for (int i = 0; i < _rageBulletsCount; i++)
        {
            float angle = angleStep * i;
            Instantiate(prefab, _firePoint.position, Quaternion.Euler(0f, angle, 0f));
        }
        yield return null;
    }

    // --- Mort ---

    protected override void Die()
    {
        DisableAllModels();
        base.Die();
    }

    // --- Formes ---

    private void DisableAllModels()
    {
        if (_forms == null) return;
        foreach (var form in _forms)
            if (form.model != null && form.model != gameObject)
                form.model.SetActive(false);
    }

    private void InitRandomForm()
    {
        if (_forms == null || _forms.Length == 0) return;
        _currentFormIndex = Random.Range(0, _forms.Length);
        ApplyForm(_currentFormIndex);
    }

    private void SwitchToRandomForm()
    {
        if (_forms == null || _forms.Length == 0) return;
        int newIndex;
        do { newIndex = Random.Range(0, _forms.Length); }
        while (newIndex == _currentFormIndex && _forms.Length > 1);
        _currentFormIndex = newIndex;
        ApplyForm(_currentFormIndex);
    }

    private void ApplyForm(int index)
    {
        for (int i = 0; i < _forms.Length; i++)
        {
            if (_forms[i].model == null || _forms[i].model == gameObject) continue;
            _forms[i].model.SetActive(i == index);
            if (i == index)
            {
                var r = _forms[i].model.GetComponent<Renderer>()
                     ?? _forms[i].model.GetComponentInChildren<Renderer>();
                if (r != null) r.SetPropertyBlock(null);
            }
        }
        SetBossColor(_forms[index].color);
    }

    private void SetBossColor(ColorType colorType) => SetEnemyColor(colorType);

    private float GetHpPercent() => CurrentHp / MaxHp;
}
