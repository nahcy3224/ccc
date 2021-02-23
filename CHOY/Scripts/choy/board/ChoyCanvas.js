class ChoyCanvas {
  constructor(id, signalR_hub, background_color = "#fff") {
    this.projectId = getUrlParameter('ProjectID')
      ? getUrlParameter('ProjectID') : "P0001" // 暫時性內容，開發完成後請刪除
    this.boardId = getUrlParameter('BoardID')
      ? getUrlParameter('BoardID') : "B0001" // 暫時性內容，開發完成後請刪除
    this.canvas = new fabric.Canvas(id, {
      backgroundColor: background_color,
      width: window.innerWidth,
      height: window.innerHeight,
      isDrawingMode: true
    })
    this.socket = new ChoySocket(signalR_hub, this.canvas)
    this.mode = { status: 0 }
    /**
     * Zoom in、Zoom out 功能
     *  為防止 zoom in 或 zoom out 時，因執行點不同而導致畫布偏移，採用堆疊 (stack)
     *  讓執行 zoom in 或 zoom out 返回時，可依照堆疊所記錄執行點，逆向依序返回初始狀態
     */
    this.zoomStatus = {
      config: { // zoom in 和 zoom out 功能的設定值
        max: 3, // zoom 最大值
        min: 1, // zoom 最小值
        delta: 0.1 // zoom 每次更新
      },
      stack: [],
      trend: true
    }
    this.tempStorage = {
      imageDataURL: ''
    }


    if (!this.projectId || !this.boardId) {
      document.location.href = "/Project/Index" // 等待處理，轉至首頁 或 會員頁面
    }

    this.__rewriteFabricJs(this.socket)

    // 事件設定
    this.canvas.on('object:modified', () => {
      if (this.mode.status === CanvasMode.Objects) { // 物件操作模式
        this.socket.saveAndNotificationAllCanvasUpdate()
      }
    })

    this.canvas.on('mouse:up', (e) => {
      if (this.mode.status === CanvasMode.Drawing) { // 繪圖模式
        this.socket.saveAndNotificationAllCanvasUpdate()
      } else if (isInArray(CanvasMode.AddObjectModes, this.mode.status)) { // 新增物件模式
        this.canvas.discardActiveObject().renderAll() // 取消目前所有被選取物件
        if (this.mode.status === CanvasMode.AddImage) { // 增加圖片物件模式
          const img = new Image()
          img.onload = () => {
            const width = img.naturalWidth
            const height = img.naturalHeight
            this.canvas.add(new fabric.Image(img, {
              width, height,
              top: e.absolutePointer.y - height / 2,
              left: e.absolutePointer.x - width / 2
            }))
            this.tempStorage.imageDataURL = ''
            this.socket.saveAndNotificationAllCanvasUpdate()
          }
          img.src = this.tempStorage.imageDataURL
        } else {
          if (this.mode.status === CanvasMode.AddText) { //
            this.canvas.add(new fabric.IText('default text', {
              top: e.absolutePointer.y,
              left: e.absolutePointer.x,
              fontSize: 24,
            }))
          } else if (this.mode.status === CanvasMode.AddCircle) {
            const radius = 30
            this.canvas.add(new fabric.Circle({
              radius,
              top: e.absolutePointer.y - radius,
              left: e.absolutePointer.x - radius,
              fill: 'transparent',
              stroke: 'black',
              strokeWidth: 1,
            }))
          } else if (this.mode.status === CanvasMode.AddRect) {
            const width = 60
            const height = width
            this.canvas.add(new fabric.Rect({
              width, height,
              top: e.absolutePointer.y - height / 2,
              left: e.absolutePointer.x - width / 2,
              fill: 'transparent',
              stroke: 'black',
              strokeWidth: 1,
            }))
          } else if (this.mode.status === CanvasMode.AddTriangle) {
            const width = 60
            const height = width
            this.canvas.add(new fabric.Triangle({
              width,
              height,
              top: e.absolutePointer.y - height / 2,
              left: e.absolutePointer.x - width / 2,
              fill: 'transparent',
              stroke: 'black',
              strokeWidth: 1,
            }))
          }
          this.socket.saveAndNotificationAllCanvasUpdate() // 保存並通知在線上的其他人執行更新 
        }
        this.mode.status = CanvasMode.Objects
        setObjectsModeConfig(this.canvas)
      }

    })

    this.canvas.on('mouse:wheel', (e) => {
      const _max = this.zoomStatus.config.max * 100
      const _min = this.zoomStatus.config.min * 100
      const _delta = this.zoomStatus.config.delta

      const deltaZoom = (e.e.deltaY > 0 ? _delta : _delta * (-1)) * 100
      const zoom = this.canvas.getZoom() * 100
      // 已是最大值或最小值，跳離程式
      if ((zoom === _max && deltaZoom > 0) || (zoom === _min && deltaZoom < 0))
        return;

      let point = { x: e.e.offsetX, y: e.e.offsetY }

      if (this.zoomStatus.stack.length === 0)
        this.zoomStatus.trend = deltaZoom > 0

      let newZoom = zoom + deltaZoom
      newZoom = newZoom < _min ? _min : newZoom > _max ? _max : newZoom
      newZoom = parseInt(newZoom) / 100

      if ((this.zoomStatus.trend && deltaZoom > 0) || (!this.zoomStatus.trend && deltaZoom < 0))
        this.zoomStatus.stack.push(point)
      else
        point = this.zoomStatus.stack.pop()

      this.canvas.zoomToPoint(point, newZoom)
    })

    this.socket.getCanvasData() // 取得過去所儲存資料
  }
  // 以JSON格式匯出 canvas 資料
  saveByJSON = () => this.canvas.toJSON()
  // 設定 canvas 的 size
  setSize = (width = window.innerWidth, height = window.innerHeight) => {
    this.canvas.setWidth(width)
    this.canvas.setHeight(height)
  }
  // 新增圖片至 canvas
  addImageToTempStorage = (dataURL) => {
    compressedImage(dataURL, 200, (dataURL) => {
      this.tempStorage.imageDataURL = dataURL
    })
  }
  // 設定 background image 至 canvas
  setBackgroundImage = (dataURL, x = 0, y = 0, angle = 0) => {
    const img = new Image()
    img.onload = () => {
      // 依照 background image 長寬比，重新調整 canvas 的 size
      // 固定 canvas 的寬，並重新計算 canvas 的高
      const width = this.canvas.width
      const ratio = width / img.naturalWidth
      const height = img.naturalHeight * ratio
      this.setSize(width, height)
      this.canvas.clear() // 清除畫布所有內容
      resizeImage(dataURL, width, height, (dataURL) => { // 調整照片資料尺寸 (降低儲存與傳輸成本)
        fabric.Image.fromURL(dataURL, (fabric_image) => { // 設定成 canvas 背景
          this.canvas.setBackgroundImage(fabric_image.set({
            top: y,
            left: x,
            angle: angle,
            width: width,
            height: height
          }), () => {
            // 保存更新並通知在線上的其他人執行更新 
            this.socket.saveAndNotificationAllCanvasUpdate()
          }).renderAll()
        })
      })
    }
    img.src = dataURL
  }
  // 設定畫筆 size
  setBrushSize = size => { this.canvas.freeDrawingBrush.width = size }
  // 設定畫筆 color
  setBrushColor = color => { this.canvas.freeDrawingBrush.color = color }
  // 設定 canvas mode
  setMode = mode => {
    // 參數檢查
    if (typeof mode !== 'number')
      throw new Error("mode 設定失敗，輸入參數 type 錯誤 !!")

    this.mode.status = mode
    switch (this.mode.status) {
      case CanvasMode.Drawing: // drawing mode
        setDrawingModeConfig(this.canvas)
        break
      case CanvasMode.Objects: // objects mode
        setObjectsModeConfig(this.canvas)
        break
      case CanvasMode.AddText: // add Object mode
      case CanvasMode.AddImage:
      case CanvasMode.AddCircle:
      case CanvasMode.AddRect:
      case CanvasMode.AddTriangle:
        setAddObjectModeConfig(this.canvas)
        break
      default: // 當都不匹配的預設狀態 drawing mode
        this.mode.status = CanvasMode.drawing
        setDefaultModeConfig(this.canvas)
        console.warn("mode 設定為無效設定，採用預設設定")
        break
    }
  }
  // 取得目前 canvas mode
  getMode = () => this.mode.status
  __rewriteFabricJs = (ChoySocket) => { // 覆寫 fabric 功能
    const cancel = new Image()
    cancel.src = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAEyklEQVRYR62WbUwTdxzHv0XbG09FhMyhKAxBHC7bQAX3ELMt4N4sviBrJnFje0G2LBluBnGIsmTEZdTZ2OmAoTE+4LJsWTKjoDI3zCyPc6boEObYGMp42BBaaWmB+3u3XK/X9q53bYH1TZPev//v5/t7PBUUPkM5yFvRictKz/+v31VyF7FA2HAOZvWDSDk8jLsLFbu9NqLl4rij7P0xtEjv8gNwi9MJxsawv0vySU3/TFrVKAbmC9Gbph57NG9bfF/zWfv5f2wF5RY0+N4lAvAVdx2iCQZ368ihgdk04zwgBHENFQEQgu5LX9sb7tlFEB4AFlAN5+BBgrGR/40mrj+BpjGwr5AcHpwbhFScuwfkAW42f2c/P+YoqLDxkfAADOWATTA28tHxiHsh/qp8ixzpn00zWoOnw1/cew8HccN0wd5gcbogVGwqqJFV2rZHPv4qS+qcOyyQc9/9+mLy+QAdEELJue89XGQ7r7fcuTo2ucMVgVdj8fjBxyjziqpvFvNhFxP7/vlPYympvisPEcy5cE+P2TR9wuKoNljJFU8Kti5B5pF0qnNl5Sm11LkUqq/mQ1I7JIYI1XmPuWX6pMVRc9BKmgFcFnVBfgyyDGuojqS9dUEgCG4f30/qRogrHXNxfsriqP3USn7kxAHM+s0BXQzW61Op9uRdn6mDpeO3egNhGUyu3lKwVGg1ca6FGiLgnNdbHLV6H3FRF/gOh20x2LA/hWpLKdZLIMS1wUw7wdI0FrEqd8vK1w4nftoy9cUBK/lBcC7oyY5i7iEHUZmkaUt9+6Og6RDmhVzt9JhbFcUVIyDQuSBWalpT3yzX+Iv4dou8896uVufpCXudXsZ50AgIB7ZrsbEiUdOypmCnG0IQU25VDra3q81ZP2E/WmUlXLG5Ck5unyimwPdwoRbZZcs1prX574QEwTk/M2E/9omVfB9IPGgKfCG6M6L6knJyUyOXLPPsCLmc044pdHe1ObaM0tvHgQtKzkNOAXewN50aTn4hP4EK1wasdr4F+dq4bjYx+8bZxCZgJNAqD5oCf3F+q0lnu9wC++XWNabCioAQAQHm41w6vH7+41emdBKJJoVIKAIsxLk3Qnw6Ogb7mA/s8hCyAKE6//3WNduUbRKZGdnRymObh2j/d5Apc/hD+AGE6ry3q915ZsJ2rMlGzEdjF9VlpWdppM49UNx6p2m03r/HlM+IIUQAoTrnhgwnLvS5To2ndkeHmdYnr1OGcL/etTjtzF7aC+EBmKtz6ZDRqZG9KyLMtHH5au+wcjuXds1VhjAVDA+hurMKsTNRkWeTXnxls1r9UMA+lzqXDhkOooRSmbLjEyUTU9K6LIufgNEO4GVXBA49jPy8OO2JjFydVmmfCzkPNl51QPbOcJVpU3ScTzq87wWc3g3Adgk4twf41pOC0hgUFS6LNqzbvNUPIphz6aTjIN6jYHqGihJDsCxuAraLvPg5v1eykhgUvR4fZXhi00tuCG6r8dUezLkcxA41TM+yi/l0iJ27xAFY/NqwJBpFr8VFGp7Mel47V+dyEMVhMD3HQCN1zokrbsM3wlHwbqym5uQUaqvvz7YGW6mBlk0ukLkHuNIEfHkAXO3xzkPZhk8DWLoQcR+wFAAbpOLc8/8AWHSaqrGGVdQAAAAASUVORK5CYII='
    // 變更所有物件畫出的控制項
    fabric.Object.prototype.drawControls = function (ctx, styleOverride) {
      styleOverride = styleOverride || {};
      const wh = this._calculateCurrentDimensions();
      let width = wh.x;
      let height = wh.y;
      let scaleOffset = styleOverride.cornerSize || this.cornerSize;
      let left = -(width + scaleOffset) / 2;
      let top = -(height + scaleOffset) / 2;
      let transparentCorners = typeof styleOverride.transparentCorners !== 'undefined'
        ? styleOverride.transparentCorners
        : this.transparentCorners;
      let hasRotatingPoint = typeof styleOverride.hasRotatingPoint !== 'undefined'
        ? styleOverride.hasRotatingPoint
        : this.hasRotatingPoint;
      let methodName = transparentCorners ? 'stroke' : 'fill';

      ctx.save();
      ctx.strokeStyle = ctx.fillStyle = styleOverride.cornerColor || this.cornerColor;
      if (!this.transparentCorners)
        ctx.strokeStyle = styleOverride.cornerStrokeColor || this.cornerStrokeColor;
      this._setLineDash(ctx, styleOverride.cornerDashArray || this.cornerDashArray, null);

      // top-left
      this._drawControl('tl', ctx, methodName, left, top, styleOverride);

      // top-right
      // this._drawControl('tr', ctx, methodName, left + width, top, styleOverride);
      ctx.drawImage(cancel, left + width, top, this.cornerSize, this.cornerSize)
      // bottom-left
      this._drawControl('bl', ctx, methodName, left, top + height, styleOverride);

      // bottom-right
      this._drawControl('br', ctx, methodName, left + width, top + height, styleOverride);

      if (!this.get('lockUniScaling')) {
        // middle-top
        this._drawControl('mt', ctx, methodName, left + width / 2, top, styleOverride);
        // middle-bottom
        this._drawControl('mb', ctx, methodName, left + width / 2, top + height, styleOverride);
        // middle-right
        this._drawControl('mr', ctx, methodName, left + width, top + height / 2, styleOverride);
        // middle-left
        this._drawControl('ml', ctx, methodName, left, top + height / 2, styleOverride);
      }
      // middle-top-rotate
      if (hasRotatingPoint)
        this._drawControl('mtr', ctx, methodName, left + width / 2, top - this.rotatingPointOffset, styleOverride);

      ctx.restore();

      return this;
    }

    fabric.Canvas.prototype._getActionFromCorner = function (target, corner, e) {
      if (!corner)
        return 'drag';

      switch (corner) {
        case 'mtr':
          return 'rotate';
        case 'ml':
        case 'mr':
          return e[this.altActionKey] ? 'skewY' : 'scaleX';
        case 'mt':
        case 'mb':
          return e[this.altActionKey] ? 'skewX' : 'scaleY';
        case 'tr': // 增加刪除功能
          this.remove(target)
          ChoySocket.saveAndNotificationAllCanvasUpdate()
          return 'deleted'
        default:
          return 'scale';
      }
    }
    fabric.Canvas.prototype.cursorMap[1] = 'pointer'
  }
}
