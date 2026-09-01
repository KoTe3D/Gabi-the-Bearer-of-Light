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

    public enum DialogueSide
    {
        Left,
        Right
    }

    public enum LineSide
    {
        Inherit,
        Left,
        Right
    }

    [System.Serializable]
    public sealed class DialogueLine
    {
        public DialogueLineKind Kind = DialogueLineKind.Spoken;
        public string Speaker;
        public string Text;
        public string Emotion;
        public LineSide MoveSide = LineSide.Inherit;
    }

    [System.Serializable]
    public sealed class DialogueChoice
    {
        public string Label;
        public int NextNodeIndex;
        public List<string> FlagsToSet = new List<string>();
    }

    [System.Serializable]
    public sealed class CastEntry
    {
        public CharacterDefinition Character;
        public DialogueSide Side = DialogueSide.Right;
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