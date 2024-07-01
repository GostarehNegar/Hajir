using GN.Library.Odoo;
using GN.Portal.Crm.Data.Odoo.Internals;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hajir.AI.Bot.Internals
{
    internal class OdooContactStore : IContactStore
    {
        private readonly IOdooConnection connection;

        public OdooContactStore(IOdooConnection connection)
        {
            this.connection = connection;
        }

        public ContactData Create(ContactData contact)
        {
            if (contact.Id < 1)
            {
                var c = this.connection.NewContact();
                c.Name = contact.FirstName + " " + contact.LastName;
                c.TelegramUserId = contact.TelegramId.ToString();
                c.TelegramChatId = contact.ChatId.ToString();
                c.Save();
            }
            return contact;
        }


        public ContactData ToContactData(OdooContact contact)
        {
            return contact == null ? null : new ContactData
            {
                Id = contact.Id,
                FirstName = contact.Name,
                LastName = contact.Name,
                TelegramId = long.TryParse(contact.TelegramUserId, out var _r) ? _r : 0,
                ChatId = long.TryParse(contact.TelegramChatId, out var __r) ? __r : 0,
                Mobile = contact.MobilePhone,
            };
        }
        public ContactData GetByTelegramUserId(long id)
        {
            var result = this.connection
                .CreateQuery<OdooContact>()
                .Execute(q =>
                {
                    q.Filter.Equal(OdooContact.Schema.TelegramUserId, id.ToString());
                })
                .FirstOrDefault();


            return ToContactData(result);
        }

        public async Task UpdatePhoto(long userId , Func<Task<byte[]>> getPhoto)
        {
            var data = this.connection.CreateQuery<OdooContact>()
               .Execute(q =>
               {
                   q.AddField("image_1920")
                    .AddField("id")
                   .Filter.Equal(OdooContact.Schema.TelegramUserId, userId.ToString());
               })
               .FirstOrDefault();
            if (data.Photo == null)
            {
                var photo = await getPhoto();
                data.Photo = Convert.ToBase64String(photo);
                data.Save();


            }

        }
        public ContactData GetContactById(string id)
        {
            if (int.TryParse(id, out var _id))
            {
                var data = this.connection.CreateQuery<OdooContact>()
                .Execute(q =>
                {
                    q.AddField("image_1920");
                    q.Filter.Equal("id", _id);
                })
                .FirstOrDefault();
                var val = data.GetAttributeValue<string>("image_1920");
                byte[] _data = Convert.FromBase64String(val);
                File.WriteAllBytes("qq.jpeg", _data);


                return ToContactData(data);
            }

            return null;

        }

        public ContactData CreateOrUpdate(ContactData contact)
        {
            var c = this.connection.NewContact();
            c.Id = contact.Id > 0 ? contact.Id : -1;
            c.Name = contact.FirstName + " " + contact.LastName;
            c.TelegramUserId = contact.TelegramId.ToString();
            c.TelegramChatId = contact.ChatId.ToString();
            if (!string.IsNullOrWhiteSpace(contact.Mobile))
            {
                c.MobilePhone = contact.Mobile;
            }

            c.Save();

            return ToContactData(c);

        }
    }
}
