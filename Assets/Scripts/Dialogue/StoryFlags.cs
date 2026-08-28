using System.Collections.Generic;

namespace Gabi.Dialogue
{
    public sealed class StoryFlags
    {
        private readonly HashSet<string> _flags = new HashSet<string>();

        public void Set(string flag)
        {
            if (string.IsNullOrEmpty(flag))
            {
                return;
            }

            _flags.Add(flag);
        }

        public bool Has(string flag)
        {
            return _flags.Contains(flag);
        }
    }
}
