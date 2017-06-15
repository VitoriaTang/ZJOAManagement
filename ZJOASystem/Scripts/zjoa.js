function menuClick() {
    //Enable sidebar push menu
    if ($(window).width() > (screenSizes.sm - 1)) {
        if ($("body").hasClass('sidebar-collapse')) {
            $("body").removeClass('sidebar-collapse').trigger('expanded.pushMenu');
        } else {
            $("body").addClass('sidebar-collapse').trigger('collapsed.pushMenu');
        }
    }
        //Handle sidebar push menu for small screens
    else {
        if ($("body").hasClass('sidebar-open')) {
            $("body").removeClass('sidebar-open').removeClass('sidebar-collapse').trigger('collapsed.pushMenu');
        } else {
            $("body").addClass('sidebar-open').trigger('expanded.pushMenu');
        }
    }
    return false;
}

screenSizes = function () {
    var xs = 480;
    var sm = 768;
    var md = 992;
    var lg = 1200;
};