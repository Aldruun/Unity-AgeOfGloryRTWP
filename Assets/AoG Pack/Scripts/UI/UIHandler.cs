using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace AoG.UI
{
    public enum InputMode
    {
        Selection,
        Construction
    }

    public enum CursorType
    {
        INVALID = -1,
        NORMAL = 0,
        TAKE = 2,
        WALK = 4,
        BLOCKED = 6,
        USE = 8,
        WAIT = 10,
        ATTACK = 12,
        SWAP = 14,
        DEFEND = 16,
        TALK = 18,
        TALKCLICK,
        CAST = 20,
        INFO = 22,
        LOCK = 24,
        LOCK2 = 26,
        STAIR = 28,
        DOOR = 30,
        CHEST = 32,
        TRAVEL = 34,
        STEALTH = 36,
        TRAP = 38,
        PICK = 40,
        PASS = 42,
        GRAB = 44,
        WAY = 46,
        INFO2 = 46,
        PORTAL = 48,
        STAIR2 = 50,
        EXTRA = 52,
        RESIZEVERT = 53,
        RESIZEHOR = 54,
        MASK = 127,
        GRAY = 128
    }

    public class UIHandler
    {
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pos);

        public static Action OnUIEnabled;
        public static Action OnUIDisabled;
        //public static Dictionary<GameObject, Action> activePanels = new Dictionary<GameObject, Action>();
        public static GameObject activePanel;

        public bool showGameTimeOnSart;
        internal RectTransform selectionBoxVisual;
        private static bool gamePaused;
        private static Canvas[] canvasGroup;
        private static float storedTimeScale;
        private readonly GameObject worldMapPanel;
        private readonly GameObject portraitPanel;
        private readonly RectTransform cursorRect;
        private readonly GameObject gameMenu;
        private readonly GameObject inventory;
        private readonly Text txt_gamespeed;
        private readonly Button btn_timeslower;
        private readonly Button btn_timefaster;
        private readonly float[] gameSpeedStages;
        private readonly ColorAdjustments _glbColorAdj;
        private readonly GameObject uiMaster;
        private readonly Transform playerInterface;
        private readonly GameObject playerControls;
        private readonly UIPartyPotraitManager partyPotraitManager;
        private readonly UIDialogBox dialogBox;
        private Canvas mainCanvas;
        private GameObject ingameInterface;
        private Toggle tgl_partyai;
        private int gameSpeedIndex;

        //float _mouseDelta;
        private bool _startedOverUI;
        private Volume _globalPPVolume;
        private UIFloatingInfoManager floatingInfoManager;

        public bool GamePaused
        {
            get => gamePaused;
            set
            {
                gamePaused = value;
                //Debug.Log("Game Paused");
                if(gamePaused)
                {
                    GameSpeedZero();
                }
                else
                {
                    GameSpeedNormal();
                }
            }
        }

        public UIHandler(Camera camera, Volume ppVolume)
        {
            GameEventSystem.RequestCloseDialogueBox -= CloseDialogbox;
            GameEventSystem.RequestCloseDialogueBox += CloseDialogbox;

            uiMaster = GameObject.FindWithTag("UIMaster");

            dialogBox = new UIDialogBox(uiMaster.transform.Find("Canvas - Dialog Box/Dialogbox"));
            dialogBox.dialogboxPanel.parent.gameObject.SetActive(false);
            cursorRect = uiMaster.transform.Find("Canvas - Persistent/Cursor").GetComponent<RectTransform>();
            SetCursor(CursorType.NORMAL);
            gameMenu = uiMaster.transform.Find("Canvas - Main Menu").gameObject;
            playerInterface = uiMaster.transform.Find("Canvas - Player Interface");
            playerInterface.gameObject.SetActive(true);
            playerControls = playerInterface.Find("Player Controls").gameObject;

            floatingInfoManager = new UIFloatingInfoManager(playerInterface.Find("Floating Info Holder"), camera);
            selectionBoxVisual = playerInterface.Find("Selection Rect").GetComponent<RectTransform>();

            gameMenu.SetActive(false);
            inventory = playerInterface.Find("Inventory UI/Inventory Panel").gameObject;
            inventory.SetActive(false);
            worldMapPanel = playerInterface.Find("World Map").gameObject;
            worldMapPanel.SetActive(false);
            portraitPanel = playerControls.transform.Find("Portrait Panel/content").gameObject;
            portraitPanel.SetActive(true);
            
            partyPotraitManager = new UIPartyPotraitManager(portraitPanel.transform);
            GameEventSystem.RequestCreatePortrait = partyPotraitManager.CreatePortrait;
            GameEventSystem.RequestRemovePortrait = partyPotraitManager.RemovePortrait;
            GameEventSystem.RequestHighlightPCPortrait = partyPotraitManager.HighlightPartymemberPortrait;

            GameObject gameSpeedPanel = playerControls.transform.Find("Top Bar/Game Speed Manipulator/").gameObject;
            txt_gamespeed = gameSpeedPanel.transform.Find("_txt_gamespeed").GetComponent<Text>();

            btn_timeslower = gameSpeedPanel.transform.Find("_btn_timeSlower").GetComponent<Button>();
            btn_timeslower.onClick.RemoveAllListeners();
            btn_timeslower.onClick.AddListener(() => GameSpeedDecrease());

            btn_timefaster = gameSpeedPanel.transform.Find("_btn_timeFaster").GetComponent<Button>();
            btn_timefaster.onClick.RemoveAllListeners();
            btn_timefaster.onClick.AddListener(() => GameSpeedIncrease());

            tgl_partyai = playerControls.transform.Find("Toggle Party AI/Toggle").GetComponent<Toggle>();
            tgl_partyai.onValueChanged.RemoveAllListeners();
            tgl_partyai.onValueChanged.AddListener((on) => GameEventSystem.RequestTogglePartyAI(on));

            gameSpeedStages = new float[] { 0.2f, 0.5f, 1f, 2f, 4f };
            gameSpeedIndex = 2;

            VolumeProfile profile = ppVolume?.profile;
            ColorAdjustments c;
            if(profile.TryGet(out c) == false)
                throw new System.NullReferenceException(nameof(c));
            _glbColorAdj = c;

            //_glbColorAdj.saturation
            //! -------------
            mainCanvas = uiMaster.GetComponentInParent<Canvas>();

            UIHandler.storedTimeScale = 1;
            canvasGroup = uiMaster.GetComponentsInChildren<Canvas>();

            GameEventSystem.UIRequestShowAIToggleAsOn = SetUIAIToggleOn;
            GameEventSystem.OnSetTacticalPause -= SetTacticalPause;
            GameEventSystem.OnSetTacticalPause += SetTacticalPause;

            inventory.SetActive(false);
        }

        public static bool UIOnScreen(Vector2 uiScreenPosition)
        {
            Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

            return screenRect.Contains(uiScreenPosition);
        }

        public void Release()
        {
            GameEventSystem.RequestCloseDialogueBox -= CloseDialogbox;
            GameEventSystem.OnSetTacticalPause -= SetTacticalPause;
        }

        public void UpdateFloatingInfo()
        {
            floatingInfoManager.Update();
        }

        public void Update(ref bool panelActive)
        {
            //Cursor.visible = false;

            //Debug.Log(activePanels.Count);
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(GameEventSystem.isAimingSpell)
                {
                    return;
                }

                SetActivePanel(null);

                ToggleGameMenu();
            }

            if(Input.GetKeyDown(KeyCode.M))
            {
                worldMapPanel.SetActive(worldMapPanel.activeInHierarchy == false);
                activePanel = worldMapPanel.activeInHierarchy ? worldMapPanel : null;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                //OnSpacebarPressed?.Invoke();

                _glbColorAdj.saturation.Override(GamePaused == false ? -100 : 0);
                SetTacticalPause(GamePaused == false);
            }

            cursorRect.position = (Vector2)Input.mousePosition;

            if(Input.GetKeyDown(KeyCode.I))
            {
                if(GameEventSystem.isAimingSpell)
                    return;

                bool visible = inventory.activeInHierarchy;
                UIInventory.ToggleActive(!visible);
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                //foreach(Door door in GameStateManager.Instance.GetCurrentGame().GetCurrentMap().doors)
                //{
                //    door.highlightObject.Highlight();
                //}
                //foreach(Container container in GameStateManager.Instance.GetCurrentGame().GetCurrentMap().containers)
                //{
                //    container.highlightObject.Highlight();
                //}
                //HighlightableMonoObject.HighlightAll(true);
                //ContainerMonoObject.HighlightAll(true);
                GameEventSystem.showFloatingInfo = true;
            }

            if(Input.GetKeyUp(KeyCode.Tab))
            {
                //foreach(Door door in GameStateManager.Instance.GetCurrentGame().GetCurrentMap().doors)
                //{
                //    door.highlightObject.Unhighlight();
                //}
                //foreach(Container container in GameStateManager.Instance.GetCurrentGame().GetCurrentMap().containers)
                //{
                //    container.highlightObject.Unhighlight();
                //}
                //DoorMonoObject.HighlightAll(false);
                //HighlightableMonoObject.HighlightAll(false);
                GameEventSystem.showFloatingInfo = false;
            }

            if((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
            {
                _startedOverUI = EventSystem.current.IsPointerOverGameObject(-1);

                if(_startedOverUI)
                    return;
            }
            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                //_mouseDelta = 0;

                if(_startedOverUI)
                    return;
            }
            if(Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if(_startedOverUI)
                    return;
            }

            panelActive = activePanel != null;
        }

        public void ShowDialogbox(Actor talkingPC, ConversationData data)
        {
            SetActivePanel(dialogBox.dialogboxPanel.gameObject);
            playerControls.SetActive(false);
            dialogBox.dialogboxPanel.parent.gameObject.SetActive(true);
            dialogBox.Setup(talkingPC, data);
            Time.timeScale = 0.000000001f;
            //GameStateManager.Instance.core.SetGameStateFlags(GameStateFlags.DIALOGPAUSE);
        }

        //void ShowWorldMap(bool on)
        //{
        //    _worldMapPanel.gameObject.SetActive(on);
        //}
        public void CloseDialogbox()
        {
            Time.timeScale = 1;
            dialogBox.Hide();
            dialogBox.dialogboxPanel.parent.gameObject.SetActive(false);

            playerControls.SetActive(true);
            //GameStateManager.Instance.core.DisableGameStateFlags(GameStateFlags.DIALOGPAUSE);
            activePanel = null;
        }

        public void SetCursor(CursorType cursorType)
        {
            Sprite sprite = null;
            switch(cursorType)
            {
                case CursorType.NORMAL:
                    sprite = ResourceManager.cursor_default;
                    break;
                case CursorType.BLOCKED:
                    sprite = ResourceManager.cursor_blocked;
                    break;
                case CursorType.ATTACK:
                    sprite = ResourceManager.cursor_attack;
                    break;
                case CursorType.CAST:
                    sprite = ResourceManager.cursor_castspell;
                    break;
                case CursorType.USE:
                    sprite = ResourceManager.cursor_use;
                    break;
                case CursorType.RESIZEHOR:
                    sprite = ResourceManager.cursor_resizehorizontal;
                    break;
                case CursorType.RESIZEVERT:
                    sprite = ResourceManager.cursor_resizevertical;
                    break;
                case CursorType.GRAB:
                case CursorType.TAKE:
                    sprite = ResourceManager.cursor_grab;
                    break;
                default:
                    Debug.LogError("Invalid cursor index");
                    break;
            }
            cursorRect.GetComponentInChildren<Image>().overrideSprite = sprite;
            cursorRect.pivot = (sprite.pivot / sprite.rect.size);
            Debug.Log("////// CURSOR -> " + cursorType);
        }

        public void SaveGame()
        {
            //GameEventManager.Callback_SaveInitiated();
        }

        public void LoadGame()
        {
            //GameEventManager.Callback_LoadInitiated();
        }

        public void ExitGame()
        {
        }

        public void ShowInteractionText(bool show, string actionText)
        {
            //if(show)
            //    interactionText.GetComponent<Text>().text = "[" + actionText + "]";

            //interactionText.SetActive(show);
        }

        public void ToggleGameMenu()
        {
            gameMenu.SetActive(gameMenu.activeInHierarchy == false);
            GamePaused = gameMenu.activeInHierarchy;
            //ingameInterface.SetActive(gameMenu.activeInHierarchy == false);

            //gamePaused = false;
        }

        public void ToggleInventory()
        {
            //this.m_inventory.gameObject.SetActive(this.m_inventory.gameObject.activeSelf == false);
            //bool playerInvActive = this.m_inventory.gameObject.activeSelf;
            //if(playerInvActive == false) {
            inventory.SetActive(inventory.activeInHierarchy == false);
            //    this.m_inventory.gameObject.SetActive(false);
            //}
        }

        public void TogglePausePanel()
        {
            //pausePanel.SetActive(pausePanel.activeInHierarchy == false);
            //gamePaused = pausePanel.activeInHierarchy;
        }

        public void SetUIAIToggleOn(bool on)
        {
            tgl_partyai.isOn = on;
            //pausePanel.SetActive(pausePanel.activeInHierarchy == false);
            //gamePaused = pausePanel.activeInHierarchy;
        }

        public void PMSaveDialogOverwrite()
        {
            // overwrite the selected save game
        }

        //public void ShowPMSaveDialog()
        //{
        //    pauseSaveGameMenu.SetActive(true);
        //}
        public void PMSaveDialogSave()
        {
            // save the game
        }

        //public void HidePauseMenuOptions()
        //{
        //    pauseOptionsMenu.SetActive(false);
        //}
        public void ClosePMSaveDialog()
        {
            //pauseSaveGameMenu.SetActive(false);
        }

        //public void ClosePMOptionsAndSave()
        //{
        //    pauseOptionsMenu.SetActive(false);
        //}
        public void ShowPMLoadDialog()
        {
            //pausePanel.SetActive(true);
        }

        //public void ShowPauseMenuOptions()
        //{
        //    pauseOptionsMenu.SetActive(true);
        //}
        public void ClosePMLoadDialog()
        {
            //pausePanel.SetActive(false);
        }

        public void PlayPauseMenuButtonPressedSound()
        {
        }

        public void GameSpeedZero()
        {
            Time.timeScale = 0.00000000001f;

            txt_gamespeed.text = 0.ToString() + "x";
        }

        public void GameSpeedNormal()
        {
            gameSpeedIndex = 2;
            Time.timeScale = 1;
            txt_gamespeed.text = 1.ToString() + "x";
        }

        public void GameSpeedDecrease()
        {
            if(GamePaused)
            {
                GamePaused = false;
            }

            gameSpeedIndex--;

            gameSpeedIndex = (int)Mathf.Clamp(gameSpeedIndex, gameSpeedStages[0], gameSpeedStages[gameSpeedStages.Length - 1]);
            Time.timeScale = gameSpeedStages[gameSpeedIndex];
            txt_gamespeed.text = Time.timeScale.ToString() + "x";
        }

        public void GameSpeedIncrease()
        {
            if(GamePaused)
            {
                _glbColorAdj.saturation.Override(0);
                GamePaused = false;
                return;
            }

            gameSpeedIndex++;

            gameSpeedIndex = (int)Mathf.Clamp(gameSpeedIndex, gameSpeedStages[0], gameSpeedStages[gameSpeedStages.Length - 1]);
            Time.timeScale = Mathf.Clamp(gameSpeedStages[gameSpeedIndex], gameSpeedStages[0], gameSpeedStages[gameSpeedStages.Length - 1]);
            txt_gamespeed.text = Time.timeScale.ToString() + "x";
        }

        private void SetActivePanel(GameObject panel)
        {
            if(activePanel != null)
            {
                activePanel.SetActive(false);
                activePanel = null;
            }
            activePanel = panel;
        }

        private void SetTacticalPause(bool on)
        {
            GamePaused = on;
            if(on)
            {
                DevConsole.Log("<color=red>PAUSED</color>");
            }
            else
            {
                DevConsole.Log("<color=red>UNPAUSED</color>");
            }
        }

        //private void DeactivateAllPauseMenuPanels()
        //{
        //    foreach(GameObject go in pauseMenuElements)
        //        go.SetActive(false);
        //}

        //private void ActivatePauseMenuPanelByIndex(int panelIndex)
        //{
        //    pauseMenuElements[panelIndex].SetActive(true);
        //    for(int i = 0; i < pauseMenuElements.Count; i++)
        //    {
        //        if(i == panelIndex)
        //            continue;
        public struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}