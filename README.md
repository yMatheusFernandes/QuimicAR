# QuimicAR — Documentação e Manual do Usuário

## Visão geral

QuimicAR é um aplicativo móvel de realidade aumentada desenvolvido na **Unity 2021.3 LTS** com o **Vuforia Engine**. Abre a câmera do celular e reconhece cartões de papel contendo QR codes e o nome de elementos da tabela periódica. Ao detectar um cartão, o app posiciona um modelo 3D correspondente sobre o cartão no fluxo de câmera. Ao colocar cartões próximos, o app combina elementos e gera modelos 3D resultantes (por exemplo, H + H + O → H₂O).

---

## Recursos principais

* Reconhecimento em tempo real de cartões com QR code e rótulo do elemento.
* Renderização de modelos 3D posicionados e ancorados sobre o cartão físico.
* Combinação de múltiplos cartões para gerar moléculas compostas.
* PDF dos cartões para impressão, recorte e uso 

---

## Requisitos

* Unity **2021.3 LTS**.
* Vuforia Engine SDK (configurado no projeto Unity).
* Android 8.0+ / iOS 13+ com suporte AR.
* Permissão de acesso à câmera no dispositivo.

---

## Instalação e build

1. Clone o repositório:

```bash
git clone https://github.com/SEU_USUARIO/QuimicAR.git
cd QuimicAR
```

2. Abra o projeto na Unity **2021.3 LTS**.

3. Instale e configure o **Vuforia Engine** no Unity Hub (através do Package Manager ou importando o pacote oficial).

4. Configure a licença Vuforia em **Project Settings → Vuforia Configuration**.

5. Build para Android ou iOS:

   * Android: abra **File → Build Settings → Android → Build**.
   * iOS: abra **File → Build Settings → iOS → Build** e exporte para Xcode.

---

## Uso rápido (fluxo do usuário)

1. Abra o app e permita acesso à câmera.
2. Aponte a câmera para um cartão impresso com QR code e nome do elemento.
3. Aguarde o app localizar o marcador. O modelo 3D aparece ancorado sobre o cartão.
4. Para combinar elementos, aproxime os cartões na mesma cena. O motor de combinação verifica proximidade e gera a molécula correspondente.

---

## Arquitetura técnica (resumo)

* **Captura de câmera**: fornecida pelo Vuforia Engine.
* **Detecção de marcador**: Vuforia Image Targets associados a QR codes ou imagens dos cartões.
* **Ancoragem 3D**: Unity + Vuforia ARCamera. Cada Image Target referencia um modelo 3D específico.
* **Sistema de combinação**: lógica em scripts C# que valida proximidade de múltiplos Image Targets. Ao detectar combinação, instancia prefab da molécula.

---

## Permissões e privacidade

* O app solicita apenas permissão de câmera. Não armazena vídeo localmente por padrão.

---

## Troubleshooting

* **Câmera não abre**: verifique permissões e se outro app está usando a câmera.
* **Cartão não reconhecido**: aumente iluminação, ajuste foco ou imprima cartões em melhor qualidade

---

## FAQ rápido

**P: Funciona em todos os celulares?**
R: Depende do suporte à câmera e performance. Teste em dispositivos reais.

**P: Posso usar qualquer impressora para os cartões?**
R: Sim. Ajuste escala para 100% e prefira papel rígido.

**P: Onde baixo o aplicativo?**
R: O app estará disponível para download no **Google Drive**. O link ficará disponível no README e na documentação do repositório.

---

## Anexos e links no repositório

* Link para download do app no Google Drive
* https://drive.google.com/drive/folders/1nhJNqrLw7KT3n3kc3KL7P2oJjwSXrLM_?usp=sharing
