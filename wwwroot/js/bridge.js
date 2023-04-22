// Open account menu when mouse is over this control.
function FadeShow() {
    let dropdown1 = new bootstrap.Dropdown($('#dropdownMenuLink'));
    dropdown1.show();

    // hide dropdown
    let dropdown2 = new bootstrap.Dropdown($('#explore'));
    dropdown2.hide();
}

// View store products and plants.
function ExploreShop() {
    let dropdown1 = new bootstrap.Dropdown($('#explore'));
    dropdown1.show();

    // hide account menu
    let dropdown2 = new bootstrap.Dropdown($('#dropdownMenuLink'));
    dropdown2.hide();
}
