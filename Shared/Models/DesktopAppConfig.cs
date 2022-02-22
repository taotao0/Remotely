using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URemote.Shared.Models
{
    public class DesktopAppConfig
    {
private string _host = "https://remote.cookmung.com";

        public string Host
        {
            get => _host.TrimEnd('/');
            set
            {
                _host = value?.TrimEnd('/');
            }
        }
        public string OrganizationId { get; set; } = "";
    }
}
