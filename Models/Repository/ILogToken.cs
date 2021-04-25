using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shivyshine.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace Shivyshine.Models
{
    public interface ILogToken
    {
        //type: phone or email
        //void SetLogTokenAsync(Application user, string Type, string TargetValue, string TokenValue);
        bool SaveChanges();
        Task<IEnumerable<LogToken>> TokensAsync();
        Task<IEnumerable<LogToken>> TokensAsync(TokenFilter filter);
        LogToken GetCommandById(int id);
        void CreateCommand(LogToken cmd);
        void UpdateCommand(LogToken cmd);
        //void DeleteCommand(LogToken cmd);
        Task<RepositoryResult> GenerateLogTokenAsync(TokenType tokenType, string userId, string emailPhone, string token);
        Task<bool> IsLogTokenExpiredAsync(LogToken token);
        Task<RepositoryResult> ChangeTokenAsync(LogToken token);
        Task<LogToken> GetLogTokenAsync(LogToken token);
        Task<LogToken> GetLogTokenAsync(string userId, string emailPhone, string token);
        Task<int> IsLogTokenExpiredAsync(string userId, string emailPhone);
        Task<int> GetLogTokenIdAsync(LogToken token);

    }
}
