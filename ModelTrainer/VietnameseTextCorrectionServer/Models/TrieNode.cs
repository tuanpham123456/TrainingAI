using System.Collections.Generic;

namespace VietnameseTextCorrectionServer.Models
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>();
        public bool IsEndOfPhrase { get; set; }
    }
}
