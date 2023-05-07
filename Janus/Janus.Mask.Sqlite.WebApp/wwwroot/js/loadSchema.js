function loadSchema(targetNodeId) {
    $.get(`/LoadSchema/${targetNodeId}`, function (data, status) {
        //return data;
    })
        .done(function (data) {
            location.reload();
        })
        .fail(function (data) {
            $("#load-schema-notif").removeAttr("hidden");
            $("#load-schema-notif").text("Error while loading schema from " + targetNodeId + ": " + data.responseText);
        });
}