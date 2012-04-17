	var mainUpdateInterval, authUpdateInterval, dataSource = "data.json";
	var Loggedin = false;
	
	$(function(){
		$('#tabs').tabs();	
		$("#InputBar").keypress( function(event) { CheckKeys(event, 0) } );
		$("#UserBar").keypress( function(event) { CheckKeys(event, 1) } );
		$("#PassBar") .keypress( function(event) { CheckKeys(event, 1) } );
		
		disableTabs();
		
		ReceiveAuth(true, false);		
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
		$("#tabs").tabs('select', 5);
		$("#tabs").tabs({disabled: []});
		
		$("#tabs").tabs('select', 0);
	}
	
	function disableTabs() {
		$("#tabs").tabs('select', 5);
		$("#tabs").tabs({disabled: [1, 2, 3, 4]});
		
		$("#tabs").tabs('select', 0);
	}
	
	function startUpdates() {
		ReceiveConfig();
		
		mainUpdateInterval = setInterval(mainUpdates, 2000); //2 seconds /check
		authUpdateInterval = setInterval(authCheck, 1000 * 30); //30 seconds /check
	}
	
	function mainUpdates() {
		ReceiveServerStats();
		ReceiveChatData();
	}
	
	function authCheck() {
		ReceiveAuth(false, true);
	}
	
	function ReceiveAuth(enable, reload) {
		var checked = false;
		$.getJSON(dataSource + "?request=auth", 		
			function (data) {
				if (data)
				{
					if(data['auth']) {
						checked = true;
						Loggedin = true;
						if(enable) {
							removeLogin();
							startUpdates();
							enableTabs();
						}
						return;
					} else {
						disableTabs();
						if(reload) {
							window.location.reload();
						}
					}
				}
				
				Loggedin = false;
				$("#LoginBtn").click(Login); //Start Login
			}		
		);
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
	
	function ReceiveUserData() {
		var Player = $(this).text();
		
		$("#UserDataForm").empty();
		
		$.getJSON(dataSource + "?request=pdata&user=" + Player, function (data) {
				if (data)
				{
					var PlayerData = data["data"];
					
					for (var i in data['data']) {
						var info = data['data'][i];
						//$("#UserDataForm").append("<div class='UserMonitorNames'>" + info + "</div");
						//$("#UserDataForm").text($("#UserDataForm").text() + "\n" + info);
						AppendUserData(info);
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
				$("#PlayerDataList").empty();

				var players = false;
				for (var i in data['userlist']) {
					name = data['userlist'][i];
					$("#UserList").append("<div class='ChatMessageNames'>" + name + "</div>");
					$("#PlayerDataList").append("<div class='UserMonitorNames'>" + name + "</div>");
					
					$(".UserMonitorNames").click(ReceiveUserData);
					players = true;
				}
				
				if(!players) {
					$("#UserList").append("<div class='ChatMessageNames'>No Players online</div>");
					$("#PlayerDataList").append("<div class='ChatMessageNames'>No Players online</div>");
				}

				if (data['ready']) {
					$("#serverStat").text("Online");
				}
			}
		});
	}

	function AppendLine(ClassName, Form, InnerHTML) {
		lineEntry = document.createElement("div");
		lineEntry.className = ClassName;		
		
		lineEntry.innerHTML = InnerHTML

		if ($("#" + Form).children("div").size() > MaxChatLines) { //Max messages
			$("#" + Form).children("div").first().remove();
		}

		chatForm = document.getElementById(Form);
		chatForm.appendChild(lineEntry);
		chatForm.scrollTop = chatForm.scrollHeight;
	}
	
	function AppendChat(Sender, Rank, Message) {
		Message = unescape(Message);

		if(Rank.length > 0) {
			Rank = "[" + Rank + "]";
		}
		
		var innerHTML = "<span class='ChatMessageNames'>" + Rank + Sender + "</span>:&nbsp;" + Message;
		AppendLine("ChatMessage", "ChatForm", innerHTML);
	}
	
	var lastVariable;
	function AppendUserData(Message) {
		var split = Message.split(":", 2);
		
		var variable = split[0];
		if(lastVariable == variable) {
			variable = "";
		}
		
		var innerHTML = "<span class='ChatMessageNames'>" + variable + "</span>:&nbsp;" + split[1];
		AppendLine("ChatMessage", "UserDataForm", innerHTML);
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