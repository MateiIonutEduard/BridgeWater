var index = 0;
var childIndex = 0;
var MaxQuantity = 0;
var PricePerUnit = 0;

$(document).ready(function () {
    $('#body').summernote();
    $('#description').summernote();
    $('#techInfo').summernote();
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
    var quantity = parseFloat($(`#quant_${childIndex}`).val());
    var price = quantity * PricePerUnit;

    var childs = $(`#order_${childIndex}`).children();
    var parentControl = childs[2];

    var realPrice = Math.trunc(price * 100) / 100;
    $(childs[3].querySelector('#totalPrice')).html(`${realPrice.toFixed(2)}&nbsp;Lei`);

    // display update button if quantity was changed
    if (quantity < MaxQuantity) {
        $(childs[5].querySelector('#changeOrder')).css('visibility', 'visible');
        $(childs[5].querySelector('#Stock')).val(parseInt(quantity));
    }
    else {
        // otherwise, revert initial control
        $(childs[5].querySelector('#changeOrder')).css('visibility', 'hidden');
        $(`#quant_${childIndex}`).remove();

        $(parentControl.querySelector("#quantity")).css('display', 'block');
        childIndex = 0;
    }
}

function ModifyQuantity(newIndex, maxQuantity, pricePerUnit) {
    if (childIndex !== 0) {
        if (childIndex === newIndex) {
            var childs = $(`#order_${newIndex}`).children();
            var parentControl = childs[2];
            $(parentControl.querySelector("#quantity")).css('display', 'none');

            MaxQuantity = maxQuantity;
            PricePerUnit = pricePerUnit;

            const nextUnit = `<input onchange='UpdatePrice()' class='form-control form-control-sm' style='width: 80px !important;' id='quant_${newIndex}' name='quant_${newIndex}' type='number' value='${maxQuantity}' min='1' max='${maxQuantity}' />`;
            $(parentControl).append(nextUnit);
        }
    }
    else {
        var childs = $(`#order_${newIndex}`).children();
        var parentControl = childs[2];
        $(parentControl.querySelector("#quantity")).css('display', 'none');

        MaxQuantity = maxQuantity;
        PricePerUnit = pricePerUnit;
        childIndex = newIndex;

        const nextUnit = `<input onchange='UpdatePrice()' class='form-control form-control-sm' style='width: 80px !important;' id='quant_${newIndex}' name='quant_${newIndex}' type='number' value='${maxQuantity}' min='1' max='${maxQuantity}' />`;
        $(parentControl).append(nextUnit);
    }
}