using UnityEngine;

public class FormManager : MonoBehaviour
{
    public static FormManager Instance { get; private set; }
    
    [Header("Form References")]
    [SerializeField] private Melee meleeForm;
    [SerializeField] private Ranged rangedForm;
    [SerializeField] private Traversal traversalForm;

    private PlayerForm[] _forms;
    private int _currentFormIndex = 0;
    
    public PlayerForm CurrentForm => _forms[_currentFormIndex];

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(PlayerController player)
    {
        _forms = new PlayerForm[] { meleeForm, rangedForm, traversalForm };
        foreach (var form in _forms)
            form.Initialize(player);
        _forms[_currentFormIndex].OnFormEnter();
    }

    public void SwitchToForm(int index)
    {
        if (index < 0 || index >= _forms.Length)
            return;
        if (index == _currentFormIndex)
            return;
        _forms[_currentFormIndex].OnFormExit();
        _currentFormIndex = index;
        Debug.Log($"Switching to {_forms[index].GetType().Name}");
        _forms[_currentFormIndex].OnFormEnter();
    }
    
    public void OnUpdate() => CurrentForm.OnUpdate();
    public void OnFixedUpdate() => CurrentForm.OnFixedUpdate();
    
    public void OnActive1Pressed() => CurrentForm.OnActive1Pressed();
    public void OnActive2Pressed() => CurrentForm.OnActive2Pressed();
}