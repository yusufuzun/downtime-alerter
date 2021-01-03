"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/alertHub").build();

connection.on("ReceiveStatus", function (id, status, lastUpdatedDate) {

    const statusObj = document.getElementById("alertstatus-" + id);
    statusObj.classList = [];
    statusObj.classList.add("alert-" + status);
    statusObj.innerText = status;

    const ludObj = document.getElementById("alertLud-" + id);
    ludObj.classList = [];
    ludObj.classList.add("alert-" + status);
    ludObj.innerText = lastUpdatedDate;
    setTimeout(function () {
        statusObj.classList = [];
        ludObj.classList = [];
    }, 3000);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
