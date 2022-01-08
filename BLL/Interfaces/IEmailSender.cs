using DAL.EF.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(User user);
    }
}
