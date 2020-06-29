using System.Collections.Generic;
using FruityNET.DTOs;

namespace FruityNET.Models
{
    public class AdminRequestsViewModel
    {
        public List<AdminRequestDTO> AdminRequests;
        public AdminRequestsViewModel()
        {
            AdminRequests = new List<AdminRequestDTO>();
        }
    }
}