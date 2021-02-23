class ImagesQueue {
  constructor(id, length = 5) {
    this.el = document.getElementById(id)
    this.queue = []
    this.length = length > 0 ? length : 5
    this.init()
  }
  init = () => {
    this.render()
  }
  render = () => {
    this.el.innerHTML = ""
    const queue = [...this.queue]
    queue.forEach(img => { this.el.append(img) })
  }
  add = (imgObj) => {
    if (!imgObj.tagName || imgObj.tagName.toLowerCase() !== "img")
      throw new Error("輸入物件type錯誤，請輸入IMG物件")
      
    if (this.queue.length >= this.length) {
      this.queue.reverse()
      this.queue = this.queue.splice(0, this.length - 1)
      this.queue.reverse()
    }
    this.queue.push(imgObj)
    this.render()
  }

  getContent = () => [...this.queue]

  setLength = (length = 5) => {
    if (length <= 0 || length === this.length) {
      // console.warn("")
      return
    }
      
    this.length = length
    this.queue = this.queue.splice(0, length)
    this.render()
  }

  clear = () => {
    this.queue = []
    this.render()
  }
}