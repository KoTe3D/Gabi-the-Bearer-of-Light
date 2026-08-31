using System.Collections.Generic;
using UnityEngine;

namespace Gabi.Dialogue
{
    // Упорядоченная цепочка диалоговых сцен: контент добавляется в Inspector, код не трогаем.
    [CreateAssetMenu(fileName = "NewStoryFlow", menuName = "Gabi/Story Flow")]
    public sealed class StoryFlow : ScriptableObject
    {
        [SerializeField] private List<DialogueScene> _scenes = new List<DialogueScene>();

        public IReadOnlyList<DialogueScene> Scenes => _scenes;
    }
}