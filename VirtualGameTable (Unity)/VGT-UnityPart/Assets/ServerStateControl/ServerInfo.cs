using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ServerStateControl
{
    public class ServerInfo
    {
        public bool IsActive { get; set; }

        public ServerInfo()
        {
            IsActive = CheckState();
        }

        public bool CheckState()
        {
            try
            {
                var request = WebRequest.Create($"http://localhost:5000/api/state/");
                var response = request.GetResponse();

                string answer;

                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        answer = reader.ReadToEnd();
                    }
                }

                return answer == "Active";
            }
            catch (Exception)
            {
                return false;
            }
            
        }
    }
}
