using BusTrack.BusTrack.DB.ClassesDB;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IUserRepositoryDB
    {
        // Método para obter todos os usuários
        List<UserDB> Get();

        // Método para obter um usuário pelo e-mail
        UserDB GetByEmail(string email);

        // Método para criar um novo usuário
        UserDB Create(UserDB user);

        // Método para obter um usuário pelo ID
        UserDB Read(string id);

        // Método para atualizar um usuário
        bool Update(string id, UserDB user);

        // Método para excluir um usuário
        bool Delete(string id);
    }
}
