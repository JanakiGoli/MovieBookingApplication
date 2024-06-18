using AccessIdentityAPI.Models;


namespace AccessIdentityAPI.Interfaces
{
    public interface ITokenService
    {
       Task<string> CreateToken(ApplicationUser appUser);
    }
}
