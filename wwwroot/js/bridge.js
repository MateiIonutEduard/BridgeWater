var index = 0;

$(document).ready(function () {
    $('#body').summernote();

    $("#rating-form").submit(function (e) {
        e.preventDefault();
        let formData = new FormData();

        formData.append('accountId', $('#auid').val());
        formData.append('productId', $('#puid').val());

        formData.append('body', $('#body').summernote('code'));
        formData.append('rating', index);

        // disable submit button
        $("#sender").attr("disabled", true);

        $.ajax({
            url: '/Home/CreatePost',
            type: 'post',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: data => {
                console.log("New post was created successfully.");
            },
            statusCode: {
                200: () => {
                    setTimeout(() => {
                        // enable submit button and refresh page
                        $("#sender").attr("disabled", false);
                        location.reload();
                    }, 200);
                },
                400: () => {
                    alert('Trebuie sa aveti campurile completate!');
                    console.log("The post rating cannot be empty.");
                },
                403: () => {
                    alert('Nu puteti lasa mai mult de o recenzie!');
                    console.log("The post rating already exists.");
                }
            },
            async: true
        });

        return true;

    });
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
