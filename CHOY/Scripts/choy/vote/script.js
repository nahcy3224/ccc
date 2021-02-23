class VotingSystrm {
  constructor(signalR_hub) {
    this.projectId = getUrlParameter('ProjectID')
      ? getUrlParameter('ProjectID') : "P0001" // 暫時性內容，開發完成後請刪除
    this.socket = signalR_hub
    this.data = null
    this.onlineUsers = null

    this.socket.client.getVote = (voteId) => {
      const storage = JSON.parse(localStorage.getItem('__ChoyVote__'))
      if (storage) {
        storage.push(voteId)
        localStorage.setItem('__ChoyVote__', JSON.stringify([...storage]))
      } else {
        localStorage.setItem('__ChoyVote__', JSON.stringify([voteId]))
      }
    }

    this.socket.client.updateOnlineUsersList = (data) => {
        this.onlineUsers = data
        console.log(data)
    }

    this.socket.client.updateVoting = (callback) => {
      const fn = eval(callback)
      this.updateVotesData(fn)
    }

    $.connection.hub.start().done(() => {
      this.socket.server.userConnected(this.projectId, getMemberID()) // 加入特定房間，並通知更新
    })
  }
  handleOnbeforeunload = (callback) => {
    window.onbeforeunload = () => {
      const storage = JSON.parse(localStorage.getItem('__ChoyVote__'))
      if (storage) {
        this.socket.server.handleOnbeforeunload(this.projectId, storage, callback.toString())
      }
      localStorage.removeItem('__ChoyVote__')
    }
  }
  updateVotesData = (callback = null) => { // 取得在該專案發起的所有投票資料
    const api = `/api/project/${this.projectId}/votes`
    const successCallback = (response) => {
      this.data = response.Data
      if (callback)
        callback(this.data)
    }
    const errorCallback = (response) => {
      const httpStatusCode = response.status
      const result = response.responseJSON
      switch (httpStatusCode) {
        case 404:
          console.warn(result.Message)
          break
        default:
          console.warn(result.Message)
          alert("存取投票資料發生錯誤 !!")
          break
      }
    }
    response(api, "GET", successCallback, errorCallback)
  }
  createVote = (voteName, choices, callback = null, notificationUpdateCallback = null) => {
    this.updateOnlineUsersList()
    const api = `/api/project/${this.projectId}/vote`
    const successCallback = (response) => {
      const voteId = response.Data

      const storage = JSON.parse(localStorage.getItem('__ChoyVote__'))
      if (storage) {
        storage.push(voteId)
        localStorage.setItem('__ChoyVote__', JSON.stringify([...storage]))
      } else {
        localStorage.setItem('__ChoyVote__', JSON.stringify([voteId]))
      }
      this.socket.server.sendVote(this.projectId, voteId)

      console.log(response.Message);
      if (callback)
        this.updateVotesData(callback)
      if (notificationUpdateCallback)
        this.notificationUpdateVoting(notificationUpdateCallback)
    }
    const errorCallback = (response) => {
      const httpStatusCode = response.status
      const result = response.responseJSON
      switch (httpStatusCode) {
        case 404:
        // console.warn(result.Message)
        // break
        default:
          console.warn(result.Message)
          alert("投票建立失敗")
          break
      }
    }
    const data = {
      "VoteName": voteName,
      "Choices": [...choices],
      "VoteCount": this.getOnlineUsersList().length
    }
    response(api, "Post", successCallback, errorCallback, data)
  }

  vote = (voted, choiceId, callback = null, notificationUpdateCallback = null) => {
    const storage = JSON.parse(localStorage.getItem('__ChoyVote__'))
    if (!isInArray(storage, voted))
      return;
    const api = `/api/vote/${voted}/choice/${choiceId}`
    const successCallback = (response) => {
      console.log(response.Message)
      const newStorage = storage.filter(item => item !== voted)
      localStorage.setItem('__ChoyVote__', JSON.stringify([...newStorage]))
      if (callback)
        this.updateVotesData(callback)
      if (notificationUpdateCallback)
        this.notificationUpdateVoting(notificationUpdateCallback)

    }
    const errorCallback = (response) => {
      const httpStatusCode = response.status
      const result = response.responseJSON
      switch (httpStatusCode) {
        case 404:
        // console.warn(result.Message)
        // break
        default:
          console.warn(result.Message)
          alert("投票執行失敗 !!")
          break
      }
    }
    response(api, "Patch", successCallback, errorCallback)
  }
  getVotesData = () => this.data

  notificationUpdateVoting = (callback) => {
    this.socket.server.notificationUpdateVoting(this.projectId, callback.toString())
  }
  updateOnlineUsersList = () => { // 更新自己的線上名單
    this.socket.server.getOnlineUsersList(this.projectId)
  }
  getOnlineUsersList = () => this.onlineUsers.map(user => user.MemberID)
}



const votingSystrm = new VotingSystrm($.connection.voteHub)

votingSystrm.updateVotesData(render)
votingSystrm.handleOnbeforeunload((data) => {
  votingSystrm.updateVotesData(render(data))
})
let i = 2
document.querySelectorAll('[data-choy-function]').forEach(item => {
  const func = item.dataset.choyFunction
  switch (func) {
    case 'add-vote-item':
      item.addEventListener('click', () => {
        i += 1
        const votingItems = document.getElementById('voting-items')
        votingItems.innerHTML += `
          <div class="form-group">
            <label for="choice-${i}">Option ${i}</label>
            <input type="text" class="form-control" id="choice-${i}" name="choice" />
          </div>`
      })
      break
    case 'create-vote':
      item.addEventListener('click', () => {
        const createVoteForm = document.getElementById('create-vote-form')
        const voteName = document.getElementById('voteName').value
        const choices = [...createVoteForm.querySelectorAll('input[name="choice"]')]
          .filter(item => item.value !== "")
          .map(item => item.value)
        if (voteName === '' || choices.length < 2) {
          alert('至少要填寫問題與兩個選項!!')
          return;
        }
        // 第4個參數只能使用 匿名箭頭韓式
        votingSystrm.createVote(voteName, choices, handleCreateVote, (data) => {
          votingSystrm.updateVotesData(render(data))
        })
      })
  }
})

function handleCreateVote(data) {
  render(data)
  document.getElementById('voteName').value = ''
  document.getElementById('choice-1').value = ''
  document.getElementById('choice-2').value = ''
  document.getElementById('voting-items').innerHTML = ''
  i = 2
}

function render(data) {
  const votedList = data.filter(vote => vote.Result !== null)
  const voted = document.getElementById('voted')
  voted.innerHTML = ''
  votedList.reverse().forEach(vote => {
    voted.innerHTML += `
    <div class="form-group">
      <h3>${vote.VoteName}</h3>
      <p>${vote.Result}<p/>
    </div>`
  })
  const voting = document.getElementById('voting')
  const storage = JSON.parse(localStorage.getItem('__ChoyVote__'))

  const votingList = storage 
    ? data.filter(vote => storage.indexOf(vote.VoteID) !== -1)
    : []

  voting.innerHTML = ''
  for (let vote of votingList) {
    let choices = ''
    for (let choice of vote.Choices) {
      let choiceId = `vote-choice-${choice.ChoiceID}`
      choices += `
        <div>
          <input class=""
            id="${choiceId}"
            type="radio"
            name="choice"
            value="${choice.ChoiceID}"
            /> 
            <label for="${choiceId}">${choice.Choice}</label>
        </div>`
    }
    let formId = `vote-form-${vote.VoteID}`
    voting.innerHTML += `
    <div class="each-vote">
      <h3 class="font-small">${vote.VoteName}</h3>
      <form id="${formId}" onsubmit="return false;">
        ${choices}
        <input type="submit" value="Submit" class="btn"onclick="vote('${formId}')"/>
      </form>
    <hr/>
    </div>`
  }
}

function vote(formId) {
  const form = document.getElementById(formId)
  const checked = [...form.querySelectorAll('input[type="radio"]')].find(item => {
    return item.checked
  }) ?? null

  if (checked) {
    const voteId = formId.split("-").pop()
    const choiceId = checked.id.split("-").pop()
    votingSystrm.vote(Number(voteId), Number(choiceId), render, (data) => {
      votingSystrm.updateVotesData(render(data))
    })
  }
}



