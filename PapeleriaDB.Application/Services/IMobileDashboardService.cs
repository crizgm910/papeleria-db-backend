using PapeleriaDB.Application.DTOs;

namespace PapeleriaDB.Application.Services;

public interface IMobileDashboardService
{
    Task<MobileDashboardDto> GetAsync();
}
