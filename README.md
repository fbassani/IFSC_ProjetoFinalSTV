IFSC_ProjetoFinalSTV
====================

Projeto final da disciplina de Sistemas de Visão do mestrado em mecatrônica do IFSC. Recebe a notificação de um sensor, captura uma imagem de um produto, avalia as cores e aciona e envia uma mensagem de descarte para os produtos que devem ser removidos da linha de produção.

http://instagram.com/p/o60HdhmFJ4/

** Reconhecimento **

Para este projeto, o reconhecimento funciona da seguinte maneira:
* Capturamos a imagem (após mensagem de que o produto passou na frente do sensor)
* Cortamos a imagem para utilizarmos apenas a parte com as diferenças relevantes
* Aplicamos algumas transformações para uniformizar as cores e diminuir o reflexo das luzes
* Separamos os canais da imagem
* Em cada canal, e pixel a pixel, comparamos os valores com os valores configurados para a tolerância . Se o valor do pixel está dentro do configurado, ele é alterado para 255. Se não estiver, é alterado para zero. Desta forma, temos imagens em preto e branco de cada canal
* Fazemos um `AND` dos canais
* Contamos os pontos cujo valor é 255.

Desta forma, conseguimos descobrir se a caixa na frente da câmera é a que deve ser descartada.

** Acionamento de motor e válvula pneumática **

O controle é feito via porta serial. As mensagens são trocadas com um Arudino UNO. O circuito para os acionamentos é personalizado.
