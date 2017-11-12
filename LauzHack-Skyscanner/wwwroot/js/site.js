$(document).ready(function () {

    // Datetime pickers
    $("#DepartureDate").datepicker({dateFormat: "yy-mm-dd"});
    $("#ReturnDate").datepicker({dateFormat: "yy-mm-dd"});

    // Autocomplete for departure destination
    var availableDestinations = [];
    var selectedOrigin = "";
    $("#Origin").autocomplete({
      source: "/Home/Autocomplete/" + $("#Origin").val(),
      minLength: 2,
      select: function( event, ui ) {
        alert("Option selected");
      }
    });
    var selectedDestination = "";
    $("#Origin").autocomplete({
      source: "/Home/Autocomplete/" + $("#Destination").val(),
      minLength: 2,
      select: function( event, ui ) {
        alert("Option selected");
      }
    });
    $("#Destination").autocomplete({
      source: "/Home/Autocomplete/" + $("#Destination").val(),
      minLength: 2,
      select: function( event, ui ) {
        alert("Option selected");
      }
    });

    //TODO: Get destination visibility
    var friendList = [];

    function validateFriendDetails() {
        return true;
    }

    function repopulateFriendListTable() {
        var friendTableHtml = "";

        for (var i=0; i<friendList.length; i++) {
            friendTableHtml += "<tr>";
            friendTableHtml += "<td>" + friendList[i].serialNo + "</td>";
            friendTableHtml += "<td>" + friendList[i].name + "</td>";
            friendTableHtml += "<td>" + friendList[i].origin + "</td>";
            friendTableHtml += "<td>" + friendList[i].destination + "</td>";
            if (friendList[i].isReturn) {
                friendTableHtml += "<td><input type='checkbox' disabled checked></td>";
            } else {
                friendTableHtml += "<td><input type='checkbox' disabled></td>";          
            }
            friendTableHtml += "<td>" + friendList[i].departureDate + "</td>";
            friendTableHtml += "<td>" + friendList[i].returnDate + "</td>";
            friendTableHtml += "</tr>";
        }

        $("#friendList > tbody").html(friendTableHtml);
    }

    $("#addFriendFormDiv").hide();
    $("#showFriendForm").show();
    $("#friendList").show();

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
                serialNo: serialNo, 
                name: $("#Name").val(), 
                origin: $("#Origin").val(),
                destination: $("#Destination").val(),
                isReturn: isReturn,
                departureDate: $("#DepartureDate").val(),
                returnDate: $("#ReturnDate").val(),
                originId: originValue,
                destinationId: destinationValue
            };

            friendList.push(friend);
            repopulateFriendListTable();

            $("#showFriendForm").show();
            $("#addFriendFormDiv").hide();

        }
    });

    $("#getDestination").click();
});