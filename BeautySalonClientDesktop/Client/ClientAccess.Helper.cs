using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalonClientDesktop.Client
{
    public partial class ClientAccess
    {
        public int Id { get; set; }
        public async Task AddAuthorizeHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
