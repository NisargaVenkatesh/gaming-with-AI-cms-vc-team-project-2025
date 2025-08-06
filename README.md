# 🎮 Crimson Pact: AI-Powered 2D Survival Game

**Crimson Pact** is a 2D rogue-like survival game built using Unity and enhanced with a reinforcement learning agent trained via Unity ML-Agents.  
Inspired by _Vampire Survivors_, the game is a team project developed under CMS-VC and CMS-CLS modules at TU Dresden (2025).

---

## Project Objectives

- Build a dynamic game environment with enemies, hazards, and powerups.
- Train an AI agent using **Proximal Policy Optimization (PPO)**.
- Evaluate performance, learning stability, and gameplay behavior.

---

## Tech Stack

- Unity 6.1
- Unity ML-Agents Toolkit
- Python 3.10
- TensorBoard (for training visualization)

---

## Folder Structure

```bash
TEA TEST ML copy 6/
├── Assets/                     # Unity scenes, scripts, prefabs, UI
│   ├── MLAgents/              # Agent behaviors and settings
│   ├── Scenes/                # Game.unity
│   └── Scripts/               # CrimsonPact.cs, GameManager.cs, etc.
│
├── Packages/                  # Unity package dependencies
├── ProjectSettings/           # Unity build, tag/layer, and input settings
├── player_config.yaml         # PPO training configuration
├── results/                   # Training outputs from ML-Agents
│   └── run26/                 # Logs, YAML configs, metrics
├── README.md                  # Project overview (you’re reading it)
├── .gitignore                 # Ignore Unity cache, builds, venvs, etc.
```

---

## Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download) + Unity Editor 6.1 or later
- Python 3.10+
- ML-Agents Toolkit (`mlagents==0.30.0` or compatible)

---

### Setup Instructions

```bash
# 1. (Optional) Create a Python virtual environment
python -m venv mlagentvenv
source mlagentvenv/bin/activate  # or .\mlagentvenv\Scripts\activate on Windows

# 2. Install ML-Agents and required packages
pip install mlagents tensorboard matplotlib pandas
```

---

### 🧪 Training the Agent

```bash
mlagents-learn player_config.yaml --run-id=CrimsonRun1 --env=Game
```

> Training results will be saved in the `results/CrimsonRun1/` folder.

---

### Playing the Game

1. Open the project in Unity Hub
2. Load the `Main Menu` scene
3. Select one of the following modes:
   - **Manual** (you control the player)
   - **AI** (trained AI)
4. Press **Play** and observe how the AI behaves.

---

## Evaluation & Visualization

- Run training visualizations using TensorBoard:
  ```bash
  tensorboard --logdir=results/
  ```

---

## Team Members

| Name                    | Module      |
| ----------------------- | ----------- |
| Efe Ismet Yurteri       | CMS-CLS-TEA |
| Nisarga Madhu Venkatesh | CMS-VC-TEA  |
| Sayandip Srimani        | CMS-VC-TEA  |
| Vanshika Saini          | CMS-VC-TEA  |

> Developed at **TU Dresden**, 2025

---

## Links

- 🔗 GitHub Repo: [github.com/NisargaVenkatesh/gaming-with-AI-cms-vc-team-project-2025](https://github.com/NisargaVenkatesh/gaming-with-AI-cms-vc-team-project-2025)
- 🎮 WebGL Build: _Coming Soon on Itch.io_

---

## License

This project is for academic and demonstration purposes only.  
You may adapt it for personal or educational use with proper credit.

---

## Notes

- Game behaviors may vary depending on training progress and reward structure.
- The agent’s intelligence improves with longer training.
- All training parameters are configurable in `player_config.yaml`.

---

## Screenshots

> _(Optional: Add images in `/results/` folder and use Markdown like below)_

```markdown
![Main Menu](results/screenshots/main_menu.png)
![Gameplay](results/screenshots/gameplay1.png)
```
