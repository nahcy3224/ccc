class CanvasMode {
  static get Drawing() { return 0 }
  static get Objects() { return 1 }
  static get Eraser() { return 2 }
  static get AddText() { return 3 }
  static get AddImage() { return 4 }
  static get AddCircle() { return 5 }
  static get AddRect() { return 6 }
  static get AddTriangle() { return 7 }
  static get AddObjectModes() {
    return [
      CanvasMode.AddText,
      CanvasMode.AddImage,
      CanvasMode.AddCircle,
      CanvasMode.AddRect,
      CanvasMode.AddTriangle
    ]
  }
}

function setDrawingModeConfig(fabric_canvas) { // drawing mode
  fabric_canvas.isDrawingMode = true
  fabric_canvas.selection = true
  fabric_canvas.hoverCursor = 'move'
  fabric_canvas.forEachObject(obj => { obj.selectable = true })
}
function setObjectsModeConfig(fabric_canvas) { // objects mode
  fabric_canvas.isDrawingMode = false
  fabric_canvas.selection = true
  fabric_canvas.hoverCursor = 'grab'
  fabric_canvas.forEachObject(obj => { obj.selectable = true })
}
function setEraserModeConfig(fabric_canvas) { // eraser mode
  fabric_canvas.isDrawingMode = false
  fabric_canvas.selection = true
  fabric_canvas.hoverCursor = 'move'
  fabric_canvas.forEachObject(obj => { obj.selectable = true })
}
function setAddObjectModeConfig(fabric_canvas) { // add Object mode
  fabric_canvas.isDrawingMode = false
  fabric_canvas.selection = false
  fabric_canvas.hoverCursor = 'default'
  fabric_canvas.forEachObject(obj => { obj.selectable = false })
}
function setDefaultModeConfig(fabric_canvas) {
  setDrawingMode(fabric_canvas)
}
