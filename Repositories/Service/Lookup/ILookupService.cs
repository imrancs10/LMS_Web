using LearningManagementSystem.ViewModels.Response;

namespace LearningManagementSystem.Repositories.Service.Lookup;

public interface ILookupService
{
    public List<Models.Lookup> GetLookupDetailByType(string LookupType);
    public List<Models.Lookup> GetAllLookup();
    public Models.UserDetail CheckUserDetail(string username, string password);
    public List<Models.UserDetail> TotalUserDetail();
    public Models.StudentCredential CheckStudentDetail(string username, string password);
}