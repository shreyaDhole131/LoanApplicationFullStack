using LoanApp_API.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface ISanctionLetter
    {
        Task<List<PendingSanctionDTO>> fetchCustomers();

        Task<SanctionPrefillDTO?> fetchSanctionDetails(int dealId);

        Task AddSanctionLetter(CreateSanctionDTO dto);

        Task<List<SanctionGridDTO>> fetchSanctionGrids();

        Task<List<SanctionGridDTO>> fetchSanctionsGrid(string? name, string? FromDate, string? ToDate, string? Filter, string? sort);
    }
}
