SpacePool
=========

My final project for Introduction to Game Programming in the Spring 2013 semester.

###BACKSTORY
In the far flung future, you and your opponent are rival mining companies. The major form of transportation between solar systems is a wormhole network. The destination when entering a wormhole is determined by a unique vibrational vector that acts as an address for a destination.
You are mining dark energy planets. These cannot be interacted with directly, so a dark matter intermediary (“cue ball”) must be used. The dark matter itself can only be manipulated by laser. The dark matter can imbue the dark energy with a vibrational signature (which changes the planet to a different color) that will slowly wear off. The vibrational signature acts as the address for the wormhole. Dark energy planets can share their vibrational signature with planets they collide with. Opposing vibrational signatures will cancel out when the planets collide. Act quickly before your vibrational signature wears off the planets, and shoot them into the wormholes to win over your opponent!

###NOTE
Upon starting the game, practice getting used to the controls... then hit R to start!

###OBJECTIVE
You and your opponent fly around in spaceships, trying to claim ownership over planets and sink them into black holes. Each of you has your own "cue ball", which is colored either light pink or light blue (depending on whether you're the red or blue player). To claim ownership over a planet (and turn it red/blue), you have to hit it with your cue ball. If a planet is owned by neither of you, it's gray. When colliding, planets can also pass on ownership; if an owned planet hits an un-owned planet, the un-owned planet becomes owned by the same player as the owned planet. However, if a planet you own collides with a planet your opponent owns, both planets become un-owned and turn gray. If it's been a set time since a planet last had its ownership set, it turns back to gray.
When an owned planet sinks into a black hole (dark purple circle), the corresponding player earns a point. If an un-owned planet sinks into a black hole, nobody gets a point. After a planet sinks into a black hole, a new planet is created just outside the screen, and drifts in to the play area.
Planets are attracted to each other and to black holes (unless certain effects are applied, see Weapons). They are repelled away from white holes (light purple circles).
When one player has racked up a certain number of points, the game is over and they win.

###CONTROLS
|Action|P1 (Red)|P2 (Blue)|
|---------|-----------|--------|
|Move forward|W|I|
|Turn clockwise (quickly)|D|L|
|Turn counter-clockwise (quickly)|A|J|
|Turn clockwise (slowly)|E|O|
|Turn counter-clockwise (slowly)|Q|U|
|Fire|Space|Question Mark|
|Switch weapons (in one direction)|F|Semicolon|
|Switch weapons (in the other direction)|Caps Lock|H|
|Time Bomb|T|T|
|Reset game|R|R|

###WEAPONS
Depending on which weapon you currently have selected, your ship will have a differently colored aura/outline.
*No aura* - Shoots a laser that only affects your "cue ball", which speeds it up in the direction that your laser is pointing.
*Pink aura* - Shoots a laser which makes the planet it hits heavier, up to 4x its initial mass. When a planet is heavier, it will have a translucent black outline.
*Orange aura* - Shoots a laser which makes the planet it hits lighter, up to 0.25x its initial mass. When a planet is lighter, it will have a translucent white outline.
*Pale yellow aura* - Shoots a bullet which only collides with your opponent's ship. If it hits their ship, you slow their move speed and turn speed, and make their cue-hitting laser weaker. This effect persists for a given period of time, then their ship turns back to normal.
*Green aura* - Shoots a "gravitational polarizer" bullet that hits planets. When it hits a planet, that planet's gravitational effect on other planets is negated (so it's still attracted to other planets, but other planets are repelled from it). This effect persists until the same planet is shot with this type of bullet again. When a planet is affected by this, it has a solid yellow outline.
*Light blue aura* - Shoots a "phase inducer" bullet that hits planets. When it hits a planet, that planet can pass through other planets without colliding. This effect persists for a given period of time, then turns off automatically. Planet ownership is still transferred as usual during this effect, if the planet passes through other planets.
*Light purple aura* - Shoots a "gravity canceller" bullet that hits planets. When it hits a planet, that planet doesn't exert a gravitational effect on other planets, and other planets do not exert a gravitational effect on it. This effect persists for a given period of time, then turns off automatically. When a planet is affected by this, it has a solid green outline.
*Time Bomb* - Doesn't have an aura, since it's not in either player's arsenal of weapons, but rather just a separate key to be pressed. When pressed, it negates all planets' velocities to simulate time going backwards. This effect can only be used a certain amount of times per game, and it has a cooldown (so it can't be used too quickly in succession).

