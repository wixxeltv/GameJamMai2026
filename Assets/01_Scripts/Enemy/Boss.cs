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
    [SerializeField] private float _figureEightSpeed = 0.8f;
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

    private int _currentPhase = 0;
    private int _currentFormIndex = 0;
    private float _figureEightT;
    private Vector3 _spawnCenter;
    private float _volleyAngle;

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
        _figureEightT += _figureEightSpeed * Time.deltaTime;
        float x = _figureEightWidth * Mathf.Sin(_figureEightT);
        float z = _figureEightHeight * Mathf.Sin(_figureEightT * 2f);
        transform.position = _spawnCenter + _centerOffset + new Vector3(x, 0f, z);
    }

    // --- Phases ---

    private IEnumerator PhaseRoutine()
    {
        _currentPhase = 1;
        yield return StartCoroutine(Phase1());

        _currentPhase = 2;
        yield return StartCoroutine(Phase2());

        _currentPhase = 3;
        yield return StartCoroutine(Phase3());
    }

    private IEnumerator Phase1()
    {
        while (IsAlive && GetHpPercent() > 0.6f)
            yield return StartCoroutine(ShootByColor());
    }

    private IEnumerator Phase2()
    {
        StartCoroutine(ColorSwitchRoutine());
        while (IsAlive && GetHpPercent() > 0.2f)
            yield return StartCoroutine(ShootByColor());
    }

    private IEnumerator Phase3()
    {
        while (IsAlive)
        {
            SwitchToRandomForm();
            yield return StartCoroutine(ShootByColor());
        }
    }

    private IEnumerator ColorSwitchRoutine()
    {
        while (IsAlive && _currentPhase == 2)
        {
            yield return new WaitForSeconds(_colorSwitchInterval);
            SwitchToRandomForm();
        }
    }

    // --- Tir selon couleur ---

    private IEnumerator ShootByColor()
    {
        switch (EnemyColor)
        {
            case ColorType.Red:
                ShootSpiral();
                yield return new WaitForSeconds(_spiralFireInterval);
                break;
            case ColorType.Yellow:
                ShootStraight();
                yield return new WaitForSeconds(_shooterFireInterval);
                break;
            default: // Blue
                yield return StartCoroutine(ShootBurst());
                yield return new WaitForSeconds(_burstFireInterval);
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

    private void ShootStraight()
    {
        if (_bulletPrefabYellow == null || _firePoint == null || _player == null) return;
        Vector3 dir = (_player.position - _firePoint.position).normalized;
        Instantiate(_bulletPrefabYellow, _firePoint.position, Quaternion.LookRotation(dir));
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
            if (_forms[i].model != null && _forms[i].model != gameObject)
                _forms[i].model.SetActive(i == index);
        SetBossColor(_forms[index].color, _forms[index].visualColor);
    }

    private void SetBossColor(ColorType colorType, Color visualColor)
    {
        SetEnemyColor(colorType);
        if (_currentFormIndex < _forms.Length && _forms[_currentFormIndex].model != null)
        {
            var r = _forms[_currentFormIndex].model.GetComponent<Renderer>()
                 ?? _forms[_currentFormIndex].model.GetComponentInChildren<Renderer>();
            if (r != null) r.material.color = visualColor;
        }
    }

    private float GetHpPercent() => CurrentHp / MaxHp;
}
