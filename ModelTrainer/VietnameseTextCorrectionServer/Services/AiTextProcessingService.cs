using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using VietnameseTextCorrectionServer.Interfaces;
using VietnameseTextCorrectionServer.Models;

namespace VietnameseTextCorrectionServer.Services
{
    public class AiTextProcessingService : ITextProcessingService
    {
        private static readonly string DictionaryPath = @"D:\Github\SpellCheck\ModelTrainer\Data\dictionary.txt"; // Cập nhật đường dẫn phù hợp
        private readonly TrieNode _root;
        private readonly HashSet<string> _phraseDictionary;

        public AiTextProcessingService()
        {
            // Kiểm tra sự tồn tại của tệp từ điển
            if (!File.Exists(DictionaryPath))
            {
                throw new FileNotFoundException($"Không tìm thấy tệp từ điển tại đường dẫn: {DictionaryPath}");
            }

            // Tạo Trie và HashSet chứa các cụm từ, không phân biệt chữ hoa/thường
            _root = new TrieNode();
            _phraseDictionary = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var phrases = File.ReadAllLines(DictionaryPath);
            foreach (var phrase in phrases)
            {
                var trimmedPhrase = phrase.Trim();
                if (!string.IsNullOrEmpty(trimmedPhrase))
                {
                    _phraseDictionary.Add(trimmedPhrase);
                    AddPhraseToTrie(trimmedPhrase.ToLowerInvariant());
                }
            }
        }

        private void AddPhraseToTrie(string phrase)
        {
            var node = _root;

            foreach (var ch in phrase)
            {
                if (!node.Children.TryGetValue(ch, out var child))
                {
                    child = new TrieNode();
                    node.Children[ch] = child;
                }
                node = child;
            }
            node.IsEndOfPhrase = true;
        }

        public Task<List<Word>> ProcessAsync(string inputText)
        {
            var wordsList = new List<Word>();

            if (string.IsNullOrWhiteSpace(inputText))
            {
                return Task.FromResult(wordsList);
            }

            // Loại bỏ khoảng trắng thừa ở đầu và cuối
            var trimmedInput = inputText.Trim();

            // Chuyển văn bản về chữ thường để so sánh không phân biệt chữ hoa/thường
            var textLower = trimmedInput.ToLowerInvariant();
            int index = 0;
            int length = trimmedInput.Length;
            var matches = new List<(int Start, int Length)>();

            // Tìm tất cả các cụm từ đúng trong văn bản
            while (index < length)
            {
                var matchLength = FindLongestMatch(textLower, index);
                if (matchLength > 0)
                {
                    matches.Add((index, matchLength));
                    index += matchLength;
                }
                else
                {
                    index++;
                }
            }

            // Đánh dấu các vị trí đã được tìm thấy
            var mask = new bool[length];
            foreach (var match in matches)
            {
                for (int i = match.Start; i < match.Start + match.Length; i++)
                {
                    mask[i] = true;
                }
            }

            // Phân tích đoạn văn bản thành các phần đúng và sai
            index = 0;
            while (index < length)
            {
                if (mask[index])
                {
                    // Tìm cụm từ đúng
                    int start = index;
                    while (index < length && mask[index])
                    {
                        index++;
                    }
                    int matchLength = index - start;
                    string matchedText = trimmedInput.Substring(start, matchLength);
                    wordsList.Add(new Word { Text = matchedText, IsCorrect = true });
                }
                else
                {
                    // Tìm cụm từ sai
                    int start = index;
                    while (index < length && !mask[index])
                    {
                        index++;
                    }
                    int errorLength = index - start;
                    string errorText = trimmedInput.Substring(start, errorLength);
                    wordsList.Add(new Word { Text = errorText, IsCorrect = false });
                }
            }

            return Task.FromResult(wordsList);
        }

        private int FindLongestMatch(string textLower, int startIndex)
        {
            var node = _root;
            int currentIndex = startIndex;
            int maxMatchLength = 0;

            while (currentIndex < textLower.Length)
            {
                char ch = textLower[currentIndex];
                if (!node.Children.TryGetValue(ch, out var child))
                {
                    break;
                }
                node = child;
                currentIndex++;

                if (node.IsEndOfPhrase)
                {
                    maxMatchLength = currentIndex - startIndex;
                }
            }

            return maxMatchLength;
        }
    }
}
