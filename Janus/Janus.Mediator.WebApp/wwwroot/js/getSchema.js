function getSchema(targetElement) {
    let targetNodeId = targetElement.getAttribute("data-node-id");
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "/GetSchema/" + targetNodeId, true);
    //xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send();
    xhr.onerror = function () {
        document.getElementById("schema-" + targetNodeId).textContent = this.responseText;
    }
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            let data = JSON.parse(this.responseText)
            document.getElementById("schema-" + targetNodeId).textContent = JSON.stringify(data, undefined, 2);
        } else {
            document.getElementById("schema-" + targetNodeId).textContent = this.responseText;
        }
    }
}