using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface IAuthRepository
    {
        User? GetUserByEmail(string email);
        User CreateUser(User user);
    }
}
