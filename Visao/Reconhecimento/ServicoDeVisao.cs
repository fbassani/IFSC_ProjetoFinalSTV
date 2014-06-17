using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using Visao.Properties;

namespace Visao.Reconhecimento {
    public class ServicoDeVisao : IServicoDeVisao {
        private readonly IEstrategiaDeReconhecimento _estrategiaDeReconhecimento;

        private readonly CvCapture _capture;
        private readonly CvWindow _window;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private IplImage _imagem;
        public event Action<bool> ImagemProcessada = delegate { };

        public ServicoDeVisao(IEstrategiaDeReconhecimento estrategiaDeReconhecimento) {
            _estrategiaDeReconhecimento = estrategiaDeReconhecimento;
            _capture = CvCapture.FromCamera(Settings.Default.Camera);
            _cancellationTokenSource = new CancellationTokenSource();
            var exibeSaidaDaCamera = Settings.Default.ExibeSaidaDaCamera;
            if (exibeSaidaDaCamera) {
                _window = new CvWindow("Saída da câmera", WindowMode.AutoSize);
            }
            Task.Run(() => {
                while (!_cancellationTokenSource.IsCancellationRequested) {
                    _imagem = _capture.QueryFrame();
                    var janela = _window;
                    if (janela != null && exibeSaidaDaCamera) {
                        janela.ShowImage(_imagem);
                    }
                }
            }, _cancellationTokenSource.Token);

        }
        
        public void ProcesseImagem() {
            Trace.WriteLine("Analisando imagem");
            var descartarSempre = Settings.Default.DescartarSempre;
#if DEBUG
            ExibaImagemCapturadaPorCincoSegundos();
#endif
            bool estahOk = _estrategiaDeReconhecimento.EstahOk(_imagem) && !descartarSempre;
            Trace.WriteLine(estahOk ? "OK" : "Descarte");
            ImagemProcessada(estahOk);
        }

        private async void ExibaImagemCapturadaPorCincoSegundos() {
            using (var w = new CvWindow("Imagem capturada", WindowMode.AutoSize)) {
                using (var clone = _imagem.Clone()) {
                    w.ShowImage(clone);
                    await Task.Delay(5000);
                }
            }
        }

        public void Dispose() {
            if (_capture != null) {
                _capture.Dispose();
            }
            if (_window != null) {
                _window.Dispose();
            }
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}