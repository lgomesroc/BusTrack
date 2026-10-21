using BusTrack.BusTrack.DB.ClassesDB;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IInspectorRepositoryDB
    {
        List<InspectorDB> Get();

        InspectorDB GetByCPF(string cpf);

        InspectorDB Create(InspectorDB inspector);
    }
}
