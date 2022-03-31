
$(document).ready(() => {
    $('.chat-container').hide()
    $('#chat-header').hide()
    $('#personalMessages').hide()

    //#region CONNECTION CONFIGURATION
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5188/chat")
        .build();

    connection.start();
    //#endregion

    //#region CHAT FUNCTIONS
    $('#loginButton').click(() => {
        const username = $('#username').val()
        $('.login-container').hide()
        $('.chat-container').fadeIn(2000)
        $('#chat-header').fadeIn(2000)
        connection.invoke("GetName", username).catch(err => console.error(err.toString()))
        connection.invoke("GetMessgaes").catch(err => console.error(err.toString()))
    })

    const getMessages = () => {
        connection.on("getMessages", messages => {
            $('.message-container').empty()
            messages.forEach(message => {
                $('.message-container').prepend(`<div id="message" class="message">
                <div class="message-top">
                    <span class="badge bg-info text-white">${message.name}</span>
                    <small>${message.date}</small>
                </div>
                <div class="message-content">
                    <p>${message.messageContent}</p>
                </div>
            </div>`)
            })
        })
    }

    openPersonalMessageModal = (user) => {
        $('#exampleModal').modal('show')
        $('#exampleModalLabel').text(user)
    }
    const getClients = () => {
        connection.on("getClients", clients => {
            $('#clients').empty()
            clients.forEach(client => {
                $('#clients').append(`<li onclick="openPersonalMessageModal('${client.name}')" class="list-group-item"> <small>${client.name} </small>
                <span class="online"></span></li>`)
            })
        })
    }
    getMessages()

    connection.on("getClientName", name => {
        $('#chat-header').append(`<small></small><h4>${name} Hoşgeldiniz... Keyifli Sohbetler</h4>`)
    })
    getClients()

    connection.on("clientJoined", name => {
        toastr.info(`${name} sohbete katıldı`)
    })

    //#region sendMessage
    $('#messageButton').click(() => {
        const message = $('#userMessage').val()
        connection.invoke("SendMessage", message).catch(err => console.error(err.toString()))
        $('#userMessage').val('')
    })
    //#endregion

    //#region sendPersonalMessage
    $('#personalMessagesButton').click(() => {
        $('#allMessages').hide()
        $('#personalMessages').show()
    })

    $('#closePersonalMessages').click(() => {
        $('#allMessages').show()
        $('#personalMessages').hide()
    })

    $('#sendPersonalMessage').click(() => {
        const message = $('#personalUserMessage').val()
        const user = $('#exampleModalLabel').text()
        connection.invoke("SendPersonalMessage", message, user).catch(err => console.error(err.toString()))
        $('#personalMessage').val('')
        $('#exampleModal').modal('hide')
    })

    const personalMessages = () => {
        connection.on("getPersonalMessages", messages => {
            $('.personal-messages-container').empty()
            messages.forEach(message => {
                $('.personal-messages-container').append(`<div id="personalMessage" class="message">
              <div class="message-top">
                  <span class="badge bg-info text-white font-span">${message.senderName} gönderdi ${message.recieverName} kişisine</span>
                  <small>${message.date}</small>
              </div>
              <div class="message-content">
                  <p>${message.message}</p>
              </div>
          </div>`)
            })
            $('#messageInfo').remove()
        })
    }
    personalMessages()
    //#endregion

    connection.on("userLeaved", name => {
        toastr.error(`${name} sohbetten ayrıldı`)
    })

    //#endregion
});