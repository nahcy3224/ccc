function blobToDataURL(blob, callback) {
  const fileReader = new FileReader()
  fileReader.onload = (e) => { callback(e.target.result) }
  fileReader.readAsDataURL(blob)
}

function response(api, httpMethod, successCallback, errorCallback, data = null) {
  const settings = {
    "url": api,
    "method": httpMethod,
    "timeout": 0,
    "headers": { "Content-Type": "application/json" },
    "dataType": "json",
  };
  if (data) { settings["data"] = JSON.stringify(data) }
  $.ajax(settings).done(successCallback).fail(errorCallback)
}

function getUrlParameter(name) {
  name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]')
  const regex = new RegExp('[\\?&]' + name + '=([^&#]*)')
  const results = regex.exec(location.search)
  if (!results) {
    console.warn(`未尋找到 ${name} 參數`)
    return undefined
  }
  return decodeURIComponent(results[1].replace(/\+/g, ' '))
}

function isInArray(array, element) {
  let i = array.length;
  while (i--)
    if (array[i] === element)
      return true
  return false
}

function PrefixInteger(num, length) {
  if (num.toString().length > length)
    throw new Error('輸入數值已超出設定位數 !!')
  return (Array(length).join('0') + num).slice(-length);
}

function compressedImage(dataURL, maxLength, callback) {
  const img = document.createElement('img')
  img.onload = function () {
    const canvas = document.createElement("canvas")
    const context = canvas.getContext("2d")
    let width, height, ratio
    if (this.naturalWidth > this.naturalHeight) {
      width = this.naturalWidth > maxLength ? maxLength : this.naturalWidth
      ratio = width / this.naturalWidth
      height = this.naturalHeight * ratio
    } else {
      height = this.naturalHeight > maxLength ? maxLength : this.naturalHeight
      ratio = height / this.naturalHeight
      width = this.naturalWidth * ratio
    }
    canvas.width = width
    canvas.height = height
    context.drawImage(this, 0, 0, width, height)
    const _dataURL = canvas.toDataURL()
    callback(_dataURL)
  }
  img.src = dataURL
}

function resizeImage(dataURL, width, height, callback) {
  const img = document.createElement('img')
  img.onload = function () {
    const canvas = document.createElement("canvas")
    const context = canvas.getContext("2d")
    canvas.width = width
    canvas.height = height
    context.drawImage(this, 0, 0, width, height)
    result = canvas.toDataURL()
    callback(result)
  }
  img.src = dataURL
}

function checkEmail(strEmail) {
  const emailRule = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/;
  return strEmail.search(emailRule) != -1 ? true : false
}