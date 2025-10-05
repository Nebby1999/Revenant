# Revenant Anim Doc

"An undead, void infested survivor that has been stuck in the cells for what feels like aeons, After its painful transfiguration granting them immense height, void-powered rocket launchers and an intense unfiltered rage, the revenant seeks to avenge their former self by take out the Voidling"

The revenant is basically what happens if you take the Revenant from the new Doom Games, and implement it as a survivor in Risk of Rain 2, it takes inspiration from multiple iterations of the Enemy across the games.

The main things the animation should evoke are:

1. Agility
2. Bloodlust
3. "Endorphin-Induced rage"
4. Chaos

Most of the animtations should evoke similar feelings to the revenants found in:

Doom 2016
Doom Eternal

Feel free to even "Break" the limbs if necesary, revenant is mostly a Skeleton and as such isnt bound by flesh limitations such as muscles and ligaments (iirc the lower extremities arent directly attached.)

# Main States

As the revenant has a jetpack, it's main states are split between Grounded and Flying.

## Grounded State

The revenant is hunched down, while still pertaining some considerable height nontheless (he should appear taller than most survivors in the base game.)

Focus more on chaotic and agile movements while in this state.

## Flight State

When the revenant jumps, if the player keeps pressing the spacebar it will take flight, while flying the revenant is no longer hunched down and instead is considerably more straight (it's hurtbox doesnt change).

Take into inspiration how the Revenant behaves in [Doom: The Dark Ages](https://youtu.be/WmCbaq_C9aY?t=1246)

# Basics

## Universal

* Aim Pitch
* Aim Yaw
    * Pitch and Yaw should affect mostly from its hips upwards, utilize the cannons to their fullest potential as they can also rotate around.

## Grounded State

* Jump
* Landing from Jump
    * (Tapping the jump input doesnt cause the revenant to fly.)
* Move
    * Forwards
    * Backwards
    * Right
    * Left

## Flying State

* Move
    * Forwards
    * Backwards
    * Right
    * Left
        * The thrusters on the jetpack can rotate, use these too as the player will be mostly looking at the back of the revenant.

## Menu

Idle In -> Idle -> Idle Action

* Idle In: The revenant flies from above and lands into the ship, letting out a shriek and agitating as a result.
* Idle: The revenant appears hunched down so he can fit inside the survivor diorama, its eye darts around and it's extremities twitch slightly.
* Idle Action: The revenant proceeds to curl down and later stand straight, shrieking again. Standing should fully reveal it's true height.

## Death State

The revenant has a special death animation that plays depending on the direction from which the killing blow was made. The closer the killing blow is to it's back the higher the chances for this unique animation to play

If the chance for the special death state fails, then the revenant ragdolls like other survivors.

Instead of Ragdolling, the revenant's jetpack/caparace  destabilizes, beginning a void implosion, as this happens, the revenant looses complete control, causing it to fly against the user's will and spin around erratically. (Reference to a death animation revenants in 2016/eternal have, couldnt find an actual clip for it.); the void implosion kills anyone caught by it.

# Skill Related

Note: The revenant _can_ attack both in mid-air and grounded, keep this in mind when animating

The revenant's attacks that utilize it's cannons are split into 2 different ammo types.

### Laser

Lasers are bullets, considerably quicker fire-rate with less consumption of Void Energy, but at the expense of damage dealt. They're fired from the cannons and should have minimal recoil.

### Rocket

Void Rockets that are fired from the launchers, slower fire-rate compared with the Laser and higher void consumption but considerable more damage. The cannons should have a considerable amount of recoil.

#### Sidenote
Knowing this, maybe its worth making a state that can be masked out and blended between low and high recoil in unity?

## Primary

The revenant has a singular primary: "Void Slash"

It's a stepped skill def that has a total of 2 Steps, where the revenant alternates between each arm to punch/slash its oponent.

## Secondary

The hallmark of the revenant. The revenant fires projectiles from the launchers. This is a stepped skill def with a total of 3 steps.

* Step 1: Fires from the Left Rocket Launcher
* Step 2: Fires from the Right Rocket Launcher
* Step 3: Fires from both Rocket Launchers at the same time.

## Utility

The revenant has two utilities

### Abyssal Dash

The revenant dashes in the direction they're walking, the dash comes from the actual jetpack, for grounded i'd expect it to slightly hover off the ground during the move, while for the airborne version it should be considerably more exaggerated

## Collapsing Shriek

The revenant recoils back then fires a collapsing shriek, which stuns enemies and lowers their morale, reducing attack speed and increasing their cooldowns.

## Special

For the special, the revenant is forced into its airborne state, rising upwards and aiming to the ground. After input from the player (or 3 seconds idling), the revenant fires a barrage of Projectiles towards the area, dealing massive damage.

It's a combination from the [Doom Eternal](https://youtu.be/amcP9YWvtMA?t=67)'s Battlemode Special, due to the lack of good animation data, take into consideration the homing curved skull attack from [Doom the Dark Ages](https://youtu.be/WmCbaq_C9aY?t=1288)

# Bonus

* Add an IdleAction thats used in run

## Taunts/Emotes

* Emote 1:
    The revenant stands straight, similar vibes to Ridley's taunt in [Smash Ultimate](https://www.youtube.com/shorts/0VgQ0BW2uuo?t=6&feature=share) where he stands relatively straight
* Emote 2:
    He does a big shriek. Skeleton too agitated.

## Skin Specific Taunts (You're super epic if you do this)

If necesary just make these instead of the taunt/emotes above.

There's a special skin for the Revenant "Voided Brass", which replaces its caparace with brass trumpets, similar to the DOOT skin from [Doom Eternal](https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fi.kym-cdn.com%2Fphotos%2Fimages%2Foriginal%2F001%2F501%2F653%2F39d.jpg_large&f=1&nofb=1&ipt=e4c9a12ad3db3d4ad1bfa31e5fbb97ffcf5d024a2f639e840c9f08ba58949773)
* Emote 1: The revenant pulls out a Trumpet (similar to enforcer pulling out a lawn chair) and does a doot-doot
* Emote 2: The revenant deadass busts a [move](https://youtu.be/kjW17dALszo) (Trumpet Excluded from the video). Take inspiration from the Revenant Dancing gif i can send if needed.
