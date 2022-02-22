using System;

namespace URemote.Server.Models
{
    public class ModalButton
    {
        public string Class { get; set; }
        public string Text { get; set; }

        public Action OnClick { get; set; }
    }
}
