using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VGTServer.Models
{
    public class GameResultsModel
    {
        public Guid UserId { get; set; }
    
        public Guid SessionId { get; set; }

        public int CardsValue { get; set; }

        public int Chips { get; set; }
    }
}
