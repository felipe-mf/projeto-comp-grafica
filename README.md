# 🚀 Space Shooter 3D - Projeto de Computação Gráfica

> Jogo 3D de combate espacial desenvolvido como trabalho da disciplina de **Computação Gráfica e Realidade Virtual**

---

## 📋 Sobre o Projeto

Este projeto consiste em um jogo de tiro espacial em 3D desenvolvido na **Unity Engine**, onde o jogador controla uma nave espacial com o objetivo de destruir naves inimigas enquanto desvia de asteroides. O jogo implementa conceitos fundamentais de computação gráfica 3D, física de jogos, sistemas de partículas e interfaces de usuário.

---

## 📦 Download e Gameplay

### 🎬 Vídeo de Gameplay e Executável

Acesse a pasta do projeto no Google Drive para:
- **Assistir ao vídeo de gameplay completo**
- **Baixar o executável do jogo (Build)**

🔗 **Link:** https://drive.google.com/drive/folders/1E4Sdre6yFEwWi_EJdZO97wUg1mYB7DFS?usp=drive_link

### 📥 Como Jogar

1. Acesse o link acima
2. Baixe a pasta **Build** completa
3. Extraia todos os arquivos
4. Execute o arquivo `projeto-comp-grafica.exe`
5. Divirta-se! 🚀

---

## 🎮 Gameplay

### Objetivo
Destruir todas as naves inimigas sem perder toda a vida da nave. Ao completar a missão, um portal aparece no mapa para finalizar a fase com vitória.

### Controles
- **W/S** - Movimento vertical
- **A/D** - Movimento horizontal
- **Q/E** - Rotação lateral
- **Shift** - Acelerar nave
- **Shift direito** - Desacelerar nave
- **Espaço** - Atirar

### Níveis de Dificuldade

#### Fácil
- 5 naves inimigas
- Inimigos não atiram
- Velocidade reduzida dos inimigos
- Barreiras laser em ciano

#### Difícil
- 8 naves inimigas
- Inimigos atiram no jogador
- Maior vida e velocidade dos inimigos
- Barreiras laser em vermelho

---

## 🎯 Funcionalidades Implementadas

### Sistema de Jogo
- ✅ Menu principal com seleção de dificuldade (Fácil/Difícil)
- ✅ Sistema de vida do jogador (2 colisões = Game Over)
- ✅ Sistema de vitória com portal que aparece ao destruir todos os inimigos
- ✅ Tela de Game Over com opção de reiniciar
- ✅ Tela de vitória ao completar a missão
- ✅ Timer de 7 minutos por partida
- ✅ Sistema de mira customizável

### Mecânicas de Combate
- ✅ Sistema de tiro do jogador
- ✅ Naves inimigas com movimento aleatório
- ✅ Sistema de colisão e detecção de danos
- ✅ Asteroides destrutíveis com física realista

### Sistemas Visuais
- ✅ Efeitos de explosão usando sistemas de partículas
- ✅ Portal 3D com efeitos visuais e rotação
- ✅ Barreiras laser animadas que mudam de cor por dificuldade
- ✅ Materiais emissivos para efeitos
- ✅ UI responsiva com feedback visual ao tomar dano

### Física e Movimento
- ✅ Física realista usando Rigidbody da Unity
- ✅ Sistema de layers para gerenciamento de colisões
- ✅ Cooldown de dano para prevenir colisões múltiplas
- ✅ Movimento automático da nave com controle de aceleração

---

## 🐛 Resolução de Possíveis Problemas

### Problema 1: Asteroides Não Aparecem

**Sintoma:** Os asteroides estão invisíveis ou não carregam na cena.

**Solução:**
1. Abra a Unity
2. Vá em **Window > Asset Store** (ou pressione `Ctrl+9`)
3. Faça login na sua conta Unity
4. Acesse a aba **"My Assets"**
5. Procure por **"Breakable Asteroids"**
6. Na lista, clique no asset e selecione **"In Project"**
7. Clique em **"Download"** e depois em **"Import"**
8. Certifique-se de importar todos os arquivos

### Problema 2: Texturas Rosas/Magenta

**Sintoma:** Os asteroides ou outros objetos aparecem com cor rosa/magenta brilhante.

**Causa:** Incompatibilidade de shaders com o Universal Render Pipeline (URP).

**Solução:**
1. No menu superior da Unity: **Window > Rendering > Render Pipeline Converter**
2. Na janela que abrir, selecione **"Built-in to URP"**
3. Marque **TODAS** as opções disponíveis:
   - ☑️ Rendering Settings
   - ☑️ Material Upgrade
   - ☑️ Readonly Material Converter
   - ☑️ Animation Clip Converter
4. Clique no botão **"Initialize Converters"** (parte inferior)
5. Aguarde alguns segundos
6. Clique no botão **"Convert Assets"**
7. Aguarde a conversão completar
8. As texturas devem voltar ao normal


---

## 👥 Equipe de Desenvolvimento

- **[Jean Bonadeo Dal Santo]**
- **[Felipe Marostega Fagundes]**
- **[João Otávio Quevedo]**

---

**⭐ Obrigado por jogar o nosso Space Shooter! ⭐**

---
