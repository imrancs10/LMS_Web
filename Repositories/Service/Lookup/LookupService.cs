using LearningManagementSystem.Models;
using LearningManagementSystem.Repositories.Service.Lookup;
using LearningManagementSystem.ViewModels.Response;

namespace LearningManagementSystem.Repositories.Service.Lookup;

public class LookupService : ILookupService
{
    private readonly LMSContext _db;
    private readonly IConfiguration Configuration;

    public LookupService(LMSContext db, IConfiguration configuration)
    {
        _db = db;
        Configuration = configuration;
    }

    public List<Models.Lookup> GetLookupDetailByType(string LookupType)
    {
        return (from S in _db.Lookup
                where S.LookupType == LookupType && S.IsActive == true
                select S).ToList();
    }

    public List<Models.Lookup> GetAllLookup()
    {
        var lookupList = (from S in _db.Lookup
                          where S.IsActive == true
                          select S).ToList();
        return lookupList;
    }

    public Models.UserDetail CheckUserDetail(string username, string password)
    {
        var userdetail = (from S in _db.UserDetail
                          where S.IsActive == true && S.UserName == username && S.Password == password
                          select S).FirstOrDefault();
        return userdetail;
    }
    public List<Models.UserDetail> TotalUserDetail()
    {
        var userdetail = (from S in _db.UserDetail
                          where S.IsActive == true 
                          select S).ToList();
        return userdetail;
    }
}