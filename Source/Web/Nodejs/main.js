$(function () {
    var FADE_TIME = 150; // ms
    var TYPING_TIMER_LENGTH = 400; // ms
    var COLORS = [
      '#e21400', '#91580f', '#f8a700', '#f78b00',
      '#58dc00', '#287b00', '#a8f07a', '#4ae8c4',
      '#3b88eb', '#3824aa', '#a700ff', '#d300e7'
    ];

    // Initialize varibles
    var $window = $(window);
    var $usernameInput = $('.usernameInput'); // Input for username
    var $messages = $('.messages'); // Messages area
    var $inputMessage = $('.inputMessage'); // Input message input box

    var $loginPage = $('.login.page'); // The login page
    var $chatPage = $('.chat.page'); // The chatroom page
    var $item_user = $(".item-user");//the username in list
    // Prompt for setting a username
    var username = $("#globalUserName").val();    
    var fullname = $("#globalFullName").val();
    var cosoId = $("#globalCoSoID").val();
    var connected = false;
    var typing = false;
    var lastTypingTime;
    var $currentInput = $usernameInput.focus();

    var socket = io.connect("http://192.168.1.23:3000");
    //var socket = io("http://125.212.229.30:3000");

    $window.load(function () {
        //check xem username đã có trong socket server hay chưa
        //Nếu chưa có thì server sẽ trả về client rồi client gọi hàm call login to server để cho user login
        checkExistOnServer();

    });

    function addParticipantsMessage(data) {
        var message = '';
        message += "Hiện tại có " + data.numUsers + " người trực tuyến";
        log(message);
    }
    //function Logout()
    //{
    //    logOutFromServer();
    //    window.location = "/account/logout";
    //}
    function addParticipantsNotify(data) {
        if (!("Notification" in window)) {
            //Thông báo hiển thị ra và sẽ tự tắt đi
            Lobibox.notify('info', {
                size: 'mini',
                width: 210,
                title: 'TU-Quảng Trị',
                msg: data.fullname + ' vừa đăng nhập.'
            });
        }
        else {
            ShowDesktopNotify(data.fullname + ' vừa đăng nhập.');
        }
        //Thông báo hiển thị ra, và đc tắt đi khi lick chuột
        //Lobibox.notify('info', {
        //    delay: false,
        //    width: 80,
        //    title: 'eAita thông báo',
        //    msg: data.username + ' vừa đăng nhập.'
        //});
        // Let's check if the browser supports notifications
    }

    function removeParticipantsNotify(data) {
        if (!("Notification" in window)) {
            //Thông báo hiển thị ra và sẽ tự tắt đi
            Lobibox.notify('info', {
                size: 'mini',
                width: 210,
                title: 'TU-Quảng Trị',
                msg: data.fullname + ' vừa đăng xuất.'
            });
        }
        else {
            ShowDesktopNotify(data.fullname + ' vừa đăng xuất.');
        }
        //Thông báo hiển thị ra, và đc tắt đi khi lick chuột
        //Lobibox.notify('info', {
        //    delay: false,
        //    width: 80,
        //    title: 'eAita thông báo',
        //    msg: data.username + ' vừa đăng nhập.'
        //});
    }

    // Sets the client's username
    function setUsername() {
        // If the username is valid
        //if (username) {
        //$loginPage.fadeOut();
        //$chatPage.show();
        //$loginPage.off('click');
        //$currentInput = $inputMessage.focus();

        // Tell the server your username
        alert("F1");
        socket.emit('add user', { UserName: username, FullName: fullname, CosoID: cosoId });
        //}
    }

    function logOutFromServer() {
        socket.emit('log out', { UserName: username, CosoID: cosoId, FullName: fullname });
    }

    function checkExistOnServer() {
        socket.emit('check user exist', { UserName: username, FullName: fullname, CosoID: cosoId });
    }
    //Hiển thị danh sách toàn bộ user đang online
    function showAllUser(data) {
        var list_user = $(".list-user");
        list_user.html("");
        var users = getAllUserByCoSoID(data, cosoId);
        for (var i = 0; i < users.length; i++) {
            if (username != users[i].username) {
                var htmlAppend = "<li>";
                htmlAppend += "<a href=javascript:chatToUser('" + users[i].username + "') class='item-user' data-id='" + users[i].username + "'>";
                htmlAppend += "<div class='item img'>";
                htmlAppend += "<img class='item' src='/Uploads/Images/AvatarDemo/chat4.jpg' width='30' height='30' />";
                htmlAppend += "</div>";
                htmlAppend += "<div class='item text'>";
                htmlAppend += "<span data-id='" + users[i].username + "'>" + users[i].fullname + "</span>";
                htmlAppend += "</div>";
                htmlAppend += "<div class='item status-online'><span class='is_online'></span></div>";
                htmlAppend += "</a>";
                htmlAppend += "</li>";
                list_user.append(htmlAppend);
                //list_user.append("<li><span onclick=javascript:chatToUser('" + users[i].username + "') class='item-user' style='cursor: pointer;' data-id='" + users[i].username + "'>" + users[i].fullname + "</span></li>");
            }
        }
    }
    ////Hiển thị danh sách toàn bộ user đang online và các cửa sổ chat
    function showAllChat(chats) {
        var yourChats = getAllChatByCoSoID(chats, cosoId, username);
        soCuaSoChat = yourChats.length;
        for (var i = 0; i < yourChats.length; i++) {
            var data = {
                cosoId: yourChats[i].cosoId,
                fromUser: yourChats[i].fromUser,
                toUser: yourChats[i].toUser,
                fromFullName: yourChats[i].fromFullName,
                toFullName: yourChats[i].toFullName,
                soCuaSoChat: i,
                reload: 1,
                chat_id: yourChats[i].chatId,
                index: yourChats[i].index
            };
            $.ajax({
                type: "POST",
                url: '/Common/Chat',
                cache: false,
                data: data,
                success: function (data) {
                    $("#pnl-chat").append(data);
                }
            });
        }
    }
    ///
    ////Hiển thị danh sách toàn bộ các cửa sổ chat nhóm của user
    function showAllGroupChat(yourGroupChats) {
        var _soCuaSoChat = soCuaSoChat;
        soCuaSoChat += yourGroupChats.length;
        for (var i = 0; i < yourGroupChats.length; i++) {
            var data = {
                cosoId: yourGroupChats[i].cosoId,
                createdUser: yourGroupChats[i].createdUser,
                groupId: yourGroupChats[i].groupId,
                currentUserName: username,
                soCuaSoChat: _soCuaSoChat + i,
                reload: 1,
                groupChat_id: yourGroupChats[i].groupChatId,
            };
            $.ajax({
                type: "POST",
                url: '/Common/ChatGroup',
                cache: false,
                data: data,
                success: function (dataAppend) {
                    $("#pnl-chat").append(dataAppend);
                }
            });
        }
    }

    //chat với 1 user khác trên hệ thống
    //$(".item-user").click(function () {
    //    alert("chat nao");
    //    //socket.emit('chat with other user', $(this).attr("data-id"));
    //});
    // Sends a chat message
    function sendMessage() {
        var message = $inputMessage.val();
        // Prevent markup from being injected into the message
        message = cleanInput(message);
        // if there is a non-empty message and a socket connection
        if (message && connected) {
            $inputMessage.val('');
            addChatMessage({
                username: username,
                message: message
            });
            // tell server to execute 'new message' and send along one parameter
            socket.emit('new message', message);
        }
    }

    // Log a message
    function log(message, options) {
        var $el = $('<li>').addClass('log').html(message);
        addMessageElement($el, options);
    }

    // Adds the visual chat message to the message list
    function addChatMessage(data, options) {
        // Don't fade the message in if there is an 'X was typing'
        var $typingMessages = getTypingMessages(data);
        options = options || {};
        if ($typingMessages.length !== 0) {
            options.fade = false;
            $typingMessages.remove();
        }

        var $usernameDiv = $('<span class="username"/>')
          .text(data.username)
          .css('color', getUsernameColor(data.username));
        var $messageBodyDiv = $('<span class="messageBody">')
          .text(data.message);

        var typingClass = data.typing ? 'typing' : '';
        var $messageDiv = $('<li class="message"/>')
          .data('username', data.username)
          .addClass(typingClass)
          .append($usernameDiv, $messageBodyDiv);

        addMessageElement($messageDiv, options);
    }

    // Adds the visual chat typing message
    function addChatTyping(data) {
        data.typing = true;
        data.message = 'đang gõ ...';
        addChatMessage(data);
    }

    // Removes the visual chat typing message
    function removeChatTyping(data) {
        getTypingMessages(data).fadeOut(function () {
            $(this).remove();
        });
    }

    // Adds a message element to the messages and scrolls to the bottom
    // el - The element to add as a message
    // options.fade - If the element should fade-in (default = true)
    // options.prepend - If the element should prepend
    //   all other messages (default = false)
    function addMessageElement(el, options) {
        var $el = $(el);

        // Setup default options
        if (!options) {
            options = {};
        }
        if (typeof options.fade === 'undefined') {
            options.fade = true;
        }
        if (typeof options.prepend === 'undefined') {
            options.prepend = false;
        }

        // Apply options
        if (options.fade) {
            $el.hide().fadeIn(FADE_TIME);
        }
        if (options.prepend) {
            $messages.prepend($el);
        } else {
            $messages.append($el);
        }
        $messages[0].scrollTop = $messages[0].scrollHeight;
    }

    // Prevents input from having injected markup
    function cleanInput(input) {
        return $('<div/>').text(input).text();
    }

    // Updates the typing event
    function updateTyping() {
        if (connected) {
            if (!typing) {
                typing = true;
                socket.emit('typing', username);
            }
            lastTypingTime = (new Date()).getTime();

            setTimeout(function () {
                var typingTimer = (new Date()).getTime();
                var timeDiff = typingTimer - lastTypingTime;
                if (timeDiff >= TYPING_TIMER_LENGTH && typing) {
                    socket.emit('stop typing', username);
                    typing = false;
                }
            }, TYPING_TIMER_LENGTH);
        }
    }

    // Gets the 'X is typing' messages of a user
    function getTypingMessages(data) {
        return $('.typing.message').filter(function (i) {
            return $(this).data('username') === data.username;
        });
    }

    // Gets the color of a username through our hash function
    function getUsernameColor(username) {
        // Compute hash code
        var hash = 7;
        for (var i = 0; i < username.length; i++) {
            hash = username.charCodeAt(i) + (hash << 5) - hash;
        }
        // Calculate color
        var index = Math.abs(hash % COLORS.length);
        return COLORS[index];
    }

    // Keyboard events
    //$window.mousedown(function (event) {
    //    // Auto-focus the current input when a key is typed
    //    if (!(event.ctrlKey || event.metaKey || event.altKey)) {
    //        $currentInput.focus();
    //    }
    //    // When the client hits ENTER on their keyboard
    //    if (event.which === 13) {
    //        if (username) {
    //            sendMessage();
    //            socket.emit('stop typing');
    //            typing = false;
    //        } else {
    //            setUsername();
    //        }
    //    }
    //});




    $inputMessage.on('input', function () {
        updateTyping();
    });

    // Click events

    // Focus input when clicking anywhere on login page
    $loginPage.click(function () {
        $currentInput.focus();
    });

    //Chat with an other user anywhere on $item_user click
    //$item_user.click(function () {
    //    alert("ok-chat thoi");
    //    alert("Thằng :" + data.fromUser + " đòi chat với chú đấy!");
    //    if ($("#globalUserName").val() === data.toUser) {
    //        alert("Thằng :" + data.fromUser + " đòi chat với chú đấy!");
    //    }
    //});

    // Focus input when clicking on the message input's border
    $inputMessage.click(function () {
        $inputMessage.focus();
    });

    // Socket events
    //socket.on('call login to server', function (data) {
    //    alert(username + " cho user login: " + data.username);
    //    //Nếu đúng user thì cho đăng nhập
    //    if (username == data.username) {
    //        setUsername();
    //    }
    //});
    // Whenever the server emits 'login', log the login message
    socket.on('login', function (data) {
        connected = true;
        // Display the welcome message
        //var message = "Chào mừng <b>" + data.yourFullName + "</b>, chúc bạn có một ngày làm việc hiệu quả.";
        //log(message, {
        //    prepend: true
        //});
        //addParticipantsMessage(data);
        if (cosoId === data.cosoId) {
            //TODO: lấy những user có cùng CosoID

            showAllUser(data.listUsers);
        }
    });

    // Whenever the server emits 'new message', update the chat body
    socket.on('new message', function (data) {
        addChatMessage(data);
    });

    // Whenever the server emits 'user joined', log it in the chat body
    socket.on('user joined', function (data) {
        //log(data.username + ' joined');
        //addParticipantsMessage(data);
        if (cosoId === data.cosoId) {
            addParticipantsNotify(data);
            //TODO: lấy những user có cùng CosoID
            showAllUser(data.listUsers);
        }
    });

    //Hiển thị danh sách user
    socket.on('show list user', function (data) {
        //Kiểm tra đúng user vừa request
        if (username == data.username && cosoId == data.cosoId) {
            showAllUser(data.listUsers);
            //Nếu không ở chuyên trang chat
            if (isChatPanel == false) {
                showAllChat(data.listChats);
                showAllGroupChat(data.listGroupChats);
            }
        }
    });


    // Whenever the server emits 'user left', log it in the chat body
    socket.on('user left', function (data) {
        //log(data.username + ' left');
        //addParticipantsMessage(data);
        if (cosoId === data.cosoId) {
            removeParticipantsNotify(data);
            removeChatTyping(data);
            //TODO: lấy những user có cùng CosoID
            showAllUser(data.listUsers);
        }
    });

    // Whenever the server emits 'typing', show the typing message
    socket.on('typing', function (data) {
        addChatTyping(data);
    });

    // Whenever the server emits 'stop typing', kill the typing message
    socket.on('stop typing', function (data) {
        removeChatTyping(data);
    });

    function getAllUserByCoSoID(users, cosoId) {
        //return users;
        var result = [];
        users.forEach(function (o) { if (o.cosoId == cosoId) result.push(o); });
        return result ? result : null;

    }

    function getAllChatByCoSoID(chats, cosoId, username) {

        var result = [];

        chats.forEach(function (o) {
            if (((o.fromUser === username && o.fromUserJoin === 1) || (o.toUser === username && o.toUserJoin == 1))) {
                result.push(o);
            }
        });

        return result ? result : null;

    }

    function ShowDesktopNotify(content) {
        // Let's check if the user is okay to get some notification
        if (Notification.permission === "granted") {
            // If it's okay let's create a notification
            var options = {
                body: content,
                icon: "/Content/logo.png",
                dir: "ltr"
            };
            var notification = new Notification("TU-Quảng Trị", options);
        }
            // Otherwise, we need to ask the user for permission
            // Note, Chrome does not implement the permission static property
            // So we have to check for NOT 'denied' instead of 'default'
        else if (Notification.permission !== 'denied') {
            Notification.requestPermission(function (permission) {
                // Whatever the user answers, we make sure we store the information
                if (!('permission' in Notification)) {
                    Notification.permission = permission;
                }

                // If the user is okay, let's create a notification
                if (permission === "granted") {
                    var options = {
                        body: content,
                        icon: "/Content/logo.png",
                        dir: "ltr"
                    };
                    var notification = new Notification("TU-Quảng Trị", options);
                }
            });
        }
        else {
            Lobibox.notify('info', {
                size: 'mini',
                width: 210,
                title: 'TU-Quảng Trị',
                msg: content
            });
        }
    }
});
