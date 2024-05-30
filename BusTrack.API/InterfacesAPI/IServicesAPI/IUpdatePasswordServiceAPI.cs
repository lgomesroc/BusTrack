using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IUpdatePasswordServiceAPI
    {

        void UpdatePassword(int userId, string newPassword);

    }
}