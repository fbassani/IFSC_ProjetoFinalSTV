using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Visao.Reconhecimento {
    using Config = ConfiguracoesDoReconhecimentoPorCor;

    public class ReconhecimentoPorCor : IEstrategiaDeReconhecimento {
        private readonly CorTolerancia _r;
        private readonly CorTolerancia _g;
        private readonly CorTolerancia _b;

        public ReconhecimentoPorCor()
            : this(
                new CorTolerancia(Config.Default.RValor, Config.Default.RTolerancia),
                new CorTolerancia(Config.Default.GValor, Config.Default.GTolerancia),
                new CorTolerancia(Config.Default.BValor, Config.Default.BTolerancia)) { }


        public ReconhecimentoPorCor(CorTolerancia r, CorTolerancia g, CorTolerancia b) {
            _r = r;
            _g = g;
            _b = b;
        }

        public bool EstahOk(IplImage imagem) {
#if DEBUG
            var s = Stopwatch.StartNew();
#endif
            var imagemTransformada = TransformadorDeImagem.ApliqueTransformacoes(imagem);
            var encontrados = VerifiqueCor(imagemTransformada);
#if DEBUG
            Trace.WriteLine(String.Format("Tempo de processamento: {0} ms", s.ElapsedMilliseconds));
#endif
            return encontrados > 0;
        }

        private int VerifiqueCor(IplImage origem) {
            var canais = Enumerable.Repeat(Cv.CreateImage(origem.Size, BitDepth.U8, 1), 3).ToArray();
            Cv.Split(origem, canais[0], canais[1], canais[2], null);

            using (canais[0] = canais[0].Clone())
            using (canais[1] = canais[1].Clone())
            using (canais[2] = canais[2].Clone()) {
                var ajusteCanal1 = Task.Run(() => AjusteIntensidadeDeAcordoComTolerancia(canais[0], _b));
                var ajusteCanal2 = Task.Run(() => AjusteIntensidadeDeAcordoComTolerancia(canais[1], _g));
                var ajusteCanal3 = Task.Run(() => AjusteIntensidadeDeAcordoComTolerancia(canais[2], _r));

                Task.WaitAll(ajusteCanal1, ajusteCanal2, ajusteCanal3);

                using (var dst = Cv.CreateImage(origem.Size, BitDepth.U8, 1)) {
                    Cv.And(canais[0], canais[1], dst, null);
                    Cv.And(canais[2], dst, dst, null);
                    return NumeroDePontos(dst);
                }
            }
        }

        private static void AjusteIntensidadeDeAcordoComTolerancia(IplImage imagem, CorTolerancia corTolerancia) {
            var height = imagem.Height;
            var width = imagem.Width;
            var widthStep = imagem.WidthStep;

            unsafe {
                var ptr = (byte*)imagem.ImageData;
                for (var y = 0; y < height; y++) {
                    for (var x = 0; x < width; x++) {
                        var offset = (widthStep * y) + x;
                        var val = ptr[offset];
                        ptr[offset] = corTolerancia.NoExtremo(val);
                    }
                }
            }
        }

        private static int NumeroDePontos(IplImage imagem) {
            var height = imagem.Height;
            var width = imagem.Width;
            var widthStep = imagem.WidthStep;

            var pontosEncontrados = 0;
            unsafe {
                var ptr = (byte*)imagem.ImageData;
                for (var y = 0; y < height; y++) {
                    for (var x = 0; x < width; x++) {
                        var offset = (widthStep * y) + x;
                        var val = ptr[offset];
                        if (val == 0) {
                            continue;
                        }
                        pontosEncontrados++;
                    }
                }
            }
            return pontosEncontrados;
        }
    }
}