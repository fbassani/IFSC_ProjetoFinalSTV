using OpenCvSharp;
namespace Visao.Reconhecimento {
    public interface IEstrategiaDeReconhecimento {
        bool EstahOk(IplImage imagem);
    }
}