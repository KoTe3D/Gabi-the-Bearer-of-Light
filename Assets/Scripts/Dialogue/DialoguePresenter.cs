using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Gabi.Dialogue
{
    public sealed class DialoguePresenter : MonoBehaviour
    {
        private const int MaxAvatarsPerSide = 3;

        // Куда «смотрят» исходные картинки персонажей.
        private enum SourceOrientation
        {
            FacesLeft,
            FacesRight,
            Front
        }

        [SerializeField] private StoryFlow _flow;
        [SerializeField] private TextMeshProUGUI _speakerText;
        [SerializeField] private TextMeshProUGUI _lineText;
        [SerializeField] private Transform _choiceContainer;
        [SerializeField] private Button _choiceButtonTemplate;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _dialogPanel;
        [SerializeField] private RectTransform _avatarRoot;
        [SerializeField] private RectTransform _avatarTemplate;
        [SerializeField] private SourceOrientation _sourceOrientation = SourceOrientation.FacesLeft;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _thoughtColor = new Color(0.75f, 0.75f, 0.75f);
        [SerializeField] private Color _stageColor = new Color(0.9f, 0.85f, 0.6f);
        [SerializeField] private Color _panelNormalColor = new Color(0f, 0f, 0f, 0.55f);
        [SerializeField] private Color _panelThoughtFogColor = new Color(0.6f, 0.6f, 0.65f, 0.35f);
        // Позиции трёх слотов на каждой стороне в долях экрана (0..1); тюнинг в Inspector.
        [SerializeField] private Vector2[] _rightSlotAnchors =
        {
            new Vector2(0.82f, 0.42f),
            new Vector2(0.70f, 0.36f),
            new Vector2(0.94f, 0.42f)
        };
        [SerializeField] private Vector2[] _leftSlotAnchors =
        {
            new Vector2(0.18f, 0.42f),
            new Vector2(0.30f, 0.36f),
            new Vector2(0.06f, 0.42f)
        };

        private DialogueSession _session;
        private DialogueScene _currentScene;
        private readonly StoryFlags _flags = new StoryFlags();
        private int _flowIndex;
        private bool _isFlowFinished;

        private readonly Queue<AvatarInstance> _leftAvatars = new Queue<AvatarInstance>();
        private readonly Queue<AvatarInstance> _rightAvatars = new Queue<AvatarInstance>();

        private void Awake()
        {
            _speakerText.raycastTarget = false;
            _lineText.raycastTarget = false;
            _backgroundImage.raycastTarget = false;
            _dialogPanel.raycastTarget = false;
        }

        private void Start()
        {
            StartFlow();
        }

        private void Update()
        {
            if (!WasAdvancePressed() || _isFlowFinished || _session == null)
            {
                return;
            }

            if (_session.IsFinished)
            {
                AdvanceFlow();
                return;
            }

            if (_session.CanAdvance)
            {
                _session.Advance();
                ShowCurrentNode();
            }
        }

        public void Begin(DialogueScene scene)
        {
            _currentScene = scene;
            _session = new DialogueSession(scene.Nodes, _flags);
            ApplyBackground(scene);
            ClearAvatars();
            ShowCurrentNode();
        }

        private void ApplyBackground(DialogueScene scene)
        {
            if (scene.Background != null)
            {
                _backgroundImage.sprite = scene.Background;
                _backgroundImage.color = Color.white;
            }
            else
            {
                _backgroundImage.sprite = null;
                _backgroundImage.color = scene.BackgroundColor;
            }
        }

        private void StartFlow()
        {
            if (_flow == null || _flow.Scenes.Count == 0)
            {
                return;
            }

            _flowIndex = 0;
            _isFlowFinished = false;
            Begin(_flow.Scenes[0]);
        }

        private void AdvanceFlow()
        {
            _flowIndex++;
            if (_flowIndex >= _flow.Scenes.Count)
            {
                _isFlowFinished = true;
                Debug.Log("[Dialogue] Story flow complete.");
                return;
            }

            Begin(_flow.Scenes[_flowIndex]);
        }

        private bool WasAdvancePressed()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;
            var clickedEmptySpace = mouse != null && mouse.leftButton.wasPressedThisFrame && !IsPointerOverUi();
            var spacePressed = keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
            return clickedEmptySpace || spacePressed;
        }

        private static bool IsPointerOverUi()
        {
            var eventSystem = EventSystem.current;
            return eventSystem != null && eventSystem.IsPointerOverGameObject();
        }

        private void ShowCurrentNode()
        {
            var node = _session.CurrentNode;
            ClearChoices();

            switch (node.Kind)
            {
                case DialogueNodeKind.Line:
                    ApplyLine(node.Line);
                    _choiceContainer.gameObject.SetActive(false);
                    break;
                case DialogueNodeKind.Choice:
                    _speakerText.text = string.Empty;
                    _lineText.text = string.Empty;
                    _dialogPanel.gameObject.SetActive(true);
                    SetPanelFog(false);
                    ShowChoices(node);
                    break;
                default:
                    _speakerText.text = string.Empty;
                    _lineText.text = string.Empty;
                    _choiceContainer.gameObject.SetActive(false);
                    _dialogPanel.gameObject.SetActive(false);
                    ClearAvatars();
                    Debug.Log($"[Dialogue] Scene transition requested: {_session.RequestedNextScene}");
                    break;
            }
        }

        private void ApplyLine(DialogueLine line)
        {
            var isStage = line.Kind == DialogueLineKind.StageDirection;
            var isThought = line.Kind == DialogueLineKind.Thought;
            _speakerText.text = isStage ? string.Empty : line.Speaker;
            _lineText.text = line.Text;
            _lineText.fontStyle = line.Kind == DialogueLineKind.Spoken ? FontStyles.Normal : FontStyles.Italic;
            _lineText.alignment = isStage ? TextAlignmentOptions.Center : TextAlignmentOptions.Left;
            _lineText.color = isThought ? _thoughtColor : (isStage ? _stageColor : _normalColor);
            _dialogPanel.gameObject.SetActive(true);
            SetPanelFog(isThought);

            if (!isStage)
            {
                ShowAvatarFor(line.Speaker, line.Emotion);
            }
        }

        private void SetPanelFog(bool isFog)
        {
            _dialogPanel.color = isFog ? _panelThoughtFogColor : _panelNormalColor;
        }

        private void ShowAvatarFor(string speakerName, string emotionName)
        {
            var entry = FindCastEntry(speakerName);
            if (entry == null)
            {
                return;
            }

            var queue = entry.Side == DialogueSide.Left ? _leftAvatars : _rightAvatars;
            var visible = FindVisible(queue, entry.Character);
            if (visible != null)
            {
                // Говорящий отрисовывается поверх остальных на своей стороне.
                visible.Root.SetAsLastSibling();
                ApplyPortrait(visible, entry.Character, emotionName);
                return;
            }

            JoinSide(entry, queue, emotionName);
        }

        private CastEntry FindCastEntry(string speakerName)
        {
            if (_currentScene == null)
            {
                return null;
            }

            foreach (var entry in _currentScene.Cast)
            {
                if (entry.Character != null && entry.Character.DisplayName == speakerName)
                {
                    return entry;
                }
            }

            return null;
        }

        private static AvatarInstance FindVisible(Queue<AvatarInstance> queue, CharacterDefinition character)
        {
            foreach (var instance in queue)
            {
                if (instance.Character == character)
                {
                    return instance;
                }
            }

            return null;
        }

        private void JoinSide(CastEntry entry, Queue<AvatarInstance> queue, string emotionName)
        {
            int slot;
            if (queue.Count >= MaxAvatarsPerSide)
            {
                var evicted = queue.Dequeue();
                slot = evicted.SlotIndex;
                Destroy(evicted.Root.gameObject);
            }
            else
            {
                slot = FirstFreeSlot(queue);
            }

            queue.Enqueue(CreateAvatar(entry, slot, emotionName));
        }

        private int FirstFreeSlot(Queue<AvatarInstance> queue)
        {
            for (var slot = 0; slot < MaxAvatarsPerSide; slot++)
            {
                var taken = false;
                foreach (var instance in queue)
                {
                    if (instance.SlotIndex == slot)
                    {
                        taken = true;
                        break;
                    }
                }

                if (!taken)
                {
                    return slot;
                }
            }

            return 0;
        }

        private AvatarInstance CreateAvatar(CastEntry entry, int slot, string emotionName)
        {
            var root = Instantiate(_avatarTemplate, _avatarRoot);
            root.gameObject.SetActive(true);
            var anchors = entry.Side == DialogueSide.Left ? _leftSlotAnchors : _rightSlotAnchors;
            root.anchorMin = anchors[slot];
            root.anchorMax = anchors[slot];
            root.pivot = new Vector2(0.5f, 0.5f);
            root.anchoredPosition = Vector2.zero;

            var portrait = root.GetComponent<Image>();
            portrait.raycastTarget = false;
            var nameLabel = root.GetComponentInChildren<TMP_Text>();
            nameLabel.text = entry.Character.DisplayName;
            nameLabel.raycastTarget = false;

            // Зеркалим персонажа к центру: минус-масштаб по X, а табличке имени — встречный флип,
            // чтобы текст не отзеркалился.
            var flip = NeedsFlip(entry.Side);
            root.localScale = new Vector3(flip ? -1f : 1f, 1f, 1f);
            nameLabel.transform.localScale = new Vector3(flip ? -1f : 1f, 1f, 1f);

            var instance = new AvatarInstance
            {
                Character = entry.Character,
                SlotIndex = slot,
                Root = root,
                Portrait = portrait
            };

            ApplyPortrait(instance, entry.Character, emotionName);
            return instance;
        }

        // Персонажи должны «смотреть» к центру: зеркалим сторону, противоположную исходному развороту арта.
        private bool NeedsFlip(DialogueSide side)
        {
            switch (_sourceOrientation)
            {
                case SourceOrientation.FacesLeft:
                    return side == DialogueSide.Left;
                case SourceOrientation.FacesRight:
                    return side == DialogueSide.Right;
                default:
                    return false;
            }
        }

        private static void ApplyPortrait(AvatarInstance instance, CharacterDefinition character, string emotionName)
        {
            var sprite = character.GetSprite(emotionName);
            if (sprite == instance.CurrentSprite)
            {
                return;
            }

            instance.CurrentSprite = sprite;
            if (sprite != null)
            {
                instance.Portrait.sprite = sprite;
                instance.Portrait.color = Color.white;
                instance.Portrait.preserveAspect = true;
            }
            else
            {
                instance.Portrait.sprite = null;
                instance.Portrait.color = character.PlaceholderColor;
                instance.Portrait.preserveAspect = false;
            }
        }

        private void ClearAvatars()
        {
            DestroyQueue(_leftAvatars);
            DestroyQueue(_rightAvatars);
        }

        private static void DestroyQueue(Queue<AvatarInstance> queue)
        {
            while (queue.Count > 0)
            {
                Destroy(queue.Dequeue().Root.gameObject);
            }
        }

        private void ShowChoices(DialogueNode node)
        {
            _choiceContainer.gameObject.SetActive(true);
            for (var i = 0; i < node.Choices.Count; i++)
            {
                var choice = node.Choices[i];
                var button = Instantiate(_choiceButtonTemplate, _choiceContainer);
                button.gameObject.SetActive(true);
                button.GetComponentInChildren<TMP_Text>().text = choice.Label;
                var choiceIndex = i;
                button.onClick.AddListener(() => OnChoiceClicked(choiceIndex));
            }
        }

        private void OnChoiceClicked(int choiceIndex)
        {
            _session.Choose(choiceIndex);
            ShowCurrentNode();
        }

        private void ClearChoices()
        {
            for (var i = _choiceContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(_choiceContainer.GetChild(i).gameObject);
            }
        }

        private sealed class AvatarInstance
        {
            public CharacterDefinition Character;
            public int SlotIndex;
            public RectTransform Root;
            public Image Portrait;
            public Sprite CurrentSprite;
        }
    }
}