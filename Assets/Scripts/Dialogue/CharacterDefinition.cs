using UnityEngine;

namespace Gabi.Dialogue
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Gabi/Character")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Color _placeholderColor = Color.magenta;

        public string DisplayName => _displayName;
        public Color PlaceholderColor => _placeholderColor;
    }
}