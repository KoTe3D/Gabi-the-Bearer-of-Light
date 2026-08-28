namespace Gabi.Dialogue
{
    // Plain C#: детерминированная логика диалога, тестируется без Unity.
    public sealed class DialogueSession
    {
        private readonly DialogueScene _scene;
        private readonly StoryFlags _flags;

        public DialogueSession(DialogueScene scene, StoryFlags flags)
        {
            _scene = scene;
            _flags = flags;
            GoTo(0);
        }

        public int CurrentNodeIndex { get; private set; }
        public bool IsFinished { get; private set; }
        public string RequestedNextScene { get; private set; }

        public DialogueNode CurrentNode => _scene.Nodes[CurrentNodeIndex];

        public bool CanAdvance => !IsFinished && CurrentNode.Kind == DialogueNodeKind.Line;

        public void Advance()
        {
            if (!CanAdvance)
            {
                return;
            }

            GoTo(CurrentNodeIndex + 1);
        }

        public void Choose(int choiceIndex)
        {
            if (IsFinished || CurrentNode.Kind != DialogueNodeKind.Choice)
            {
                return;
            }

            var choice = CurrentNode.Choices[choiceIndex];
            foreach (var flag in choice.FlagsToSet)
            {
                _flags.Set(flag);
            }

            GoTo(choice.NextNodeIndex);
        }

        private void GoTo(int nodeIndex)
        {
            CurrentNodeIndex = nodeIndex;
            var node = CurrentNode;
            if (node.Kind != DialogueNodeKind.End)
            {
                return;
            }

            IsFinished = true;
            RequestedNextScene = node.NextSceneName;
        }
    }
}