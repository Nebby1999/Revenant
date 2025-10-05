# Revenant 2.0 

## Revenant

A Void corrupted survivor heavily inspired by the Revenant from Doom.

## Overview

The revenant is an undead, void infested survivor (Survivor type undecided, could still be an engineer) from the crash of contact light, stumbling themselves into the void locked away in the void fields, after the Survivors from UES Save Travels stumble into the void fields and escape, the survivor is unleashed but changed forever by the depths, becoming the revenant and escaping thru the portal and freeing themselves from the void.

Not quite human, nor void infested, nor dead, the revenant has a unique hover and jetpack mechanic which allows them to stay in the air for prolonged time, however, the revenant itself is quite frail.

### Jetpack Mechanic

The revenant’s main gameplay gimmick is his jetpack, while the revenant has an innate -5 armor and less health than huntress, the revenant makes up for it with his innate ability to fly and fall damage immunity.

The jetpack works with a fuel system, the player starting with 100FP

While the jetpack allows for great air mobility, the player cannot gain extra jump height or extra jumps, as these instead increase the revenant’s max fuel.

25FP+ per extra jump  
1FP+ per extra jump height

As being one with the void, having access to void items and collecting them increases the revenant’s FP regeneration.  
Base FP regeneration is 5 per second, each void item increases this value by 1.

Certain skills don’t have cooldown, but instead consume FP

The amount of FP the player has can be seen with a HUD overlay right nextt to the crosshair

### Potential Titles

Revenant - Undead of the Depths  
Revenant - Corrupted Fugitive

### Potential Ending Quotes:

| Quote Type | Quote | Notes |
| --- | --- | --- |
| Escape | And so they left, not quite alive nor dead |     |
| Escape | And so they left, Scream Eternal. | Play on Doom Eternal |
| Vanish | And so they vanished, agitation smothered | Agitating Skeleton joke |
| Vanish | And so they vanished, silenced forever |     |

### Skill Set

| Skill Type | Skill Name | Short Description | Cooldown | Unlock Methods | Notes |
| --- | --- | --- | --- | --- | --- |
| Passive | ??? | Extra Jumps and Jump Strength gets transformed into Void Energy. | Typical passive, maybe find a way to add the jetpack thing? idk |     |     |
| Primary | Void Slash | **Snaring**, swing your fists/claws forwards, dealing 200% Damage | No Cooldown | Unlocked by Default | Inspired by the punching and slashing from the revenants |
| Secondary | Void Rockets | **Jailing**, Fire Void rockets dealing 50% damage on impact and detonating for 400% Damage, every 3rd rocket volley is **Homing** | 3 second cooldown, 5 stocks, 3 stocks regained. | Unlocked by Default | The revenant’s rockets inspired by their regular attack from the Doom series |
| Secondary | Void Laser | **Jailing**, Fire a void laser dealing 300% damage, every 3rd volley fire from both nozzles | 3 second cooldown, 5 stocks, 3 stocks regained | Revenant: Nullified | Inspired by the laser revenant variant from Project Brutality 3 |
| Utility | Abyssal Dash | **Consuming**, spend 25VE and dash in the direction you’re moving. | No cooldown, consumes VE instead | Unlocked by Default | Inspired by the Battlemode Dash |
| Utility | Shriek | **Consuming**, spend 40VE and unleash a collapsing scream, pushing everything away from you and inflicting Collapse | 1 second cooldown, consumes 40VE | Revenant: To the skies | Inflicts collapse (might have to rewrite it so it works the same but isnt tied to DLC at all to avoid issues) |
| Special | Rocket Barrage | **Jailing**, Rise into the air, aim and fire a stream of 15 rockets that explode for 15x350% damage | 15 second cooldown | Unlocked by Default | Inspired by the rocket volley ability from Batttlemode |
| Special | Laser Barrage | **Jailing**, Rise into the air, aim and fire 15 lasers that deal 15%300 damage | 15 second cooldown | Revenant: Nullified | Same as above, but with lasers |

#### Keywords

*   **Snaring**
    *   Inflicts a building debuff, each stack reduces movement speed by 10%, reaching 10 stacks effectively reduces movement speed to 0, having more than 10 stacks has no effect
    *   Sinergizes with Jailing
*   **Jailing**
    *   Attacking a Snared enemy with a Jailing skill restores `x * procCoefficient * snareCount` VE, where x is a custom number that’ll be decided eventually, lol
    *   it also does ??? (idk. i want another effect)
*   **Consuming**
    *   Consumes VE for the skill to function

### BuffDefs

*   Snared
    *   direct multiplier applied to movement speed, where `movementSpeed *= -10 * snareCount`
    *   Reaching 10 stacks effectively sets movement speed to 0

### Unlocks

| Unlock Name | Condition | Note |
| --- | --- | --- |
| Character Unlock | Cheat death and survive a void implosion | Multiple ways of doing this, a player could easily obtain dios/larva and die to a void reaver/jailer/devastator |
| Revenant: Nullified | Reach a total of 10 Snare stacks |     |
| Revenant: To the skies | Complete the teleporter event without touching the ground | Theres a grace timing of 10 seconds after the teleporter event is started where this achievement isn’t failed if the player’s on the ground |
| Revenant: Audiocidity | As revenant, find and play a brass trumpet |     |

### Equipment - Brass Trumpet

*   A single brass trumpet that can appear from equipment barrels, using it fires a note that bounces between enemies, increasing in damage as it flies

### Skin Ideas

| Skin Name | Condition | Appearance |
| --- | --- | --- |
| Revenant | Unlocked by Default | The default appearance |
| Ghoul | Unlocked by Default | Reference to the Ghoul enemy from DoomRPG, essentially a pallete swap |
| Fiend | Unlocked by Default | Reference to the Fiend enemy from DoomRPG, essentially a pallete swap |
| [MASTERY] | Mastery Unlock | no clue for now |
| [GRAND MASTERY] | Grand Mastery Unlock | No clue for now (requires any difficulty with a scaling value greater than 3.5, or using Inferno) |
| Alloyed | Revenant: Audiocidity | TRUMPETS AS ROCKET LAUNCHERS, DOOT DOOT!! |