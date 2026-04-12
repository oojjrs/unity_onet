using System;
using System.Collections.Generic;
using System.Linq;

namespace oojjrs.onet
{
    public class MyNetAuthenticationException : MyNetRequestFailedException
    {
        /// <see cref="Unity.Services.Authentication.Notification"/>으로부터 복사
        public struct MyNotification
        {
            public string Id;

            public string CaseId;

            public string Message;

            public string PlayerId;

            public string ProjectId;

            public string Type;

            public string CreatedAt;
        }

        public List<MyNotification> Notifications { get; }

        internal MyNetAuthenticationException(int errorCode, string message, Exception innerException = null, List<Unity.Services.Authentication.Notification> notifications = null)
            : base(errorCode, message, innerException)
        {
            Notifications = notifications.Select(t => new MyNotification()
            {
                CaseId = t.CaseId,
                CreatedAt = t.CreatedAt,
                Id = t.Id,
                Message = t.Message,
                PlayerId = t.PlayerId,
                ProjectId = t.ProjectId,
                Type = t.Type,
            }).ToList();
        }
    }
}
