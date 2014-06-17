using System;
namespace Visao.Controle {
    public interface IServicoDeControle : IDisposable {

        void Conecte(string endereco);
        void InicieMotor();
        void PareMotor();
        void AcioneValvulaDescarte();
        void ProcesseImagem();
    }
}