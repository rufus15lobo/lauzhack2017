$(document).ready(function () {

    // Datetime pickers
    $("#DepartureDate").datepicker({dateFormat: "yy-mm-dd"});
    $("#ReturnDate").datepicker({dateFormat: "yy-mm-dd"});

    // Visibility
    $("#getFlights").hide();
    $("#getConnections").hide();
    $("#addFriendFormDiv").hide();
    $("#showFriendForm").show();
    $("#friendList").show();

    // Autocomplete for departure destination
    var availableDestinations = [];
    var selectedOrigin = "";
    $("#Origin").autocomplete({
      source: "/Home/Autocomplete/" + $("#Origin").val(),
      minLength: 2,
      select: function( event, ui ) {
          selectedOrigin = ui.item.id;
      }
    });
    var selectedDestination = "";
    $("#Destination").autocomplete({
      source: "/Home/Autocomplete/" + $("#Destination").val(),
      minLength: 2,
      select: function( event, ui ) {
          selectedDestination = ui.item.id;
      }
    });

    //TODO: Get destination visibility
    var friendList = [];
    var flightList = [];
    var connectionList = [];

    function validateFriendDetails() {
        return true;
    }

    function repopulateFriendListTable() {
        var friendTableHtml = "";

        for (var i=0; i<friendList.length; i++) {
            friendTableHtml += "<tr>";
            friendTableHtml += "<td>" + friendList[i].SerialNo + "</td>";
            friendTableHtml += "<td>" + friendList[i].Name + "</td>";
            friendTableHtml += "<td>" + friendList[i].Origin + "</td>";
            friendTableHtml += "<td>" + friendList[i].Destination + "</td>";
            if (friendList[i].IsReturnJourney) {
                friendTableHtml += "<td><input type='checkbox' disabled checked></td>";
            } else {
                friendTableHtml += "<td><input type='checkbox' disabled></td>";          
            }
            friendTableHtml += "<td>" + friendList[i].DepartureDate + "</td>";
            friendTableHtml += "<td>" + friendList[i].ReturnDate + "</td>";
            friendTableHtml += "</tr>";
        }

        $("#friendList > tbody").html(friendTableHtml);
    }

    function repopulateFlightListTable() {
        var flightTableHtml = "";

        for (var i = 0; i < flightList.length; i++) {
            flightTableHtml += "<tr>";
            flightTableHtml += "<td>" + flightList[i].friendName + "</td>";
            flightTableHtml += "<td>" + flightList[i].origin + "</td>";
            flightTableHtml += "<td>" + flightList[i].destination + "</td>";
            flightTableHtml += "<td>" + flightList[i].price + "</td>";
            flightTableHtml += "<td><a href='" + flightList[i].deepLink + "' target='_blank'>View offer</a></td>";
            flightTableHtml += "</tr>";
        }

        $("#getFlights > tbody").html(flightTableHtml);
    }

    function repopulateConnectionsListTable() {
        var connectionTableHtml = "";

        for (var i = 0; i < connectionList.length; i++) {
            connectionTableHtml += "<tr>";
            connectionTableHtml += "<td>" + connectionList[i].friendName + "</td>";
            connectionTableHtml += "<td>" + connectionList[i].origin + "</td>";
            connectionTableHtml += "<td>" + connectionList[i].destination + "</td>";
            connectionTableHtml += "<td>" + connectionList[i].price + "</td>";
            connectionTableHtml += "<td><a href='" + connectionList[i].deepLink + "' target='_blank'>View offer</a></td>";
            connectionTableHtml += "</tr>";
        }

        $("#getConnections > tbody").html(connectionTableHtml);
    }

    $("#addFriendFormDiv").click();

    $("#showFriendForm").click(function() {
        $("#showFriendForm").hide();
        $("#addFriendFormDiv").show();
    });

    $("#addFriendToList").click(function() {
        if(validateFriendDetails()) {
 
            var serialNo = friendList.length + 1;
            var isReturn = $("#IsReturn").is(":checked");
            var originValue = $("#Origin").val();
            if (selectedOrigin != "")
            {
                originValue = selectedOrigin;
            }
            var destinationValue = $("#Destination").val();
            if (selectedDestination != "")
            {
                destinationValue = selectedDestination;
            }

            var friend = {
                SerialNo: serialNo, 
                Name: $("#Name").val(), 
                Origin: $("#Origin").val(),
                Destination: $("#Destination").val(),
                IsReturnJourney: isReturn,
                DepartureDate: $("#DepartureDate").val(),
                ReturnDate: $("#ReturnDate").val(),
                OriginId: originValue,
                DestinationId: destinationValue
            };

            friendList.push(friend);
            repopulateFriendListTable();

            $("#showFriendForm").show();
            $("#addFriendFormDiv").hide();

        }
    });

    $("#getDestinationList").click(function() {
        //var friendListJSON = JSON.stringify(friendList);

        var friendListJSON = "[";
        for (var i=0; i<friendList.length; i++) {
            if (i>0) 
            {
                friendListJSON += ",";
            }
            friendListJSON += "{";
            friendListJSON += "\"SerialNo\": \"" + friendList[i].SerialNo + "\", ";
            friendListJSON += "\"Name\": \"" + friendList[i].Name + "\", ";
            friendListJSON += "\"Origin\": \"" + friendList[i].Origin + "\", ";
            friendListJSON += "\"Destination\": \"" + friendList[i].Destination + "\", ";
            friendListJSON += "\"IsReturnJourney\": " + friendList[i].IsReturnJourney + ", ";
            friendListJSON += "\"DepartureDate\": \"" + friendList[i].DepartureDate + "\", ";
            friendListJSON += "\"ReturnDate\": \"" + friendList[i].ReturnDate + "\", ";
            friendListJSON += "\"OriginId\": \"" + friendList[i].OriginId + "\", ";
            friendListJSON += "\"DestinationId\": \"" + friendList[i].DestinationId + "\"";
            friendListJSON += "}";
        }
        friendListJSON += "]";

        $.ajax({
            method: "GET",
            url: "/Home/Flights",
            data: { friendList: friendListJSON },
            success: function (data) {
                $("#getFlights").show();
                flightList = JSON.parse(data);
                repopulateFlightListTable();
            },
            dataType: "html"
        });
    });

    $("#getConnectionList").click(function() {
        //var friendListJSON = JSON.stringify(friendList);

        var friendListJSON = "[";
        for (var i=0; i<friendList.length; i++) {
            if (i>0) 
            {
                friendListJSON += ",";
            }
            friendListJSON += "{";
            friendListJSON += "\"SerialNo\": \"" + friendList[i].SerialNo + "\", ";
            friendListJSON += "\"Name\": \"" + friendList[i].Name + "\", ";
            friendListJSON += "\"Origin\": \"" + friendList[i].Origin + "\", ";
            friendListJSON += "\"Destination\": \"" + friendList[i].Destination + "\", ";
            friendListJSON += "\"IsReturnJourney\": " + friendList[i].IsReturnJourney + ", ";
            friendListJSON += "\"DepartureDate\": \"" + friendList[i].DepartureDate + "\", ";
            friendListJSON += "\"ReturnDate\": \"" + friendList[i].ReturnDate + "\", ";
            friendListJSON += "\"OriginId\": \"" + friendList[i].OriginId + "\", ";
            friendListJSON += "\"DestinationId\": \"" + friendList[i].DestinationId + "\"";
            friendListJSON += "}";
        }
        friendListJSON += "]";

        $.ajax({
            method: "GET",
            url: "/Home/Connections",
            data: { friendList: friendListJSON },
            success: function (data) {
                $("#getConnections").show();
                connectionList = JSON.parse(data);
                repopulateConnectionsListTable();
            },
            dataType: "html"
        });
    });

});