function getSchema(targetNodeId) {
    $.get(`/GetSchema/${targetNodeId}`, function (data, status) {
    })
        .done(function (data) {
            let prettyJson = JSON.stringify(data, undefined, 2);
            $(`#schema-${targetNodeId}`).text(prettyJson);
        })
        .fail(function (data) {
            $(`#schema-${targetNodeId}`).text(data.responseText);
        });
}

function getSchema(targetNodeId, targetElementId) {
    $.get(`/GetSchema/${targetNodeId}`, function (data, status) {
    })
        .done(function (data) {
            let prettyJson = JSON.stringify(data, undefined, 2);
            $(`#${targetElementId}`).text(prettyJson);
        })
        .fail(function (data) {
            $(`#${targetElementId}`).text(data.responseText);
        });
}