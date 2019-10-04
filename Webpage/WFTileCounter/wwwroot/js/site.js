// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//Bootstrap Enablers
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

$(function () {
    $('[data-toggle="popover"]').popover()
})


//FileList for Upload Files

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
};




//Checkboxes update flexbox above them -- Kinda messy here. Atm all checkboxes are within a span then the flexbox... so yeah. not ideal.

$(document).ready(function () {
    $('input[type="checkbox"]').click(function () {

        var parent = $(this).parent();
        var parentSecond = $(parent).parent();

        if ($(this).hasClass("blueCheckbox")) {
            changeBorderBlue(this, this);
        } else if ($(parent).hasClass("blueCheckbox")) {
            changeBorderBlue(this, parent)
        } else if ($(parentSecond).hasClass("blueCheckbox")) {
            changeBorderBlue(this, parentSecond)
        } else if ($(this).hasClass("checkBoxRed")) {
            changeBorderBlue(this, this);
        } else if ($(parent).hasClass("checkBoxRed")) {
            changeBorderBlue(this, parent)
        } else if ($(parentSecond).hasClass("checkBoxRed")) {
            changeBorderBlue(this, parentSecond)
        }

    });
});



function changeBackgroundRed(checkbox, borderBox) {

    if ($(checkbox).is(":checked")) {

        $(borderBox).removeClass("bg-dark");
        $(borderBox).addClass("bg-success");

    }
    else {
        $(borderBox).removeClass("bg-success");
        $(borderBox).addClass("bg-dark");
    }

};

function changeBorderBlue(checkbox, borderBox) {

    if ($(checkbox).is(":checked")) {

        $(borderBox).removeClass("border-light");
        $(borderBox).addClass("border-info");

    }
    else {
        $(borderBox).removeClass("border-info");
        $(borderBox).addClass("border-light");
    }

};




