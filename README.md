# Ambino LED Controller 

![Ambino Application logo](adrilight/zoe.ico)

> An Application to control All Ambino's LED products with Ambilight and effect feature

## What does it do?

Adrilight which created by https://github.com/fabsenet/adrilight
Ambino take it one more step further adding lots of new function
-independently controll led zones ( screen LED, Desk LED, Case LED) with single hardware (Ambino AIO HUB and Ambino HUB V2)
-Create your own color and music reaction style (unlimited possibility)

##Things to do
-Bring new matrix LED control to the application, now you can controll n Fans or n LED strips (each has 16 individual LED) as 16xn matrix
-Separately add new devices and control interface for each device according to the device name
-Convert all Effects to matrix UI and send RGB data (3 bytes for each LEDs), then All devices no longer need to decode the serial data on it's own
Please look at the schematic below

|                |  (Ri,Gi,Bi)  [Leds[i]= Light(R,G,B)]
|   Adrilight    | ===========> [ Ambino Device       ] ======> LED Strip   
|                |                 
|_ _ _ _ _ _ _ _ |


## Thanks
*Adrilight which created by https://github.com/fabsenet/adrilight
* This is a fork from the originally ambilight clone project [bambilight by MrBoe](https://github.com/MrBoe/Bambilight) and therefore (and to met the MIT licence) a big thank you goes to [MrBoe](https://github.com/MrBoe)
* More thanks goes to [jasonpong](https://github.com/jasonpang) for his [sample code for the Desktop Duplication API](https://github.com/jasonpang/desktop-duplication-net)

## Changelog

-Change only commit when Ambino launch a new product
-Minor change is not commited a change until a new product release ( bugs)

### Release October 28-2020
-Add support for old devices Ambino Black
### Release October 10-2020
-Release new HUBV2 support
-New Music Visuallizer UI
-New Method to control independently 3 LEDs zone (Screen, Desk, Case)

