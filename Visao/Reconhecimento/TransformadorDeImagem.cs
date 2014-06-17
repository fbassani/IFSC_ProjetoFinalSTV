using System;
using OpenCvSharp;
using Visao.Properties;

namespace Visao.Reconhecimento {
    // todo: tirar magic numbers!
    public class TransformadorDeImagem {
        private static readonly Func<IplImage, IplImage>[] Funcoes = {
            Corte,
            Blur, 
            Erosao
        };

        private static IplImage Corte(IplImage img) {
            using (img) {
                var rect = new CvRect(Settings.Default.AreaDeCorte.X, Settings.Default.AreaDeCorte.Y, Settings.Default.AreaDeCorte.Width, Settings.Default.AreaDeCorte.Height);
                rect.X = Math.Min(img.Width - 1, rect.X);
                rect.Y = Math.Min(img.Height - 1, rect.Y);
                rect.Width = Math.Min(img.Width - rect.X, rect.Width);
                rect.Height = Math.Min(img.Height - rect.Y, rect.Height);

                var saida = Cv.CreateImage(rect.Size, img.Depth, img.NChannels);
                Cv.SetImageROI(img, rect);
                Cv.Copy(img, saida);
                return saida;
            }
        }

        private static IplImage Blur(IplImage img) {
            using (img) {
                var saida = Cv.CreateImage(img.Size, img.Depth, img.NChannels);
                Cv.Smooth(img, saida, SmoothType.Gaussian, 7, 9, 0, 0);
                return saida;
            }
        }

        private static IplImage Erosao(IplImage img) {
            using (img) {
                var saida = Cv.CreateImage(img.Size, img.Depth, img.NChannels);
                var elementoEstruturante = Cv.CreateStructuringElementEx(7, 7, 1, 1, ElementShape.Rect, null);
                Cv.Erode(img, saida, elementoEstruturante, 2);
                return saida;
            }
        }

        public static IplImage ApliqueTransformacoes(IplImage img) {
            IplImage result = img;
            for (int index = 0; index < Funcoes.Length; index++) {
                result = Funcoes[index](result);
            }
            return result;
        }
    }
}