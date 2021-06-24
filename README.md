# Advanced Room Setup
SteamVR application that can perform quick Room Setups and save, load &amp; recontextualize play area data.

[Video](https://www.youtube.com/watch?v=J9bRodZIDV8)

## Introduction

I've been playing a lot of Beat Saber recently but I don't have a permanent VR setup, so I've had to re-calibrate a lot. I think Room Setup is really well optimized for letting users know what's happening and why it's necessary. As an advanced user I'm mostly interested in setting up a play area in as little steps as possible though and I don't need much explanation.

This application's a little hobby project to make room setup a little faster. It's made in Unity, just like the real Room Setup. I didn't spend a lot of time on this so it doesn't have a lot of features yet but it's a fun little experiment and it gets the job done.

I got a little carried away with making the UI look like the real room setup too but it was too fun not to do.

## Features
- Quick Calibration, allows you to create a rectangular play area in one step using only the controllers
- Save/load play areas to files to re-use them later
- Recontextualize play areas. It keeps track of where the base station was when it was created, and you can re-align that with the current base stations.

## Known Bugs
- It doesn't seem to recognize any controllers that are connected _after_ the application is started

## To Do
- Add a way to transform play areas like the original Room Setup
- Maybe integrate the 'perimeter drawing' feature of the orginal Room Setup in here, too
- Maybe add a way to create a new play area in VR itself, using the front-facing camera? That sounds cool.
