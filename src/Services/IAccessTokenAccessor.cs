namespace Clinician.Services
{
    using System.Threading.Tasks;

    public interface IAccessTokenAccessor
    {
        Task<string> GetAccessTokenAsync();
    }
}