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
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;

    [Header("Phase 1 - Tir simple")]
    [SerializeField] private float _phase1FireInterval = 2f;

    [Header("Phase 2 - Changement de couleur")]
    [SerializeField] private float _phase2FireInterval = 1.2f;
    [SerializeField] private float _colorSwitchInterval = 3f;

    [Header("Phase 3 - Chaos")]
    [SerializeField] private float _phase3FireInterval = 0.6f;

    private int _currentPhase = 0;
    private int _currentFormIndex = 0;
    private Renderer _bossRenderer;

    protected override void Start()
    {
        base.Start();
        _bossRenderer = GetComponentInChildren<Renderer>();
        InitRandomForm();
        StartCoroutine(PhaseRoutine());
    }

    private void Update()
    {
        if (_player == null || !IsAlive) return;
        
        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist > 8f)
        {
            Vector3 dir = (_player.position - transform.position).normalized;
            transform.position += dir * _moveSpeed * Time.deltaTime;
        }
    }
    

    private IEnumerator PhaseRoutine()
    {
        // Phase 1
        _currentPhase = 1;
        yield return StartCoroutine(Phase1());

        // Phase 2
        _currentPhase = 2;
        yield return StartCoroutine(Phase2());

        // Phase 3
        _currentPhase = 3;
        yield return StartCoroutine(Phase3());
    }

    private IEnumerator Phase1()
    {
        while (IsAlive && GetHpPercent() > 0.6f)
        {
            Shoot();
            yield return new WaitForSeconds(_phase1FireInterval);
        }
    }

    private IEnumerator Phase2()
    {
        StartCoroutine(ColorSwitchRoutine());

        while (IsAlive && GetHpPercent() > 0.2f)
        {
            Shoot();
            yield return new WaitForSeconds(_phase2FireInterval);
        }
    }

    private IEnumerator Phase3()
    {
        while (IsAlive)
        {
            SwitchToRandomForm();
            Shoot();
            yield return new WaitForSeconds(_phase3FireInterval);
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
    

    private void Shoot()
    {
        if (_bulletPrefab == null || _firePoint == null || _player == null) return;
        Vector3 dir = (_player.position - _firePoint.position).normalized;
        Instantiate(_bulletPrefab, _firePoint.position, Quaternion.LookRotation(dir));
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
            if (_forms[i].model != null)
                _forms[i].model.SetActive(i == index);

        SetBossColor(_forms[index].color, _forms[index].visualColor);
    }

    private void SetBossColor(ColorType colorType, Color visualColor)
    {
        SetEnemyColor(colorType);

        if (_bossRenderer != null)
            _bossRenderer.material.color = visualColor;
    }

    private float GetHpPercent() => CurrentHp / MaxHp;
}
