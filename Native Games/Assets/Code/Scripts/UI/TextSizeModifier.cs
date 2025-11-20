using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextSizeModifier : MonoBehaviour
{
    [SerializeField] private bool onAwake = false;
    [SerializeField] private int sizePercentage = 75;
    [SerializeField] private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        sizePercentage = Mathf.CeilToInt(sizePercentage);

        if (onAwake)
        {
            Unmodify();
            Modify();
        }
    }

    public void Modify()
    {
        char[] characters = textMeshPro.text.ToCharArray();
        char lastCharacter = ' ';
        string finalString = string.Empty;
        string bufferString = string.Empty;

        foreach (char character in characters)
        {
            if (character != ' ' && character != '\n'
                && char.IsLetter(character) && !char.IsUpper(character)
                && !bufferString.Contains("<size="))
            {
                bufferString = string.Empty;
                bufferString = $"<size={sizePercentage}%>{character}</size>";
            }
            else
            {
                bufferString = string.Empty;
                bufferString = $"{character}";
            }

            lastCharacter = character;
            finalString += bufferString;
        }

        textMeshPro.text = string.Empty;
        textMeshPro.text = finalString;
    }

    public void Unmodify()
    {
        string currentString = textMeshPro.text;
        currentString = currentString.Replace($"<size={sizePercentage}%>", "");
        currentString = currentString.Replace($"</size>", "");
        textMeshPro.text = currentString;
    }
}
