using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gabi.Dialogue
{
    [System.Serializable]
    public sealed class EmotionEntry
    {
        public string Name;
        public Sprite Sprite;
    }

    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Gabi/Character")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Color _placeholderColor = Color.magenta;
        [SerializeField] private List<EmotionEntry> _emotions = new List<EmotionEntry>();

        public string DisplayName => _displayName;
        public Color PlaceholderColor => _placeholderColor;

        // Спрайт по эмоции из реплики; пусто или не найдено — первая эмоция списка (дефолт).
        public Sprite GetSprite(string emotionName)
        {
            if (_emotions.Count == 0)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(emotionName))
            {
                foreach (var entry in _emotions)
                {
                    if (entry.Sprite != null && string.Equals(entry.Name, emotionName, StringComparison.OrdinalIgnoreCase))
                    {
                        return entry.Sprite;
                    }
                }

                Debug.LogWarning($"[Dialogue] Emotion '{emotionName}' not found for '{_displayName}', using default.");
            }

            return _emotions[0].Sprite;
        }
    }
}