using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTrack.BusTrack.API.DTOAPI
{
public class UpdatePasswordDTOAPI
{
    public int UserId { get; set; }
    public string? NewPassword { get; set; }
    public DateTime UpdateDate { get; set; } // Data de atualização da senha

}
}