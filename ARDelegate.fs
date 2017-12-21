namespace FunnyFace

open ARKit
open SceneKit
   
// Delegate object for AR: called on adding and updating nodes
type ARDelegate(faceGeometry : ARSCNFaceGeometry) =
   inherit ARSCNViewDelegate()
  
   // The geometry to overlay on top of the ARFaceAnchor (recognized face)
   let faceNode = new Mask(faceGeometry)

   override this.DidAddNode (renderer, node, anchor) = 
      match anchor <> null && anchor :? ARFaceAnchor with 
      | true -> node.AddChildNode faceNode
      | false -> ignore()   

   override this.DidUpdateNode (renderer, node, anchor) = 

      match anchor <> null && anchor :? ARFaceAnchor with 
      | true -> faceNode.Update (anchor :?> ARFaceAnchor)
      | false -> ignore()
