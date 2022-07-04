using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    private Vector3 offset;

    private RectTransform bgRectTransform;
    private Text text;

    private void Awake()
    {
        
        bgRectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<Text>();
        Hide();

        GameEventSystem.RequestShowTooltip = Show;
        GameEventSystem.RequestShowTooltipAtMousePosition = ShowAtMousePosition;
        GameEventSystem.RequestHideTooltip = Hide;
    }

    public void ShowAtMousePosition(string content)
    {
        text.text = content;
        float padding = 4;
        bgRectTransform.sizeDelta = new Vector2(text.preferredWidth + padding * 2,
            text.preferredHeight + padding * 2);
        transform.position = Input.mousePosition;
        gameObject.SetActive(true);
    }

    public void Show(Vector3 position, Vector3 offset, string content)
    {
        
        this.offset = offset;
        text.text = content;
        float padding = 16;
        transform.position = (Vector2)(position + offset);
        bgRectTransform.sizeDelta = new Vector2(bgRectTransform.sizeDelta.x,
            text.preferredHeight + (padding));
        gameObject.SetActive(true);
    }

    private float GetSizeOfWord(Text text, string word)
    {
        var textGen = new TextGenerator();
        var generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
        return textGen.GetPreferredHeight(word, generationSettings);
        //float height = textGen.GetPreferredHeight(newText, generationSettings);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}