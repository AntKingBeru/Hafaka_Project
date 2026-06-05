using UnityEngine;

public class Melee : PlayerForm
{
    [Header("Passive")]
    [SerializeField, Range(0f, 1f)] private float damageMultiplier = 0.5f;
    
    [Header("Melee Attack")]
    [SerializeField] private float meleeRange = 1.2f;
    [SerializeField] private float meleeRadius = 0.4f;
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private float meleeCooldown = 0.4f;
    [SerializeField] private LayerMask meleeTargetLayers;
    [SerializeField] private int meleeBufferSize = 16;
    
    [Header("Ground Slam")]
    [SerializeField] private float slamRadius = 2.5f;
    [SerializeField] private float slamDamage = 40f;
    [SerializeField] private float slamCooldown = 1.2f;
    [SerializeField] private LayerMask slamTargetLayers;
    [SerializeField] private int slamBufferSize = 24;

    private float _lastMeleeTime = -999f;
    private float _lastSlamTime = -999f;

    private Collider[] _meleeBuffer;
    private Collider[] _slamBuffer;
    
    public override float GetDamageMultiplier() => damageMultiplier;
    public override void OnActive1Pressed() => TryMeleeAttack();
    public override void OnActive2Pressed() => TryGroundSlam();

    private void Awake()
    {
        _meleeBuffer = new Collider[meleeBufferSize];
        _slamBuffer = new Collider[slamBufferSize];
    }

    private void TryMeleeAttack()
    {
        if (Time.time - _lastMeleeTime < meleeCooldown)
            return;
        _lastMeleeTime = Time.time;
        
        var origin = Player.transform.position;
        var dir = Player.FacingDirection;

        var point1 = origin + new Vector3(dir * meleeRange, 0f, 0f);
        
        var hitCount = Physics.OverlapCapsuleNonAlloc(origin, point1, meleeRange, _meleeBuffer, meleeTargetLayers);

        for (var i = 0; i < hitCount; i++)
            if (_meleeBuffer[i].TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(meleeDamage);
        
        FormManager.Instance.CurrentForm.OnAnyAbilityUsed();
        Debug.Log("[MeleeForm] Melee Attack!");
    }

    private void TryGroundSlam()
    {
        if (!Player.IsGrounded)
            return;
        if (Time.time - _lastSlamTime < slamCooldown)
            return;
        _lastSlamTime = Time.time;

        var hitCount = Physics.OverlapSphereNonAlloc(Player.transform.position, slamRadius, _slamBuffer, meleeTargetLayers);

        for (var i = 0; i < hitCount; i++)
            if (_slamBuffer[i].TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(slamDamage);
        
        FormManager.Instance.CurrentForm.OnAnyAbilityUsed();
        Debug.Log("[MeleeForm] Ground Slam!");
    }

    private void OnDrawGizmosSelected()
    {
        if (!Player)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Player.transform.position, slamRadius);
        
        Gizmos.color = Color.yellow;
        var p1 = Player.transform.position;
        var p2 = p1 + new Vector3(Player.FacingDirection * meleeRadius, 0f, 0f);
        Gizmos.DrawWireSphere(p1, meleeRadius);
        Gizmos.DrawWireSphere(p2, meleeRadius);
        Gizmos.DrawLine(p1 + Vector3.up * meleeRadius, p2 + Vector3.up * meleeRadius);
        Gizmos.DrawLine(p1 - Vector3.up * meleeRadius, p2 - Vector3.up * meleeRadius);
    }
}