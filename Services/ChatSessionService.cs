 
        using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GraduationProject.Services
    {
        public interface IChatSessionService
        {
            bool IsNewSession(int userId);
            void MarkSessionActive(int userId);
            void EndSession(int userId);
        }

        public class ChatSessionService : IChatSessionService
        {
           
            private readonly ConcurrentDictionary<int, DateTime> _activeSessions = new ConcurrentDictionary<int, DateTime>();

            private const int SESSION_TIMEOUT_MINUTES = 30;

            public bool IsNewSession(int userId)
            {
               
                if (!_activeSessions.TryGetValue(userId, out DateTime lastActivity))
                {
                    return true;
                }

                if ((DateTime.UtcNow - lastActivity).TotalMinutes > SESSION_TIMEOUT_MINUTES)
                {
                    return true;
                }

                return false;
            }

            public void MarkSessionActive(int userId)
            {
                _activeSessions.AddOrUpdate(userId, DateTime.UtcNow, (_, __) => DateTime.UtcNow);
            }

            public void EndSession(int userId)
            {
                _activeSessions.TryRemove(userId, out _);
            }
        }
    }


