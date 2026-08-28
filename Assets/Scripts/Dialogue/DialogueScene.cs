using System.Collections.Generic;
using UnityEngine;

namespace Gabi.Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogueScene", menuName = "Gabi/Dialogue Scene")]
    public sealed class DialogueScene : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> _nodes = new List<DialogueNode>();

        public IReadOnlyList<DialogueNode> Nodes => _nodes;
    }
}