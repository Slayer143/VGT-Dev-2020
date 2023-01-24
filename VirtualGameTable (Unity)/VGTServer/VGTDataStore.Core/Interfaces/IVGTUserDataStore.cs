using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core
{
    public interface IVGTUserDataStore
    {
        IDictionary<Guid, VGTUser> Users { get; }

        Guid AddUser(VGTUserRestricted user);

        Dictionary<Guid, VGTUser> GetUsers();

        void ChangeUserChips(Guid UserId, int Chips);
    }
}
