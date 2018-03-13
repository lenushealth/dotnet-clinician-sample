namespace Clinician.Services.Impl
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;

    public class AccessTokenAccessor : IAccessTokenAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccessTokenAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            return await this.httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        }
    }
}