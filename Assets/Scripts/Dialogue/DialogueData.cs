using System.Collections.Generic;

namespace Gabi.Dialogue
{
    public enum DialogueNodeKind
    {
        Line,
        Choice,
        End
    }

    public enum DialogueLineKind
    {
        Spoken,
        Thought,
        StageDirection
    }

    [System.Serializable]
    public sealed class DialogueLine
    {
        public DialogueLineKind Kind = DialogueLineKind.Spoken;
        public string Speaker;
        public string Text;
    }

    [System.Serializable]
    public sealed class DialogueChoice
    {
        public string Label;
        public int NextNodeIndex;
        public List<string> FlagsToSet = new List<string>();
    }

    [System.Serializable]
    public sealed class DialogueNode
    {
        public DialogueNodeKind Kind = DialogueNodeKind.Line;
        public DialogueLine Line = new DialogueLine();
        public List<DialogueChoice> Choices = new List<DialogueChoice>();
        public string NextSceneName;
    }
}
