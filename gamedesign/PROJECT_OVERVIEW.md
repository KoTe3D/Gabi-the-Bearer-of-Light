# Gabi, the Bearer of Light (Габи, несущий свет)

### Core Concept

Narrative adventure / visual novel with survival mechanics. The player makes choices that directly impact the survival of Gabi's friends and sister. Gabi bears the light but has a complex relationship with the dark (mechanics TBD).

### Visual \& Art Direction (Approved)

* **Style:** Pure 2D Pixel Art (Reference: *No Place for Bravery*).
* **Technique:** Hand-drawn / pre-rendered 2D sprites and modular environments. Depth and volume are achieved through composition, layering, and 2D lighting/post-processing, NOT 3D geometry.
* **Camera:** Primarily fixed side-view (acting like a comic panel or diorama). Occasional transitions to first-person perspective with subtle camera movement for narrative emphasis.

### Core Gameplay Loop

1. Observe the scene and read dialogue.
2. Make a narrative or tactical choice.
3. Resolve consequences (branching paths, character survival/death).
4. Progress to the next scene.

## Tech Stack (Approved)

* Engine: Unity 6 (6000.5.10.f1)
* Template: Universal 2D (URP + 2D Renderer)
* Language: C# (following code-style.md)

### Narrative / Choice Design (Approved)

* Choices branch dialogue text and set story flags.
* Early and mid-game choices do NOT kill main characters (Gabi, friends, sister).
* A secondary NPC may die as a choice consequence; the death is tracked as a flag and can affect later scenes.
* Major character death moments are reserved for the late game (design TBD).

### Dialogue Flow (Approved)

* Click advances dialogue; click also speeds up / skips typing (typewriter TBD).
* Line kinds: Spoken; Thought (не озвучивается, визуально отличается); StageDirection (ремарка, без говорящего).
* Choices: навестись + клик; ветки ведут к лор-моментам, смертям NPC и плохим концовкам (ближе к финалу).
* Choice feedback mechanic: после важного выбора компаньон комментирует (Том — плохой выбор, Амиция — хороший), фразы свои для каждой сцены. Реализуется после ядра диалогов.

### Saves (Approved rules)

* Автосохранение только при переходе между локациями.
* Ручное сохранение доступно перед любым диалогом; сохранение во время диалога при загрузке возвращает к началу этого диалога.

