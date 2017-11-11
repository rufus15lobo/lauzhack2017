$(document).ready(function () {

    //TODO: Get destination visibility
    function validateFriendDetails() {
        return true;
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
        

        $("#showFriendForm").show();
        $("#addFriendFormDiv").hide();
    });

    $("#getDestination").click();
});