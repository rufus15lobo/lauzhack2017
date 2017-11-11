$(document).ready(function () {

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
            friendTableHtml += "<td>" + friendList[i].isReturn + "</td>";
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
            alert(serialNo);
            var friend = {
                serialNo: serialNo, 
                name: $("#Name").val(), 
                origin: $("#Origin").val(),
                isReturn: $("#IsReturn").val(),
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