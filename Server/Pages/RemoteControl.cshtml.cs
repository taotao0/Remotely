using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using URemote.Server.Auth;
using URemote.Server.Services;
using URemote.Shared.Models;

namespace URemote.Server.Pages
{
    [ServiceFilter(typeof(RemoteControlFilterAttribute))]
    public class RemoteControlModel : PageModel
    {
        private readonly IDataService _dataService;
        public RemoteControlModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        public RemoteUser RemoteUser { get; private set; }
        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                RemoteUser = _dataService.GetUserByNameWithOrg(base.User.Identity.Name);
            }
        }
    }
}
