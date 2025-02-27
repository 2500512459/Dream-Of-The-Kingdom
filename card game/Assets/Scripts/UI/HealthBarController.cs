using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarController : MonoBehaviour
{
    [Header("Elements")]
    public Transform healthBarTransform;
    private UIDocument healthBarDocument;
    private ProgressBar healthBar;
    private CharacterBase currentCharacter;

    private VisualElement defenseElement;
    private Label defenseAmountLabel;

    private void Awake()
    {
        currentCharacter = GetComponent<CharacterBase>();
    }

    private void Start()
    {
        InitHealthBar();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 size)
    {
        Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, size, Camera.main);
        element.transform.position = rect.position;
    }

    [ContextMenu("Get UI Position")]
    public void InitHealthBar()
    {
        healthBarDocument = GetComponent<UIDocument>();
        healthBar = healthBarDocument.rootVisualElement.Q<ProgressBar>("HealthBar");
        healthBar.highValue = currentCharacter.MaxHP;

        defenseElement = healthBar.Q<VisualElement>("Defense");
        defenseAmountLabel = defenseElement.Q<Label>("DefenseAmount");
        defenseElement.style.display = DisplayStyle.None;


        MoveToWorldPosition(healthBar, healthBarTransform.position, Vector2.zero);
    }

    public void UpdateHealthBar()
    {
        if (currentCharacter.isDead)
        {
            healthBar.style.display = DisplayStyle.None;
            return;
        }

        if (healthBar != null)
        {
            healthBar.title = $"{currentCharacter.CurrentHP}/{currentCharacter.MaxHP}";
            healthBar.value = currentCharacter.CurrentHP;

            healthBar.RemoveFromClassList("highHealth");
            healthBar.RemoveFromClassList("mediumHealth");
            healthBar.RemoveFromClassList("lowHealth");

            var percentage = (float)currentCharacter.CurrentHP / currentCharacter.MaxHP;
            if (percentage < 0.3f)
            {
                healthBar.AddToClassList("lowHealth");
            }
            else if (percentage < 0.6f)
            {
                healthBar.AddToClassList("mediumHealth");
            }
            else
            {
                healthBar.AddToClassList("highHealth");
            }

            //¸üÐÂ·ÀÓùÁ¦
            defenseElement.style.display = currentCharacter.defense.currentValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            defenseAmountLabel.text = currentCharacter.defense.currentValue.ToString();
        }
    }
}
