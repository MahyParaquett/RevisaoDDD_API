using Entities.Entities;

namespace Domain.Interfaces.InterfaceServices
{
    public interface IServiceMessage
    {
        Task Adicionar(Message Objeto);

        Task Atualizar(Message Objeto);
        Task<List<Message>> ListarMenssageAtivas();
    }
}
