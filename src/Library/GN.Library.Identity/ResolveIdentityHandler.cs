using GN.Library.Messaging;
using GN.Library.Messaging.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GN.Library.Shared.Internals;

namespace GN.Library.Identity
{
    public class ResolveIdentityHandler : IMessageHandler<ResolveIdentityCommand>
    {
        private readonly IUserIdentityProvider provider;

        public ResolveIdentityHandler(IUserIdentityProvider provider)
        {
            this.provider = provider;
        }

        public async Task Handle(IMessageContext<ResolveIdentityCommand> context)
        {
            var body = context?.Message?.Body;
            try
            {
                if (body != null)
                {

                    var user = await this.provider.LoadUser(body.User.GetAttributeValue("domainname"));
                    await context.Reply(new ResolveIdentityReply
                    {
                        User = body.User,
                        UserId = user?.GetClaimsIdentity()?.Name
                    });

                }
                await Task.CompletedTask;
            }
            catch (Exception err)
            {
                _ = context.Reply(err);
            }
        }

        
    }
}
