var index = 0;

$(document).ready(function () {
    $('#body').summernote();
});

// fill stars on click, store index
function RateUs(id) {
    for (var j = 1; j <= 5; j++)
        $(`#star${j}`).removeClass("checked");

    for (var k = 1; k <= id; k++)
        $(`#star${k}`).addClass("checked");

    index = id;
}

// fill stars on hover
function HoverUs(id) {
    for (var k = 1; k <= id; k++)
        $(`#star${k}`).addClass("checked");
}

// empty stars when mouse out
function ClearStars() {
    for (var j = index + 1; j <= 5; j++)
        $(`#star${j}`).removeClass("checked");
}
