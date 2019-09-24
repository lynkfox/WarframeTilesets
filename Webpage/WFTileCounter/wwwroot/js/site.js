// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

function makeFileList() {
    var input = document.getElementById("filesToUpload");
    var ul = document.getElementById("fileList");
    while (ul.hasChildNodes()) {
        ul.removeChild(ul.firstChild);
    }
    for (var i = 0; i < input.files.length; i++) {
        var li = document.createElement("li");
        li.setAttribute('class', 'list-group-item list-group-item-dark')
        li.innerHTML = input.files[i].name;
        ul.appendChild(li);
    }
    if (!ul.hasChildNodes()) {
        var li = document.createElement("li");
        li.innerHTML = 'No Files Selected';
        li.setAttribute('class', 'list-group-item list-group-item-dark')
        ul.appendChild(li);
    }
}

