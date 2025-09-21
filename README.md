# QuimicAR — Documentação e Manual do Usuário

## Visão geral

QuimicAR é um aplicativo móvel de realidade aumentada desenvolvido na **Unity 2021.3 LTS** com o **Vuforia Engine**. Abre a câmera do celular e reconhece cartões de papel contendo QR codes e o nome de elementos da tabela periódica. Ao detectar um cartão, o app posiciona um modelo 3D correspondente sobre o cartão no fluxo de câmera. Ao colocar cartões próximos, o app combina elementos e gera modelos 3D resultantes (por exemplo, H + H + O → H₂O).

---

## Recursos principais

* Reconhecimento em tempo real de cartões com QR code e rótulo do elemento.
* Renderização de modelos 3D posicionados e ancorados sobre o cartão físico.
* Combinação de múltiplos cartões para gerar moléculas compostas.
* PDF dos cartões para impressão, recorte e uso (instruções de impressão incluídas abaixo).
* Suporte a exportar logs de combinações para debug.

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
5. Toque na molécula 3D para ver informações (fórmula, massa molar, descrição).

---

## Arquitetura técnica (resumo)

* **Captura de câmera**: fornecida pelo Vuforia Engine.
* **Detecção de marcador**: Vuforia Image Targets associados a QR codes ou imagens dos cartões.
* **Ancoragem 3D**: Unity + Vuforia ARCamera. Cada Image Target referencia um modelo 3D específico.
* **Sistema de combinação**: lógica em scripts C# que valida proximidade de múltiplos Image Targets. Ao detectar combinação, instancia prefab da molécula.
* **Dados**: `elements.json` (lista de elementos com propriedades), `models/` (prefabs ou arquivos 3D), `combinations.json` (regras de combinação e modelos resultantes).

---

## Formato dos dados

**elements.json** (exemplo)

```json
{
  "H": {"name":"Hydrogen","symbol":"H","valence":1,"model":"Prefabs/H.prefab"},
  "O": {"name":"Oxygen","symbol":"O","valence":2,"model":"Prefabs/O.prefab"}
}
```

**combinations.json** (exemplo)

```json
{
  "H2O": {
    "inputs": ["H","H","O"],
    "model":"Prefabs/H2O.prefab",
    "displayName":"Água",
    "description":"Molécula de água"
  }
}
```

---

## PDF dos cartões — inclusão na documentação

Coloque o PDF dos cartões em `Assets/Resources/Cards/cards.pdf`. No README inclua um link de download relativo:

```markdown
[Baixe e imprima os cartões (PDF)](/Assets/Resources/Cards/cards.pdf)
```

### Instruções de impressão recomendadas

* Papel: A4 180-250 g/m² ou papel couché para maior rigidez.
* Impressora: jato de tinta ou laser. Ajuste para "sem margens" ou 0.
* Escala: 100% (sem ajuste "auto fit").
* Corte: use tesoura ou guilhotina. Se possível, plastifique para maior durabilidade.
* Tamanho final sugerido por cartão: 85 mm × 55 mm ou 90 mm × 60 mm.

---

## Permissões e privacidade

* O app solicita apenas permissão de câmera. Não armazena vídeo localmente por padrão.
* Se coletar logs, informe o usuário e permita a exclusão.

---

## Troubleshooting

* **Câmera não abre**: verifique permissões e se outro app está usando a câmera.
* **Cartão não reconhecido**: aumente iluminação, ajuste foco ou imprima cartões em melhor qualidade.
* **Combinação não ocorre**: confirme que `combinations.json` contém a regra e que os cartões estão suficientemente próximos.

---

## Licença

Inclua um `LICENSE` apropriado. Recomendado MIT para projetos educativos.

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

* `Assets/Resources/Cards/cards.pdf` — PDF para impressão. (Adicionar manualmente ao repositório se ainda não existir.)
* `Assets/Data/elements.json` — lista de elementos.
* `Assets/Data/combinations.json` — regras de combinação.
* Link para download do app no Google Drive (adicionar no README).
