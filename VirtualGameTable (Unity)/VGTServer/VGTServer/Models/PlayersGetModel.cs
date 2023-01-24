using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGTDataStore.Core.Models.Enums;

namespace VGTServer.Models
{
    public class PlayersGetModel
    {
        public Dictionary<Guid, UserRolesForPoker> UsersInGame { get; set; }
    }
}
