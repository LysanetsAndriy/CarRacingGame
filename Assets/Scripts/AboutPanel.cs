using UnityEngine;
using TMPro;

public class AboutPanel : MonoBehaviour
{
    [Header("About Section")]
    [SerializeField] private TextMeshProUGUI _aboutSection;
    [SerializeField] private string _aboutText;
    [SerializeField] private string _gameName = "Car Racing Game";
    [SerializeField] private string _groupName = "МІ-31";
    [SerializeField] private string _authorName = "Лисанець Андрій Вікторович";

    private void Awake()
    {
        CheckFields();
        FillTextComponent();
    }

    private void OnValidate()
    {
        CheckFields();
        FillTextComponent();
    }

    private void CheckFields()
    {
        if (_aboutSection == null)
        {
            Debug.LogError($"Text component {this.gameObject.name} was not assigned!");
        }

        if (string.IsNullOrEmpty(_aboutText))
        {
            if (string.IsNullOrEmpty(_gameName) == false &&
                string.IsNullOrEmpty(_groupName) == false &&
                string.IsNullOrEmpty(_authorName) == false)
            {
                _aboutText = $"Гра під назвою {_gameName}\n" +
                    $"Виконана студентом групи {_groupName}\n" +
                    $"{_authorName}\n";
            }
            else
            {
                _aboutText = $"Гра під назвою Car Racing Game\n" +
                    $"Виконана студентом групи МІ-31\n" +
                    $"Лисанець Андрій Вікторович\n";
            }


        }
    }

    private void FillTextComponent()
    {
        _aboutSection.text = _aboutText;
    }

}
