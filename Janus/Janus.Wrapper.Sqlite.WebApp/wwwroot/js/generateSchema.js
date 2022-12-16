function generateSchema(targetElement) {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "/Schema/GenerateSchema", true);
    //xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.send();
    xhr.onload = function () {
        console.log(this.responseText)
        if (xhr.status >= 200 && xhr.status < 300) {
            location.reload();
        } else {
            document.getElementById("schema-generation-outcome").textContent = this.responseText;
        }
    }
}