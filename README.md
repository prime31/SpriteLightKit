# SpriteLightKit

SpriteLightKit brings back the old two buffered blend trick to get pseudo lighting with just sprites. It handles the setup process of getting that second buffer blended with your normal scene.


![how it works](http://cl.ly/c7Xq/687474703a2f2f636c2e6c792f6336784c2f7370726974656c696768746b69742e706e67.png)

The same scene with two different ambient light setups:

![low ambient light](http://cl.ly/c7Lf/darker.png)
![brighter ambient light](http://cl.ly/c7DN/lighter.png)



## Setup

- create an empty GameObject as a child of your main camera and add the SpriteLightKit component
- add the SpriteLightKitImageEffect component to your main camera and drag the SpriteLightKitBlendImageEffect.shader into the inspector in the shader property
- set the Light Layer in the SpriteLightKit component, which is the layer you want to place your sprite lights on
- remove the Light Layer from your main camera's culling mask so that it does not render the sprite lights
- create some sprites using the SpriteLightMaterial and make sure they are on the Light Layer you chose in the previous step


You can set the ambient lighting by changing the background color of the camera on the SpriteLightKit GameObject. Each of your lights can use the normal sprite tint color to change how it affects the underlying scene. The SpriteLightKitImageEffect has a toggle for 1x or 2x multiplicative blending (bool use2xMultiplicationBlending). 2x is useful for a scene where the sprite lights can lighten or darken a scene. Use colors darker than the ambient color to darken the scene and lighter colors to lighten it.


Lights look best when they are white and falloff to 0 alpha. That lets you use the tint color to color the lights and the tint color.alpha to set the intensity of the lights. Get creative with your light shapes and experiment! If you need to occlude lights (if you have walls where light shouldn't pass for example) you can just use any black sprite and place it so that it blocks the light however you want it to.



## Advanced Emissive Features

SpriteLightKit has some extra, more advanced features baked in as well along with some shaders/materials to help you utilize them. The following emissive features are for use on your normal GameObjects that are not on the Light Layer. The emissive materials will write to the stencil buffer which will then be read by the SpriteLightKitImageEffect and lights will not be rendered for those pixels. This basically makes the masked pixels always display as if lights do not affect them. Materials are provided that let you use a Sprite or a Mesh as the emissive mask.

Since the emissive materials main purpose is to write to the stencil buffer you can stick your geometry that uses them behind all other geometry. This gives you some flexibility when using the SpriteLightEmissiveSpriteMaterial. If it is behind your other Sprites/geometry then it will work like a mask. If it is in front of your other Sprites/geometry it will work like a mask **and** it will also be displayed.

- **SpriteLightEmissiveSpriteMaterial** lets you use a Sprite as an emissive mask. Set the Alpha Cutoff of the Material to control which pixels will be discarded.
- **SpriteLightEmissiveMeshMaterial** lets you use a Mesh as an emissive mask.



## Advanced Shadow Features

SpriteLightKit can also cast simple offset shadows. The current implementation uses a brute force approach to find affected lights but a b-tree or other spatial search will be added to speed things up. The shadow shader is ready to handle approximated planar shadows (via skew and scale in addition to the offset) but it hasn't yet been implemented due to some decisions that need to be made about how to best handle things.

Implementing shadows requires the following steps:

- stick the SpriteLightKitLightManager on a GameObject in your scene
- add the SpriteLightKitShadow component to any objects that should project shadows
- make sure the objects that should cast shadows have the SpriteLightShadowedMaterial on them


### Credit

The sweet little town sketch is from the amazing work of @pixelatedcrown. Follow on Twitter and [Tumblr](http://pixelatedcrown.tumblr.com/) to see more awesome art!



#### License

[Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) with [simple explanation](http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US) with the attribution clause waived. You are free to use SpriteLightKit in any and all games that you make. You cannot sell SpriteLightKit directly or as part of a larger game asset.
