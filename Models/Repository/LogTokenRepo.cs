using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shivyshine.Data;
using Shivyshine.Models;

namespace Shivyshine.Models
{
    public class LogTokenRepo : ILogToken
    {
        private readonly ApplicationDbContext db;

        public LogTokenRepo(ApplicationDbContext context)
        {
            db = context;
        }

        public void CreateCommand(LogToken cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            db.LogTokens.Add(cmd);
        }

        public void DeleteCommand(LogToken cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            db.LogTokens.Remove(cmd);
        }

        public async Task<IEnumerable<LogToken>> TokensAsync()
        {
            return await db.LogTokens.ToListAsync();
        }

        public async Task<IEnumerable<LogToken>> TokensAsync(TokenFilter filter)
        {
            IEnumerable<LogToken> logTokens = null;
            switch (filter)
            {
                case TokenFilter.ID:
                    logTokens = await db.LogTokens.Where(p => p.Id == (int)filter).ToListAsync();
                    break;
                case TokenFilter.USERID:
                    logTokens = await db.LogTokens.Where(p => p.UserId == filter.ToString()).ToListAsync();
                    break;
                case TokenFilter.TYPE:
                    logTokens = await db.LogTokens.Where(p => p.Type == filter.ToString()).ToListAsync();
                    break;
                case TokenFilter.EMAILPHONE:
                    logTokens = await db.LogTokens.Where(p => p.EMAILPHONE == filter.ToString()).ToListAsync();
                    break;
                case TokenFilter.TOKEN:
                    logTokens = await db.LogTokens.Where(p => p.Token == filter.ToString()).ToListAsync();
                    break;
                case TokenFilter.ISEXPIRED:
                    logTokens = await db.LogTokens.Where(p => p.IsExpired == false).ToListAsync();
                    break;
                case TokenFilter.EXPIREDON:
                    logTokens = await db.LogTokens.OrderByDescending(p => p.ExpiredOn).ToListAsync();
                    break;
                case TokenFilter.TIMESTAMP:
                    logTokens = await db.LogTokens.OrderByDescending(p => p.TimeStamp).ToListAsync();
                    break;
            }

            return logTokens;
        }

        public LogToken GetCommandById(int id)
        {
            return db.LogTokens.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (db.SaveChanges() >= 0);
        }


        public async Task<RepositoryResult> GenerateLogTokenAsync(TokenType tokenType, string userId, string emailPhone, string token)
        {
            if (userId == null || emailPhone == null || token == null)
            {
                return RepositoryResult.Failed;
            }

            string Type = "";

            switch (tokenType)
            {
                case TokenType.PHONE:
                    Type = TokenType.PHONE.ToString();
                    break;
                case TokenType.EMAIL:
                    Type = TokenType.EMAIL.ToString();
                    break;
            }

            LogToken cmd = new LogToken
            {
                UserId = userId,
                Type = Type,
                EMAILPHONE = emailPhone,
                Token = token,
                IsExpired = false,
                TimeStamp = DateTime.Now
            };

            await db.LogTokens.AddAsync(cmd);
            await db.SaveChangesAsync();

            return RepositoryResult.Succeeded;
        }

        public void UpdateCommand(LogToken cmd)
        {
            //Nothing
        }

        public async Task<RepositoryResult> ChangeTokenAsync(LogToken token)
        {
            if (token != null)
            {
                token.IsExpired = true;
                token.ExpiredOn = DateTime.Now;

                await db.SaveChangesAsync();
                return RepositoryResult.Succeeded;
            }

            return RepositoryResult.Failed;
        }

        public async Task<int> GetLogTokenIdAsync(LogToken token)
        {
            return (await db.LogTokens.FirstAsync(p => p.Id == token.Id)).Id;
        }

        public async Task<LogToken> GetLogTokenAsync(LogToken token)
        {
            return await db.LogTokens.FindAsync(token.Id);
        }

        public async Task<LogToken> GetLogTokenAsync(string userId, string emailPhone, string token)
        {
            return await db.LogTokens.FirstOrDefaultAsync(p => p.UserId == userId && p.EMAILPHONE == emailPhone && p.Token == token);
        }

        public async Task<bool> IsLogTokenExpiredAsync(LogToken token)
        {
            return (await db.LogTokens.FirstOrDefaultAsync(p => p.Id == token.Id)).IsExpired;
        }

        public async Task<int> IsLogTokenExpiredAsync(string userId, string emailPhone)
        {
            if (db.LogTokens.Any(p => p.UserId == userId && p.EMAILPHONE == emailPhone && !p.IsExpired))
                return (await db.LogTokens.FirstAsync(p => p.UserId == userId && p.EMAILPHONE == emailPhone && !p.IsExpired)).Id;
            else
                return 0;
        }
    }
}