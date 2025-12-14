using Hydra.DTOs;
using HydraTentacle.Blazor.Core.Http;

namespace HydraTentacle.Blazor.Core.Views.Base
{
    public abstract class ListViewBase<T> : ViewBase
    {
        protected readonly HttpClientService _http;

        protected ListViewBase(HttpClientService http)
        {
            _http = http;
        }

        public TableDTO? Table { get; protected set; }

        public virtual async Task LoadAsync()
        {
            Table = await _http.GetAsync<TableDTO>(Endpoint);
        }
    }
}
