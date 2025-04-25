# 🎯 Archery VR / Bow Masters

**Disclaimer**
Due to file size constraints, this repo only shows the main scripts used for the game.

## What is this?
Archery VR, also known as Bow Masters, is a virtual reality archery game designed with realism, challenge, and immersion at its core. Built with Unity and powered by custom physics tuned for arrow realism, this project explores how dynamic difficulty and VR interaction can create a deep and replayable experience.

## Features
* Interactive VR Menu: Choose game modes, difficulties, and maps using intuitive in-world buttons.

* Scene Transitions: Seamless animated transitions between maps for immersive flow.

* Custom Bow & Arrow Physics: Gravity, drag, and real-time wind affect your shots.

* Wind System: Wind scales off difficulty, and changes every few turns to keep things fresh.

* Difficulty Scaling: AI scales off difficulty, and adapts to the player’s skill level. If you’re struggling, it eases up. If you’re excelling, it gives you a true challenge.

* Points Board: Tracks player & AI scores, wind speed/direction, and current turn.

* Quiver Simulation: Pull arrows from a virtual back quiver—mirroring real-world motion.

* In-World VR Buttons: Respawn your bow or end the game manually via physical interactions.

## Maps/Scenes
* Main Menu Scene – Entry point with settings and mode selection.

* Training Fields – A calm forest environment, great for learning the basics.

* Kingdom – A feature rich city based in medieval times.

* Transition Scene – Adds narrative and immersion between maps.

## Custom Assets & Animation
Every map and animation was crafted from scratch using Blender—this process involved extensive modeling, UV unwrapping, and texture creation. The assets and animations are entirely unique and serve as a backbone to the game’s aesthetic and feel.

## Performance Optimization
This project was built with VR performance front of mind:

* Texture Atlases – Combined textures reduce draw calls.

* Optimized Blender Assets – Managed triangle counts carefully for VR.

* Object Pooling – Reused arrows and objects to avoid GC spikes.

* GPU Instancing – Batched similar materials for better rendering performance.

* Occlusion Culling – Disabled rendering of unseen geometry dynamically.

* Baked Lighting – Heavily relied on pre-baked lighting to avoid real-time lighting overhead, ensuring smooth framerate and rich environmental visuals.

## Technologies Used

* Unity	Game engine (C#) and XR Interaction Toolkit

* Blender	3D modeling, texturing, and animation
  
* GIMP	Texture painting & editing
  
* Audacity	Sound effect creation & editing

## Demos

**Main Menu**

* Showcases the menus with difficulty, mode, and map selections, along with the transition to the selected map.

[![Watch the video](https://img.youtube.com/vi/ZSQIoinpfak/0.jpg)](https://www.youtube.com/watch?v=ZSQIoinpfak)

**Training Fields**

* Showcases the Training Fields map along with the core mechanics of that game, which include the AI opponent, bow and arrow physics, points system, and much more.

[![Watch the video](https://img.youtube.com/vi/7sTPK3tMGlQ/0.jpg)](https://www.youtube.com/watch?v=7sTPK3tMGlQ)

**Kingdom**

* Showcases the Kingdom map.

[![Watch the video](https://img.youtube.com/vi/rCQomPZgsG0/0.jpg)](https://www.youtube.com/watch?v=rCQomPZgsG0)
