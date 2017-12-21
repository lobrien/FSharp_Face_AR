namespace NoiseGameplayKit

open System

open Foundation
open UIKit
open GameplayKit
open SpriteKit
open OpenTK
open CoreGraphics
open ARKit
open SceneKit

type Mask(geometry : ARSCNFaceGeometry) as this = 
   inherit SCNNode()

   do
      // Maintain reference to these for manual `Dispose()` every animation frame (`moveMapForever`)
      let mutable lastTexture : SKTexture = null
      let mutable lastNoiseMap : GKNoiseMap = null

      let perlinNode ()= 
         //let seed = 1 
         let seed = int (GKRandomSource.SharedRandom.GetNextInt())
         let noiseSource = new GKPerlinNoiseSource(0.8, nint(3), 0.2, 4.6, seed)
         let noise = new GKNoise(noiseSource)
         let lowColor = UIColor.Black
         let highColor = UIColor.White
         let cDict = new NSDictionary<NSNumber,UIColor>([| new NSNumber(-0.01); new NSNumber(0.01) |], [| lowColor; highColor |])

         noise.GradientColors <- cDict
         let mapSize = new Vector2i((int) 400., (int) 400.)
         let map = new GKNoiseMap(noise, new Vector2d(80.,80.), new Vector2d(0., 0.), mapSize, false)
         let texture = SKTexture.FromNoiseMap(map)
         let noiseNode = new SKSpriteNode(texture)
         noiseNode.Size <- new CGSize(noiseNode.Size.Width * nfloat(2.), noiseNode.Size.Height * nfloat(2.))
         let zStep = new Vector3d(0., 0.03, 0.) //Honestly, I don't understand why this isn't the 3rd coordinate, but that's what works
         let moveMap = SKAction.Run (fun () ->
             if lastTexture <> null then do
               lastTexture.Dispose()
             if lastNoiseMap <> null then do 
               lastNoiseMap.Dispose()
             noise.Move(zStep);
             lastNoiseMap <- new GKNoiseMap(noise)
             lastTexture <- SKTexture.FromNoiseMap(lastNoiseMap)
             noiseNode.Texture <- lastTexture
         )
         let moveMapForever = SKAction.RepeatActionForever(SKAction.Sequence([| moveMap; SKAction.WaitForDuration(0.1) |]))
         noiseNode.RunAction moveMapForever
         noiseNode

      let faceFun = 
         let scene = new SKScene(new CGSize(nfloat(400.), nfloat(400.)))
         scene.AddChild <| perlinNode()
         scene

      let mat = geometry.FirstMaterial
      //mat.Diffuse.ContentColor <- UIColor.Red // Basic: single-color mask
      //mat.Diffuse.ContentImage <- UIImage.FromFile "fsharp512.png" // Virtual face-painting
      mat.Diffuse.ContentScene <-faceFun // Arbitrary SpriteKit scene

      mat.LightingModelName <- SCNLightingModel.PhysicallyBased

      geometry.FirstMaterial <- mat
      let el = geometry.GeometryElements.[0]
      Console.WriteLine(geometry.GeometryElements.LongLength.ToString());
      Console.WriteLine("First element is " + el.ToString())
      // of type triangles 

      this.Geometry <- geometry
     
   
   member this.Update(anchor : ARFaceAnchor) = 
      let faceGeometry = this.Geometry :?> ARSCNFaceGeometry

      faceGeometry.Update(anchor.Geometry)
