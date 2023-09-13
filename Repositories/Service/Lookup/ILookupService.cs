﻿using LearningManagementSystem.ViewModels.Response;

namespace LearningManagementSystem.Repositories.Service.Lookup;

public interface ILookupService
{
    public List<Models.Lookup> GetLookupDetailByType(string LookupType);
    public List<Models.Lookup> GetAllLookup();
}