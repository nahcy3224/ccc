class ChoySocket {
  constructor(signalR_hub, fabric_canvas) {
    this.projectId = getUrlParameter('ProjectID')
      ? getUrlParameter('ProjectID') : "P0001" // 暫時性內容，開發完成後請刪除
    this.boardId = getUrlParameter('BoardID')
      ? getUrlParameter('BoardID') : "B0001" // 暫時性內容，開發完成後請刪除

    this.socket = signalR_hub
    this.canvas = fabric_canvas

    this.socket.client.onConnected = (connectionId) => {
      // console.log('加入:', connectionId);
    }
    this.socket.client.onDisconnected = (connectionId) => {
      // console.log('離開:', connectionId);
    }
    this.socket.client.getCanvasData = () => {
      this.getCanvasData(this.projectId, this.boardId)
    }
    $.connection.hub.start().done(() => {
      this.socket.server.joinRoom(this.projectId) // 加入特定房間
    })
  }
  // 取得 database 內所保存的 canvas 資料
  getCanvasData = () => {
    const api = `/api/project/${this.projectId}/board/${this.boardId}`
    const successCallback = (response) => {
      const data = response.Data
      if (data.Code) {
        const canvas = JSON.parse(data.Code)
        this.canvas.loadFromJSON(canvas) // 加載canvas資料
        if (canvas.backgroundImage) { // 如果有設定 background image，依其大小執行 resize
          const width = canvas.backgroundImage.width
          const height = canvas.backgroundImage.height
          this.canvas.setWidth(width)
          this.canvas.setHeight(height)
        }
      }
    }
    const errorCallback = (response) => {
      // console.log(response);
      console.warn("同步發生錯誤")
      alert("同步發生錯誤")
    }
    response(api, "GET", successCallback, errorCallback)
  }
  // 保存更新並通知在線上的其他人執行更新
  saveAndNotificationAllCanvasUpdate = () => {
    const canvas = JSON.stringify(this.canvas.toJSON())
    const api = `/api/project/${this.projectId}/board/${this.boardId}`
    const successCallback = () => { this.notificationAllCanvasUpdate() }
    const errorCallback = (response) => {
      // console.log(response);
      console.warn("同步發生錯誤")
      alert("同步發生錯誤")
    }
    const data = { "Canvas": canvas }
    response(api, "Patch", successCallback, errorCallback, data)
  }
  // 通知在線上的其他人執行更新
  notificationAllCanvasUpdate = () => {
    this.socket.server.notificationUpdate(this.projectId, this.BoardID)
  }
}