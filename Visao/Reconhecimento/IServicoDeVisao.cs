using System;
namespace Visao.Reconhecimento {
    public interface IServicoDeVisao : IDisposable {
        event Action<bool> ImagemProcessada;
        void ProcesseImagem();
    }
}