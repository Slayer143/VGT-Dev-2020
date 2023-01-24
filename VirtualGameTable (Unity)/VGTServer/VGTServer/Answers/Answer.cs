using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VGTServer.Answers
{
    public class Answer
    {
        public Guid UserId { get; set; }

        public string Result { get; set; }

        public Answer(Guid id, string res)
        {
            UserId = id;
            Result = res;
        }
    }
}
