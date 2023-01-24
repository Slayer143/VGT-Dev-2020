using System;
using System.Collections.Generic;
using System.Text;

namespace VGTDataStore.Core.Models
{
    public class Result
    {
        public Guid UserId { get; set; }

        public byte Value { get; set; }

        public byte Combination { get; set; }

        public Result(Guid userId, byte value, byte combination)
        {
            UserId = userId;
            Value = value;
            Combination = combination;
        }
    }
}
