using URemote.Shared.Enums;
using System.Runtime.Serialization;

namespace URemote.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class BaseDto
    {
        [DataMember(Name = "DtoType")]
        public virtual BaseDtoType DtoType { get; init; }
    }
}
