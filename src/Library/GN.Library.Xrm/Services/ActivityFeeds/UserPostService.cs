using GN.Library.Shared.Chats;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Xrm.Services.ActivityFeeds
{
    class UserPostService : IDisposable
    {
        private readonly UserSignedIn user;
        private readonly IServiceProvider serviceProvider;
        public UserSignedIn User => this.user;
        public UserPostService(UserSignedIn user, IServiceProvider serviceProvider)
        {
            this.user = user;
            this.serviceProvider = serviceProvider;
        }
        public async Task Start()
        {

        }

        public void Dispose()
        {

        }
    }
}
