function loadSchema(targetElement) {
    let targetNodeId = targetElement.getAttribute("data-node-id");
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "/LoadSchema/" + targetNodeId, true);
    //xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send();
    xhr.onerror = function () {
        document.getElementById("schema-" + targetNodeId).textContent = this.responseText;
    }
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            location.reload();
        } else {
            document.getElementById("load-schema-notif").removeAttribute("hidden");
            document.getElementById("load-schema-notif").textContent = "Error while loading schema from " + targetNodeId + ": " + this.responseText;
        }
    }
}