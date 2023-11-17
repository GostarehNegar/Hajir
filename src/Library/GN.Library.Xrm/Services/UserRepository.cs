using GN.Library.Messaging;
using GN.Library.Messaging.Internals;
using GN.Library.Xrm.StdSolution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static GN.Library.LibraryConstants;
using System.Linq;
using GN.Library.Shared.Entities;
using GN.Library.Shared.Internals;
using GN.Library.Identity;
using GN.Library.Shared.Identity;

namespace GN.Library.Xrm.Services
{

    class UserRepository : IMessageHandlerConfigurator, IUserPrimitiveRepository, IMessageHandler<QueryUserRequest>
    {
        private readonly IXrmDataServices dataServices;

        public UserRepository(IXrmDataServices dataServices)
        {
            this.dataServices = dataServices;
        }

        public void Configure(ISubscriptionBuilder subscription)
        {
            subscription.UseTopic(Subjects.IdentityServices.LoadUsers)
                .UseHandler(this.Handle);
        }

        public Task<UserEntity> GetUserByExtension(string extension)
        {
            throw new NotImplementedException();
        }

        public async Task<UserEntity> GetUserById(string id)
        {
            await Task.CompletedTask;
            return Guid.TryParse(id, out var _id)
                ? this.dataServices
                    .GetRepository<XrmSystemUser>()
                    .Queryable
                    .FirstOrDefault(x => x.SystemUserId == _id)?
                    .ToDynamic()
                    .To<UserEntity>()
                : null;
        }

        public async Task<UserEntity> GetUserByUserId(string userName)
        {
            await Task.CompletedTask;
            var _user = LibraryConventions.Instance.NormalizeUserName(userName).Item1;
            foreach (var _domian in LibraryConventions.Instance.NormalizeUserName(userName).Item2.Split('.'))
            {
                var user = this.dataServices
                   .GetRepository<XrmSystemUser>()
                   .Queryable
                   .FirstOrDefault(x => x.DomainName == $"{_domian}\\{_user}");
                if (user != null)
                {
                    return user.ToDynamic()
                        .To<UserEntity>();
                }
            }
            return null;
        }

        public async Task Handle(IMessageContext context)
        {
            await Task.CompletedTask;
            try
            {
                var command = context.Message.GetBody<LoadUsersCommand>(true) ?? new LoadUsersCommand();
                var result = new LoadUsersReply
                {

                };
                Console.WriteLine($"Skip:{command.Skip}, Take:{command.Take}");
                result.Users = await LoadUsers(command.Skip, command.Take);
                //result.Users = this.dataServices
                //    .GetRepository<XrmSystemUser>()
                //    .Queryable
                //    .ToArray()
                //    .Skip(command.Skip)
                //    .Take(command.Take)
                //    .Select(x => x.ToDynamic())
                //    .Select(x => x.To<UserEntity>())
                //    .ToArray();
                await context.Reply(result);
            }
            catch (Exception err)
            {
                await context.Reply(err);
            }
        }

        [MessageHandler(Topic =LibraryConstants.Subjects.IdentityServices.QueryUser)]
        public async Task Handle(IMessageContext<QueryUserRequest> context)
        {
            try
            {
                var message = context.Message.Body;
                bool NoResponse = false;
                UserEntity result = null;
                if (1==0 && !string.IsNullOrWhiteSpace(message.TelephoneExtension))
                {

                }
                else
                {
                    NoResponse = true;
                }
                if (!NoResponse)
                {
                    await context.Reply(new QueryUserResponse { User = result });
                }


            }
            catch (Exception err)
            {
                await context.Reply(err);
            }
            
        }

        public async Task<UserEntity[]> LoadUsers(int skip, int take)
        {
            await Task.CompletedTask;
            return this.dataServices
                    .GetRepository<XrmSystemUser>()
                    .Queryable
                    .ToArray()
                    .Skip(skip)
                    .Take(take)
                    .Select(x => x.ToDynamic().To<UserEntity>())
                    //.Select(x=> new UserEntity
                    //{
                    //    Id = x.SystemUserId.ToString(),
                    //    DomainName = x.DomainName,
                    //    FullName = x.FullName,
                    //})
                    .ToArray();
        }
    }
}
