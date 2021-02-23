
const imagesQueue = new ImagesQueue("images-group")
const choyCanvas = new ChoyCanvas("choyCanvas", $.connection.boardHub, "#fff")
choyCanvas.setSize(1200, 900)

// 上傳圖片到 canvas 成為物件或背景圖
// const imageUploader = document.getElementById('image-uploader')

// imageUploader.addEventListener('change', function () {
//   if (!this.files.length > 0)
//     return;

//   const file = this.files[0]
//   blobToDataURL(file, function (dataURL) {
//     const uploadImageController = document.getElementById('upload-image-controller')
//     const items = [...uploadImageController.querySelectorAll('input[type="radio"]')]
//     const value = items.filter(item => item.checked)[0].dataset.func
//     switch (value) {

//       case 'add-image':
//         const img = document.createElement("img")
//         img.addEventListener('mousedown', function () {
//           choyCanvas.setMode(CanvasMode.AddImage)
//           choyCanvasMode.value = "物件模式"
//           choyCanvas.addImageToTempStorage(dataURL)
//         })
//         img.src = dataURL
//         imagesQueue.add(img)
//         break


//       case 'set-background-image':
//         choyCanvas.setBackgroundImage(dataURL)
//         break
//     }
//   })
//   this.value = ""
// })
///////
const imageUploader = document.getElementById('image-uploader')

imageUploader.addEventListener('change', function () {
    if (!this.files.length > 0)
        return;
    const file = this.files[0];
    blobToDataURL(file, function (dataURL) {
        const img = document.createElement("img");
        img.addEventListener('mousedown', function () {
            choyCanvas.setMode(CanvasMode.AddImage)
            choyCanvas.addImageToTempStorage(dataURL)
        })
        img.src = dataURL;
        imagesQueue.add(img)
    })
})

const backgroundUploader = document.getElementById('background-uploader')
backgroundUploader.addEventListener('change', function () {
    if (!this.files.length > 0)
        return;
    const file = this.files[0];
    blobToDataURL(file, function (dataURL) {
        choyCanvas.setBackgroundImage(dataURL)
    })
})
// 畫筆size與color
const brushResizer = document.getElementById('brush-resizer')
const brushSizeValue = document.getElementById('brush-size-value')
brushSizeValue.value = brushResizer.value
brushResizer.addEventListener('input', function () {
  choyCanvas.setBrushSize(Number(this.value))
  brushSizeValue.value = this.value
})

const brushPalette = document.getElementById("brush-palette")

brushPalette.addEventListener('input', function () {
  choyCanvas.setBrushColor(this.value)
})


// 切換模式
// const toggleModeBtn = document.getElementById('toggle-mode-btn')
// const choyCanvasMode = document.getElementById('choy-canvas-mode')

// if (choyCanvas.getMode() === CanvasMode.Drawing)
//   choyCanvasMode.value = '繪畫模式'
// else if (choyCanvas.getMode() === CanvasMode.Objects)
//   choyCanvasMode.value = '物件模式'
// else
//   choyCanvasMode.value = '繪畫模式'


// toggleModeBtn.addEventListener('click', function () {
//   switch (choyCanvas.getMode()) {
//     case CanvasMode.Drawing:
//       choyCanvas.setMode(CanvasMode.Objects)
//       break
//     case CanvasMode.Objects:
//       choyCanvas.setMode(CanvasMode.Drawing)
//       break
//     default:
//       choyCanvas.setMode(CanvasMode.Drawing)
//       break
//   }

//   if (choyCanvas.getMode() === CanvasMode.Drawing)
//     choyCanvasMode.value = '繪畫模式'
//   else if (choyCanvas.getMode() === CanvasMode.Objects)
//     choyCanvasMode.value = '物件模式'
//   else
//     choyCanvasMode.value = '繪畫模式'
// })
/////
const pen=document.getElementById('pen_img')
const hand=document.getElementById('hand_img')
const control=document.getElementById('control')
if (choyCanvas.getMode() === CanvasMode.Drawing){
  pen.src = '/Images/pen_on.png';
  hand.src='/Images/hand.png';
}
else if (choyCanvas.getMode() === CanvasMode.Objects){
  pen.src = '/Images/pen.png';
  hand.src='/Images/hand_on.png';
}
else{
  pen.src = '/Images/pen_on.png';
  hand.src='/Images/hand.png';
}

hand.addEventListener('click', objEvent)
pen.addEventListener('click', objEvent)

function objEvent(){
  switch (choyCanvas.getMode()) {
      case CanvasMode.Drawing:
        choyCanvas.setMode(CanvasMode.Objects)
        break
      case CanvasMode.Objects:
        choyCanvas.setMode(CanvasMode.Drawing)
        break
      default:
        choyCanvas.setMode(CanvasMode.Drawing)
        break
    }
    if (choyCanvas.getMode() === CanvasMode.Drawing){
      pen.src = '/Images/pen_on.png';
      hand.src='/Images/hand.png';
    }
    else if (choyCanvas.getMode() === CanvasMode.Objects){
      pen.src = '/Images/pen.png';
      hand.src='/Images/hand_on.png';
    }
    else{
      pen.src = '/Images/pen_on.png';
      hand.src='/Images/hand.png';
    }
}


// 添加自訂文字
const toolController = document.getElementById('tool-controller')
const buttonGroup = toolController.querySelectorAll('button[data-obj]')
buttonGroup.forEach(button => {
  button.addEventListener('click', function () {
    switch (this.dataset.obj) {
      case 'text':
        choyCanvas.setMode(CanvasMode.AddText)
        break
      case 'circle':
        choyCanvas.setMode(CanvasMode.AddCircle)
        break
      case 'rect':
        choyCanvas.setMode(CanvasMode.AddRect)
        break
      case 'triangle':
        choyCanvas.setMode(CanvasMode.AddTriangle)
        break
      default:
        throw new Error("異常新增物件操作")
    }
    choyCanvasMode.value = "物件模式"
  })
})





