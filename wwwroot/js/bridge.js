var index = 0;
var ChildControl = undefined;
var MaxQuantity = 0;
var PricePerUnit = 0;

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
    $('#rating').val(index);
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

// detect if quantity was changed
function UpdatePrice() {
    var quantity = parseFloat($("#quant").val());
    var price = quantity * PricePerUnit;

    var realPrice = Math.trunc(price * 100) / 100;
    $('#totalPrice').html(`${realPrice.toFixed(2)}&nbsp;Lei`);

    // display update button if quantity was changed
    if (quantity < MaxQuantity) {
        $('#changeOrder').css('visibility', 'visible');
        $('#Stock').val(parseInt(quantity));
    }
    else {
        // otherwise, revert initial control
        $('#changeOrder').css('visibility', 'hidden');
        $('#quant').remove();

        $("#quantity").css('display', 'block');
    }
}

function ModifyQuantity(control, maxQuantity, pricePerUnit) {
    if (ChildControl !== undefined) {
        if (ChildControl === control) {
            var parentControl = $(control).parent();
            $("#quantity").css('display', 'none');

            MaxQuantity = maxQuantity;
            PricePerUnit = pricePerUnit;

            const nextUnit = `<input onchange='UpdatePrice()' class='form-control form-control-sm' style='width: 80px !important;' id='quant' name='quant' type='number' value='${maxQuantity}' min='1' max='${maxQuantity}' />`;
            parentControl.append(nextUnit);
        }
    }
    else {
        var parentControl = $(control).parent();
        $("#quantity").css('display', 'none');

        MaxQuantity = maxQuantity;
        PricePerUnit = pricePerUnit;
        ChildControl = control;

        const nextUnit = `<input onchange='UpdatePrice()' class='form-control form-control-sm' style='width: 80px !important;' id='quant' name='quant' type='number' value='${maxQuantity}' min='1' max='${maxQuantity}' />`;
        parentControl.append(nextUnit);
    }
}