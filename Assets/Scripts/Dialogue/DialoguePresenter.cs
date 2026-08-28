using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gabi.Dialogue
{
    public sealed class DialoguePresenter : MonoBehaviour
    {
        [SerializeField] private DialogueScene _scene;
        [SerializeField] private TextMeshProUGUI _speakerText;
        [SerializeField] private TextMeshProUGUI _lineText;
        [SerializeField] private Transform _choiceContainer;
        [SerializeField] private Button _choiceButtonTemplate;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _thoughtColor = new Color(0.75f, 0.75f, 0.75f);
        [SerializeField] private Color _stageColor = new Color(0.9f, 0.85f, 0.6f);

        private DialogueSession _session;
        private readonly StoryFlags _flags = new StoryFlags();

        private void Awake()
        {
            // Тексты не должны перехватывать клики: листание идёт кликом в любое место экрана.
            _speakerText.raycastTarget = false;
            _lineText.raycastTarget = false;
        }

        private void Start()
        {
            if (_scene != null)
            {
                Begin(_scene);
            }
        }

        private void Update()
        {
            if (_session == null || !_session.CanAdvance)
            {
                return;
            }

            var mouse = Mouse.current;
            var keyboard = Keyboard.current;
            var clickedEmptySpace = mouse != null && mouse.leftButton.wasPressedThisFrame && !IsPointerOverUi();
            var spacePressed = keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
            if (!clickedEmptySpace && !spacePressed)
            {
                return;
            }

            _session.Advance();
            ShowCurrentNode();
        }

        public void Begin(DialogueScene scene)
        {
            _session = new DialogueSession(scene, _flags);
            ShowCurrentNode();
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
                    ShowChoices(node);
                    break;
                default:
                    _speakerText.text = string.Empty;
                    _lineText.text = string.Empty;
                    _choiceContainer.gameObject.SetActive(false);
                    Debug.Log($"[Dialogue] Scene transition requested: {_session.RequestedNextScene}");
                    break;
            }
        }

        private void ApplyLine(DialogueLine line)
        {
            var isStage = line.Kind == DialogueLineKind.StageDirection;
            _speakerText.text = isStage ? string.Empty : line.Speaker;
            _lineText.text = line.Text;
            _lineText.fontStyle = line.Kind == DialogueLineKind.Spoken ? FontStyles.Normal : FontStyles.Italic;
            _lineText.alignment = isStage ? TextAlignmentOptions.Center : TextAlignmentOptions.Left;
            _lineText.color = line.Kind == DialogueLineKind.Thought ? _thoughtColor : (isStage ? _stageColor : _normalColor);
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
    }
}