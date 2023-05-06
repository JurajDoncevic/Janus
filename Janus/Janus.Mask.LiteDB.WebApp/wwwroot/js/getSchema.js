function getSchema(targetNodeId, outputElementId) {
    $.get(`/GetSchema/${targetNodeId}`, function (data, status) {
    })
        .done(function (data) {
            let prettyJson = JSON.stringify(data, undefined, 2);
            $(`#${outputElementId}`).text(prettyJson);
        })
        .fail(function (data) {
            $(`#${outputElementId}`).text(data.responseText);
        });
}