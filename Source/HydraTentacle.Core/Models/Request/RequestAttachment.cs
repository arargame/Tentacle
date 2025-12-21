using System;
using Hydra.Core;
using Hydra.FileOperations;

namespace HydraTentacle.Core.Models.Request
{
    public class RequestAttachment : BaseObject<RequestAttachment>
    {
        public Guid RequestId { get; set; }
        public Request Request { get; set; } = null!;

        public Guid FileId { get; set; }
        public CustomFile File { get; set; } = null!;
    }
}
