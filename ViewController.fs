namespace FunnyFace

open System

open Foundation
open UIKit
open ARKit
open SceneKit

[<Register ("ViewController")>]
type ViewController (handle:IntPtr) =
    inherit UIViewController (handle)

    let mutable arsceneview : ARSCNView = new ARSCNView()

    let ConfigureAR() = 
       let cfg = new ARFaceTrackingConfiguration()
       cfg.LightEstimationEnabled <- true
       cfg

    override this.DidReceiveMemoryWarning () =
      base.DidReceiveMemoryWarning ()

    override this.ViewDidLoad () =
      base.ViewDidLoad ()

      match ARFaceTrackingConfiguration.IsSupported with
      | false -> raise <| new NotImplementedException() 
      | true -> 
        arsceneview.Frame <- this.View.Frame
        arsceneview.Delegate <- new ARDelegate (ARSCNFaceGeometry.CreateFaceGeometry(arsceneview.Device, false))
        //arsceneview.DebugOptions <- ARSCNDebugOptions.ShowFeaturePoints + ARSCNDebugOptions.ShowWorldOrigin

        this.View.AddSubview arsceneview

    override this.ViewWillAppear willAnimate = 
        base.ViewWillAppear willAnimate

        // Configure ARKit 
        let configuration = new ARFaceTrackingConfiguration()

        // This method is called subsequent to `ViewDidLoad` so we know arsceneview is instantiated
        arsceneview.Session.Run (configuration , ARSessionRunOptions.ResetTracking ||| ARSessionRunOptions.RemoveExistingAnchors)

    override x.ShouldAutorotateToInterfaceOrientation (toInterfaceOrientation) =
        // Return true for supported orientations
        if UIDevice.CurrentDevice.UserInterfaceIdiom = UIUserInterfaceIdiom.Phone then
           toInterfaceOrientation <> UIInterfaceOrientation.PortraitUpsideDown
        else
           true
