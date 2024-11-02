using System.Threading.Tasks;
using System.Collections.Generic;
using VietnameseTextCorrectionServer.Models;

namespace VietnameseTextCorrectionServer.Interfaces
{
    public interface ITextProcessingService
    {
        /// <summary>
        /// Xử lý văn bản đầu vào và trả về danh sách các cụm từ kèm thông tin chính tả.
        /// </summary>
        /// <param name="inputText">Văn bản đầu vào cần kiểm tra.</param>
        /// <returns>Danh sách các cụm từ với trạng thái chính tả.</returns>
        Task<List<Word>> ProcessAsync(string inputText);
    }
}
