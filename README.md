# Lights of Providence

Protótipo de jogo de terror em 1ª pessoa desenvolvido em Unity. O foco inicial é estabelecer a base de **movimentação do jogador**, **imersão de câmera**, e **sistemas de agachar, sprint com stamina e ruído**.

## Estado atual (protótipo)
- **Movimento (WASD)** com `CharacterController`
- **Look** (mouse) com limite de pitch
- **Crouch** (Ctrl/C) com **câmera que abaixa suavemente**
- **Sprint** (Shift) com **stamina** (drena/regen e bloqueio quando zera)
- **Nível de ruído** exposto (0..1) e **NoiseRadius**
- **Playground** simples (chão + paredes + luz) para teste
- **UI**: barra de stamina (Slider, opcional)

## Requisitos
- **Unity 2022.x ou 2023.x** (URP opcional)
- **Input System (novo)** habilitado
- Windows/macOS

## Como abrir
1. Clone o repositório ou baixe o ZIP
2. **Unity Hub → Open** e selecione a pasta do projeto
3. Abra a cena de teste (ex.: `SampleScene`) e pressione **Play**

## Controles (padrão)
| Ação      | Tecla                 |
|----------|-----------------------|
| Andar    | **W / A / S / D**     |
| Olhar    | **Mouse**             |
| Agachar  | **Left Ctrl** ou **C**|
| Sprint   | **Left Shift**        |
| Interagir| **E**                 |

> O projeto usa **Input System** com `PlayerInput (Send Messages)` e o asset `PlayerControls.inputactions`.
> Actions: `Move`, `Look`, `Crouch`, `Sprint`, `Interact`.


## PlayerController
- Controle de velocidades: `walkSpeed`, `crouchSpeed`, `sprintSpeed`
- Sistema de stamina: `maxStamina`, `drainSprintPerSec`, `regenIdlePerSec`, `regenWalkPerSec`, `regenCrouchPerSec`, `minStaminaToStartSprint`
- Controle da câmera: `cameraPivot`, `cameraStandY`, `cameraCrouchY`, `crouchTransitionSpeed`
- Sistema de ruído: `noiseWalk`, `noiseCrouch`, `noiseSprint`, `maxNoiseRadius`, `CurrentNoise`, `NoiseRadius`

## Próximos passos
- Implementar IA dos inimigos com visão e audição baseadas em NoiseRadius.
- Adicionar interações (portas, chaves, objetos colecionáveis).
- Criar o terreno e aprimorar iluminação, adicionar arvores, pedras e afins.
- Desenvolver HUD com barra de vida e slots de itens, e adicionar funcionalidade as barras.
- Implementar menus (Pause e Opções).

## Fluxo de trabalho (commits)
1. Criar uma branch para cada nova feature ou correção, por exemplo: `feat/ia-basica` ou `fix/camera-ajuste`.
2. Fazer commits pequenos e descritivos.
3. Abrir Pull Requests para a branch principal (`main`), com descrição detalhada das alterações.

## Licença
A definir pela equipe ou conforme orientação do professor.

