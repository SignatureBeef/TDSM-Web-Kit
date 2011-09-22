	var mainUpdateInterval, dataSource = "data.json";
	var Loggedin = false;
	
	$(function(){
		$('#tabs').tabs();	
		$("#InputBar").keypress( function(event) { CheckKeys(event, 0) } );
		$("#UserBar").keypress( function(event) { CheckKeys(event, 1) } );
		$("#PassBar") .keypress( function(event) { CheckKeys(event, 1) } );
		
		disableTabs();
		
		ReceiveAuth();		
	});
	
	function CheckKeys(event, Type) {
		if (event.keyCode == '13') {
			switch(Type) {
				case 0: {
					event.preventDefault();
					message = escape($("#InputBar").attr('value'));
					
					if (message[0] == "/")
					{
						$.getJSON(dataSource + "?request=webcommand&command=" + message.substring(1, message.length));
						AppendChat("Server Command", "", message);
					} else
					{
						$.getJSON(dataSource + "?request=webchat&message=" + message);
						AppendChat("Server Message", "", message);
					}
					
					$("#InputBar").attr('value', "");
					break;
				}
				case 1: {
					$("#LoginBtn").click();
					break;
				}
			}
			
		}
	}
	
	function ReceiveConfig() {
		$.getJSON(dataSource + "?request=config", function (data) {		
			if(data['maxLines'] > 0) {
				MaxChatLines = data['maxLines'];
			}			
		});
	}
	
	function Login() {		
		var username = $("#UserBar").val();
		var password = $("#PassBar").val();
		
		VerifyPassword(username, password);
	}
	
	function removeLogin() {
		$('#tabs').tabs('remove', 0);		
		$("#tabs").tabs('select', 0);
	}
	
	function enableTabs() {
		$("#tabs").tabs('select', 4);
		$("#tabs").tabs({disabled: []});
		
		$("#tabs").tabs('select', 0);
	}
	
	function disableTabs() {
		$("#tabs").tabs('select', 4);
		$("#tabs").tabs({disabled: [1, 2, 3]});
		
		$("#tabs").tabs('select', 0);
	}
	
	function startUpdates() {
		ReceiveConfig();
		
		mainUpdateInterval = setInterval(mainUpdates, 1500); //1.5 seconds /check
	}
	
	function mainUpdates() {
		ReceiveServerStats();
		ReceiveChatData();
	}
	
	function ReceiveAuth() {
		$.getJSON(dataSource + "?request=auth", function (data) {
			if (data)
			{
				if(data['auth']) {
					Loggedin = true;
					removeLogin();
					startUpdates();
					enableTabs();
					return;
				}
			}
			
			Loggedin = false;
			$("#LoginBtn").click(Login); //Start Login
		});
	}
	
	function VerifyPassword(User, Pass) {
		$.getJSON(dataSource + "?request=verauth&user=" + User + "&pass=" + Pass, function (data) {
			if (data)
			{
				if(data['match'] == 0) {
					Loggedin = true;
					startUpdates();
					enableTabs();
					removeLogin();
					$("#authstatus").text("Verified");
				} else if(data['match'] == 1) {
					alert("Authentication failed!\n\nUser does not exist.");
				} else if(data['match'] == 2) {
					alert("Authentication failed!\n\nIncorrect password.");
				}
			}
		});
	}
	
	function ReceiveServerStats() {
		$.getJSON(dataSource + "?request=stats", function (data) {
			if (data)
			{
				$("#onlineplayers").text(data['users'] + " / " + data['maxusers'] + " players");

				$("#cpumonitor").text(data["cpu"]);
				$("#virMem").text(data["virmem"]);					
				$("#phyMem").text(data["phymem"]);
				
				$("#UserList").empty();

				for (var i in data['userlist']) {
					name = data['userlist'][i];
					$("#UserList").append("<div class='ChatMessageNames'>" + name + "</div>");
				}

				if (data['ready']) {
					$("#serverStat").text("Online");
				}
			}
		});
	}

	function AppendChat(Sender, Rank, Message) {
		lineEntry = document.createElement("div");
		lineEntry.className = "ChatMessage";

		if(Rank.length > 0) {
			Rank = "[" + Rank + "]";
		}
		
		Message = unescape(Message);
		lineEntry.innerHTML = "<span class='ChatMessageNames'>" + Rank + Sender + "</span>:&nbsp;" + Message;

		if ($("#ChatForm").children("div").size() > MaxChatLines) { //Max messages
			$("#ChatForm").children("div").first().remove();
		}

		chatForm = document.getElementById("ChatForm");
		chatForm.appendChild(lineEntry);
		chatForm.scrollTop = chatForm.scrollHeight;
	}

	var prevStamp = "";
	function ReceiveChatData() {
		$.getJSON(dataSource + "?request=chat&since=" + prevStamp, "", function (data) {
			if (data) {
				array = data['messages'];

				for (var i in array) {
					line = array[i];

					AppendChat(line['sender'], line['rank'], line['message']);
				}
				
				prevStamp = data['timesent'];
			}
		});
	}