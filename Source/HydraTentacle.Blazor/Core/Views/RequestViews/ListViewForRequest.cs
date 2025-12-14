using HydraTentacle.Blazor.Core.Http;
using HydraTentacle.Blazor.Core.Views.Base;
using HydraTentacle.Core.DTOs;

namespace HydraTentacle.Blazor.Core.Views.RequestViews
{
    public class ListViewForRequest : ListViewBase<RequestDTO>
    {
        public ListViewForRequest(HttpClientService http)
            : base(http)
        {
        }

        public override string Endpoint => "api/Request/GetTestRequests"; // Adjust endpoint as needed to match TableDTO expectation
    }
}
