﻿var index = 0;
var childIndex = 0;
var MaxQuantity = 0;
var PricePerUnit = 0;
let ReplyIndex = -1;

/* Old way function call is deprecated since jQuery 1.8 and completely removed in jQuery 3.0. */
$(function () {
    $('#body').summernote();
    $('#message').summernote();
    $('#description').summernote();
    $('#techInfo').summernote();
});

function PayOrder(orderId) {
    const win = window.open(`/Order/Payment/?orderId=${orderId}`, '_blank', 'location=yes,height=520,width=520,scrollbars=yes,status=yes');

    /* captures the window close event */
    win.addEventListener('beforeunload', e => {
        setTimeout(location.reload(true), 1000);
    });
}

function CancelReply(postId) {
    ReplyIndex = -1;
    $(`#replyBox_${postId}`).css('display', 'none');
}

function OnCancelReply(postId) {
    $(`#canceled_${postId}`).css('color', 'white');
}

function OnMouseOutReply(postId) {
    $(`#canceled_${postId}`).css('color', '#d9534f');
}

function ActivateReply(postId) {
    if (ReplyIndex < 0) {
        ReplyIndex = postId;
        $(`#body_${postId}`).summernote();
        $(`#replyBox_${postId}`).css('display', 'block');
    }
}

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
        $(childs[6].querySelector(`#changeOrder_${childIndex}`)).css('visibility', 'visible');
        $(childs[6].querySelector('#Stock')).val(parseInt(quantity));
    }
    else {
        // otherwise, revert initial control
        $(childs[6].querySelector(`#changeOrder_${childIndex}`)).css('visibility', 'hidden');
        $(`#quant_${childIndex}`).remove();

        $(parentControl.querySelector("#quantity")).css('display', 'inline');
        childIndex = 0;
    }
}

function ModifyQuantity(canEdit, newIndex, maxQuantity, pricePerUnit) {
    if (!canEdit) {
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
}

function OverQuantity(index, canEdit) {
    var childs = $(`#order_${index}`).children();
    var parentControl = childs[2];
    let control = parentControl.querySelector("#quantity");

    if (!canEdit) {
        control.style.border = '1px #8f7167 solid';
        control.style.borderRadius = '15%';
    }
    else {
        let value = 'Nu se poate modifica!';
        control.setAttribute("title", value);
    }
}

function LeaveQuantity(index, canEdit) {
    if (!canEdit) {
        var childs = $(`#order_${index}`).children();
        var parentControl = childs[2];

        let control = parentControl.querySelector("#quantity");
        control.style.border = 'none';
    }
}

function OnMouseOver() {
    $('#trash').css('color', 'white');
}

function OnMouseExit() {
    $('#trash').css('color', '#DC4C64');
}

function OnTrashOver() {
    $('#remove').css('color', 'white');
}

function OnTrashOut() {
    $('#remove').css('color', '#DC4C64');
}