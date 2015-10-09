﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Make_A_Friend.index" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Make a Friend!</title>
</head>

<body>
    <!--Script references. -->
    <script src="Scripts/jquery-1.6.4.min.js"></script>
    <script src="Scripts/jquery.signalR-2.2.0.min.js"></script>
    <script src="signalr/hubs"></script>
    <link href="ChatStyleSheet.css" rel="stylesheet" />
    
    <script type="text/javascript">
        $(document).ready(function () {
            $(document.body).hide(0).fadeIn(800);

            $('#message').bind("cut copy paste", function (e) {
                e.preventDefault();
            });
        });

        // Emoticon functions
        function showemoticonpanel() {            
            var e = document.getElementById('emoticonpanel');
            if(e.style.display == 'block')
                e.style.display = 'none';
            else
                e.style.display = 'block';
        }
        function emoticonclick(i) {
            $('#message').html($('#message').html() + "<img src='Resources/Icons/" + i + ".gif'/>");
            document.getElementById('emoticonpanel').style.display = 'none';
            $('#message').html($('#message').html()).focus();
        }
        function emoticonhover(i) {
            document.getElementById("emoticonpreview").src = "Resources/Icons/" + i + ".gif";
        }

        // Pressing enter will send the message in the textbox
        function replaceEnter(e) {
            if (e.keyCode == 13) {
                var allElements = document.getElementsByTagName('*');
                for (var i = 0; i < allElements.length; i++) {
                    if (allElements[i].id.indexOf("sendmessage") != -1) {
                        allElements[i].click();
                    }
                }
                return false;
            } else {
                return true;
            }
        }
        window.onkeypress = replaceEnter;


        $(function () {
            var chat = $.connection.chatHub;
            var theirmessage = true;

            chat.client.broadcastMessage = function (name, message) {
                if (message != '' && message.search(/^&nbsp;*/) != 0) {
                    var encodedName = $('<div />').text(name).html();
                    //var encodedMsg = $('<div />').text(message).html();
                    var encodedMsg = $('<div />').html(message).html();

                    // Add the message to the page
                    if (theirmessage) {
                        $('#discussion').append('<div class="theirbubblecontainer"">'
                            + '<strong class="bubblename">'
                                + encodedName
                            + '</strong>&nbsp;&nbsp;&nbsp;&nbsp;'
                            + '<div class="theirbubble">'
                                + encodedMsg
                            + '</div>'
                        + '</div>');
                    } else {
                        $('#discussion').append($('<div class="yourbubblecontainer">'
                            + '<div class="yourbubble">'
                                + encodedMsg
                            + '</div>&nbsp;&nbsp;&nbsp;&nbsp;'
                            + '<strong class="bubblename">'
                                + encodedName
                            + '</strong>'
                        + '</div>').css({fontSize: '0px', opacity: 0.4}).animate({fontSize: '16px', opacity:1}, 350, 'swing'));
                    }

                    theirmessage = true;

                    var audio = document.getElementById("audio");
                    audio.play();

                    // Scrolls chat history down automatically until user scrolls up.
                    var scrolled = false;
                    if (!scrolled) {
                        var element = document.getElementById("chatcontainer");
                        //element.scrollTop = element.scrollHeight;
                        $("#chatcontainer").animate({ scrollTop: $("#chatcontainer").get(0).scrollHeight }, 500);
                    }

                    $("#chatcontainer").on('scroll', function () {
                        scrolled = true;
                    });
                }
            };


            // Get the user name and store it to prepend to messages.
            $('#displayname').val(prompt('Enter your name:', ''));


            $('#message').html('').focus();

            $.connection.hub.start().done(function () {
                $('#sendmessage').click(function () {
                    chat.server.send($('#displayname').val(), $('#message').html());
                    theirmessage = false;

                    $('#message').html('').focus();
                });
            });
        });
    </script>


    <audio id="audio" src="Resources/Woosh.mp3" ></audio>


    <div id="headercontainer">
        <h1 id="headerTitle">Make a Friend!</h1>
    </div>


    <div id="pagecontent">
        <div id="chatcontainer">
            <ul id="discussion"></ul>
        </div>

        <div id="textboxcontainer">
            <div contenteditable="true" id="message" data-text="Type a message here" onkeypress="return (this.innerText.length <= 150)"></div>
            <input type="button" id="sendmessage" value="Send" />
            <div id="emoticonbutton" onclick="showemoticonpanel()">
                <img src="Resources/Icons/emote.png"/>
            </div>
            <div id="emoticonpanel">
                <div style="height: 125px; width: 137px; text-align:center;">
                    <img src="Resources/Icons/1.gif" id="emoticonpreview" style="height: 75px"/>
                </div>
                <hr />
                <div style="height:135px; margin-top: 10px;">
                    <img class="emoticons" src="Resources/Icons/1.gif" onclick="emoticonclick(1)" onmouseover="emoticonhover(1)"/>
                    <img class="emoticons" src="Resources/Icons/2.gif" onclick="emoticonclick(2)" onmouseover="emoticonhover(2)"/>
                    <img class="emoticons" src="Resources/Icons/3.gif" onclick="emoticonclick(3)" onmouseover="emoticonhover(3)"/>
                    <img class="emoticons" src="Resources/Icons/4.gif" onclick="emoticonclick(4)" onmouseover="emoticonhover(4)"/>
                    <img class="emoticons" src="Resources/Icons/5.gif" onclick="emoticonclick(5)" onmouseover="emoticonhover(5)"/>
                    <img class="emoticons" src="Resources/Icons/6.gif" onclick="emoticonclick(6)" onmouseover="emoticonhover(6)"/>
                    <img class="emoticons" src="Resources/Icons/7.gif" onclick="emoticonclick(7)" onmouseover="emoticonhover(7)"/>
                    <img class="emoticons" src="Resources/Icons/8.gif" onclick="emoticonclick(8)" onmouseover="emoticonhover(8)"/>
                    <img class="emoticons" src="Resources/Icons/9.gif" onclick="emoticonclick(9)" onmouseover="emoticonhover(9)"/>
                    <img class="emoticons" src="Resources/Icons/10.gif" onclick="emoticonclick(10)" onmouseover="emoticonhover(10)"/>
                    <img class="emoticons" src="Resources/Icons/11.gif" onclick="emoticonclick(11)" onmouseover="emoticonhover(11)"/>
                    <img class="emoticons" src="Resources/Icons/12.gif" onclick="emoticonclick(12)" onmouseover="emoticonhover(12)"/>
                    <img class="emoticons" src="Resources/Icons/13.gif" onclick="emoticonclick(13)" onmouseover="emoticonhover(13)"/>
                    <img class="emoticons" src="Resources/Icons/14.gif" onclick="emoticonclick(14)" onmouseover="emoticonhover(14)"/>
                    <img class="emoticons" src="Resources/Icons/15.gif" onclick="emoticonclick(15)" onmouseover="emoticonhover(15)"/>
                    <img class="emoticons" src="Resources/Icons/16.gif" onclick="emoticonclick(16)" onmouseover="emoticonhover(16)"/>
                    <img class="emoticons" src="Resources/Icons/17.gif" onclick="emoticonclick(17)" onmouseover="emoticonhover(17)"/>
                    <img class="emoticons" src="Resources/Icons/18.gif" onclick="emoticonclick(18)" onmouseover="emoticonhover(18)"/>
                    <img class="emoticons" src="Resources/Icons/19.gif" onclick="emoticonclick(19)" onmouseover="emoticonhover(19)"/>
                    <img class="emoticons" src="Resources/Icons/20.gif" onclick="emoticonclick(20)" onmouseover="emoticonhover(20)"/>
                    <img class="emoticons" src="Resources/Icons/21.gif" onclick="emoticonclick(21)" onmouseover="emoticonhover(21)"/>
                    <img class="emoticons" src="Resources/Icons/22.gif" onclick="emoticonclick(22)" onmouseover="emoticonhover(22)"/>
                    <img class="emoticons" src="Resources/Icons/23.gif" onclick="emoticonclick(23)" onmouseover="emoticonhover(23)"/>
                    <img class="emoticons" src="Resources/Icons/24.gif" onclick="emoticonclick(24)" onmouseover="emoticonhover(24)"/>
                    <img class="emoticons" src="Resources/Icons/25.gif" onclick="emoticonclick(25)" onmouseover="emoticonhover(25)"/>
                </div>
                <div style="position: absolute; bottom: -30px; right: 28px; border-top: solid 30px #899dff; border-left: solid 35px transparent; border-right: solid 35px transparent"></div>
                <div style="position: absolute; bottom: -29px; right: 29px; border-top: solid 29px white; border-left: solid 34px transparent; border-right: solid 34px transparent"></div>
            </div>
            <input type="hidden" id="displayname" />
        </div>
    </div>

</body>
</html>
