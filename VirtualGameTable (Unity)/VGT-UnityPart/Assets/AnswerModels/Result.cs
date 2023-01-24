using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AnswerModels
{
    public class Result
    {
        public Guid UserId { get; set; }

        public byte Value { get; set; }

        public byte Combination { get; set; }
    }
}
