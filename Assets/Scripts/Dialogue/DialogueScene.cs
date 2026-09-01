using System.Collections.Generic;
using UnityEngine;

namespace Gabi.Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogueScene", menuName = "Gabi/Dialogue Scene")]
    public sealed class DialogueScene : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> _nodes = new List<DialogueNode>();
        [SerializeField] private List<CastEntry> _cast = new List<CastEntry>();
        [SerializeField] private Color _backgroundColor = Color.black;
        [SerializeField] private Sprite _background;

        public IReadOnlyList<DialogueNode> Nodes => _nodes;
        public IReadOnlyList<CastEntry> Cast => _cast;
        public Color BackgroundColor => _backgroundColor;
        public Sprite Background => _background;
    }
}