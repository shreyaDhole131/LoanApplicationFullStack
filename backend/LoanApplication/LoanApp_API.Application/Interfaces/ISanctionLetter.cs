using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface ISanctionLetter
    {

        Task<List<PendingSanctionDTO>> fetchCustomers();

        Task<SanctionPrefillDTO?> fetchSanctionDetails(int dealId);

        Task<SanctionLetters> AddSanctionLetter(CreateSanctionDTO dto);

        //Task<List<SanctionGridDTO>> fetchSanctionGrids();

        Task<List<SanctionGridDTO>> fetchSanctionsGrid(string? name, string? FromDate, string? ToDate, string? Filter, string? sort);

        Task<SanctionLetterDTO?> GetSanctionLetter(int letterId);

        byte[] GenerateSanctionLetterPdf(SanctionLetterDTO letter);
    }
}
