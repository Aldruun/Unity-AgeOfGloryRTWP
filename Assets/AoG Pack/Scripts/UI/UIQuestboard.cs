using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestboard : MonoBehaviour
{
    public static UIQuestboard Instance;

    protected CanvasGroup _canvasGroup;
    private Button _closeButton;
    private Transform _contentHolder;

    private QuestBoard _currQuestboard;
    private Actor _player;
    private GameObject _prefab_questUIElement;
    private List<GameObject> _questUIElements;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        _closeButton = transform.Find("Button - Close").GetComponent<Button>();
        _closeButton.onClick.AddListener(delegate { Hide(); });

        _contentHolder = transform.Find("Scroll View/Viewport/Content - Quests");
        _questUIElements = new List<GameObject>();
        for (var i = 0; i < 5; i++)
        {
            var obj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Quest UI Element"), _contentHolder);
            obj.SetActive(false);

            _questUIElements.Add(obj);
        }

        _canvasGroup = GetComponent<CanvasGroup>();

        GameEventSystem.OnQuestStarted += quest =>
        {
            if (_canvasGroup.interactable)
                RefreshQuestUIElements(_currQuestboard);
        };
        GameEventSystem.OnQuestAdded += quest =>
        {
            if (_canvasGroup.interactable)
                RefreshQuestUIElements(_currQuestboard);
        };
        GameEventSystem.OnQuestRemoved += quest =>
        {
            if (_canvasGroup.interactable)
                RefreshQuestUIElements(_currQuestboard);
        };
        GameEventSystem.OnPlayerInteraction_Questboard += (player, board) =>
        {
            _player = player;
            _currQuestboard = board;
            RefreshQuestUIElements(_currQuestboard);
            Show();
        };
        Hide();
    }

    private void RefreshQuestUIElements(QuestBoard qb)
    {
        foreach (var obj in _questUIElements) obj.SetActive(false);

        foreach (var q in qb.availableQuests)
        {
            var qObj = AddQuestUIElement();
            qObj.transform.Find("Quest Label").GetComponent<Text>().text = q.name;

            qObj.transform.Find("Quest State").GetComponent<Text>().text = q.running ? "..." : "";

            var c = qObj.GetComponentInChildren<UIClickHandler>();
            c.OnDoubleClick = null;
            c.OnDoubleClick = () => { q.AddQuestPartyMember(_player); };
        }
    }

    public static void Show()
    {
        Instance._canvasGroup.alpha = 1;
        Instance._canvasGroup.blocksRaycasts = true;
        Instance._canvasGroup.interactable = true;
    }

    public static void Hide()
    {
        Instance._canvasGroup.alpha = 0;
        Instance._canvasGroup.blocksRaycasts = false;
        Instance._canvasGroup.interactable = false;
    }

    public static void Toggle()
    {
        if (Instance._canvasGroup.alpha == 1)
        {
            Instance._canvasGroup.alpha = 0;
            Instance._canvasGroup.blocksRaycasts = false;
            Instance._canvasGroup.interactable = false;
        }

        if (Instance._canvasGroup.alpha == 0)
        {
            Instance._canvasGroup.alpha = 1;
            Instance._canvasGroup.blocksRaycasts = true;
            Instance._canvasGroup.interactable = true;
        }
    }

    private GameObject AddQuestUIElement()
    {
        GameObject sbObj = null;

        foreach (var cachedObj in _questUIElements)
            // Pick an inactive object from the pool and leave the loop
            if (cachedObj.activeSelf == false)
            {
                //Debug.Log("Found cached popup obj");
                sbObj = cachedObj;

                break;
            }

        // No inactive object found in the pool, so let's create a new one
        if (sbObj == null)
        {
            sbObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Quest UI Elements"), _contentHolder);

            _questUIElements.Add(sbObj);
        }

        //if(popup.InRange)
        sbObj.SetActive(true);

        return sbObj;
    }
}