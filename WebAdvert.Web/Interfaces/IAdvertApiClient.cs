using System.Threading.Tasks;

namespace WebAdvert.Web.Interfaces
{
    public interface IAdvertApiClient
    {
        Task<Models.CreateAdvertResponse> Create(Models.AdvertModel model);
    }
}
