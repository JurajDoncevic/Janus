function unloadSchema() {
    $.get(`/UnloadSchema/`, function (data, status) {
        //return data;
    })
        .done(function (data) {
            location.reload();
        })
        .fail(function (data) {
            $("#load-schema-notif").removeAttr("hidden");
            $("#load-schema-notif").text("Error while unloading schema: " + data.responseText);
        });
}