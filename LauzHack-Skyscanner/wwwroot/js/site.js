$(document).ready(function () {

    // Datetime pickers
    $("#DepartureDate").datepicker({dateFormat: "yy-mm-dd"});
    $("#ReturnDate").datepicker({dateFormat: "yy-mm-dd"});

    // Autocomplete for departure destination
    var availableDestinations = [];
    $("#Origin").autocomplete({
      source: "/Home/Autocomplete/" + $("#Origin").val(),
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

            var friend = {
                serialNo: serialNo, 
                name: $("#Name").val(), 
                origin: $("#Origin").val(),
                isReturn: isReturn,
                departureDate: $("#DepartureDate").val(),
                returnDate: $("#ReturnDate").val()
            };

            friendList.push(friend);
            repopulateFriendListTable();

            $("#showFriendForm").show();
            $("#addFriendFormDiv").hide();

        }
    });

    $("#getDestination").click();
});